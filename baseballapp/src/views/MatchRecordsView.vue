<template>
    <v-container fluid class="pa-4">
        <h1 class="text-h4 mb-6">試合記録</h1>

        <v-row class="mb-6" align="stretch">
            <v-col cols="12" md="8">
                <v-card class="pa-4 rounded-lg elevation-4 fill-height">
                    <v-card-title class="text-subtitle-1 font-weight-bold px-0 mb-2">試合の絞り込み</v-card-title>
                    <v-card-text class="px-0">
                        <v-row dense align="center">
                            <v-col cols="12" sm="4">
                                <v-text-field v-model="filterDate" label="日付" type="date" variant="outlined"
                                    density="compact" hide-details></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="4">
                                <v-text-field v-model="search" label="対戦相手" placeholder="チーム名など" variant="outlined"
                                    density="compact" hide-details></v-text-field>
                            </v-col>
                            <v-col cols="12" sm="3">
                                <v-select v-model="filterResult" :items="['すべて', 'Win', 'Loss', 'Draw']" label="結果"
                                    variant="outlined" density="compact" hide-details></v-select>
                            </v-col>
                            <v-col cols="12" sm="1" class="d-flex justify-center">
                                <v-btn icon="mdi-magnify" color="primary" elevation="2" @click="onSearch"></v-btn>
                            </v-col>
                        </v-row>
                    </v-card-text>
                </v-card>
            </v-col>

            <v-col cols="12" md="4">
                <v-card class="pa-3 rounded-lg elevation-4 fill-height text-center">
                    <v-card-title class="text-subtitle-1 font-weight-bold pb-0">勝敗比率</v-card-title>
                    <v-card-text class="d-flex justify-center align-center" style="height: 160px;">
                        <div style="max-width: 200px; width: 100%; height: 100%;">
                            <Pie :data="chartData" :options="chartOptions" v-if="hasChartData" />
                            <p v-else class="text-center text-medium-emphasis">なし</p>
                        </div>
                    </v-card-text>
                </v-card>
            </v-col>
        </v-row>

        <v-card class="rounded-lg elevation-4">
            <v-card-title class="d-flex align-center">
                試合一覧
                <v-spacer></v-spacer>
                <!-- <v-text-field v-model="search" density="compact" label="検索" prepend-inner-icon="mdi-magnify" variant="outlined" flat hide-details single-line style="max-width: 300px;"></v-text-field> -->
                <v-btn color="primary" prepend-icon="mdi-plus" elevation="2" @click="editItem(null)">
                    新規追加
                </v-btn>
            </v-card-title>

            <v-data-table-server v-model:items-per-page="itemsPerPage" :headers="headers" :items="serverItems"
                :items-length="totalItems" :loading="loading" :search="search" item-value="id"
                @update:options="loadItems" class="elevation-0">
                <template v-slot:item.date="{ item }">
                    {{ new Date(item.date).toLocaleDateString('ja-JP') }}
                </template>
                <template v-slot:item.result="{ item }">
                    <v-chip :color="getResultColor(item.result)" label size="small" class="font-weight-bold">
                        {{ item.result === 'Win' ? '勝利' : item.result === 'Loss' ? '敗北' : '引き分け' }}
                    </v-chip>
                </template>
                <template v-slot:item.score="{ item }">
                    {{ item.ourScore }} - {{ item.opponentScore }}
                </template>
                <template v-slot:item.MVP="{ item }">
                    <span v-if="item.MVP">{{ item.MVP }}</span>
                    <span v-else class="text-grey-lighten-1">—</span>
                </template>
                <template v-slot:item.actions="{ item }">
                    <div class="d-flex justify-end">
                        <v-btn icon="mdi-pencil" variant="text" size="small" color="blue-darken-1"
                            @click="editItem(item)" class="mr-2"></v-btn>
                        <v-btn icon="mdi-delete" variant="text" size="small" color="red-darken-1"
                            @click="deleteItem(item)"></v-btn>
                    </div>
                </template>
            </v-data-table-server>
        </v-card>
    </v-container>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { mockMatches, type MatchRecord } from '../data/mockMatches';
import { Pie } from 'vue-chartjs';
import { Chart as ChartJS, Title, Tooltip, Legend, ArcElement, CategoryScale } from 'chart.js';
import { confirmAction, showToast } from '../utils/dialog';

ChartJS.register(Title, Tooltip, Legend, ArcElement, CategoryScale);

export default defineComponent({
    name: 'MatchRecordsView',
    components: { Pie },

    data() {
        return {
            // フィルタ・検索用
            filterDate: '',
            filterResult: 'すべて',
            search: '',

            // テーブル用
            itemsPerPage: 10,
            loading: true,
            totalItems: 0,
            serverItems: [] as MatchRecord[],
            headers: [
                { title: '日付', key: 'date', align: 'start' },
                { title: '対戦相手', key: 'opponent', align: 'start' },
                { title: 'スコア', key: 'score', sortable: false },
                { title: '結果', key: 'result', align: 'start' },
                { title: 'MVP', key: 'MVP', align: 'start', sortable: false },
                { title: '操作', key: 'actions', align: 'end', sortable: false },
            ],

            // グラフオプション
            chartOptions: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { position: 'right', labels: { usePointStyle: true, font: { size: 14 } } },
                    tooltip: {
                        callbacks: {
                            label: (context: any) => {
                                const value = context.parsed;
                                const total = mockMatches.length;
                                const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                                return `${context.label}: ${value} (${percentage}%)`;
                            }
                        }
                    }
                },
            }
        };
    },

    computed: {
        // グラフ用データ
        chartData() {
            const wins = mockMatches.filter(m => m.result === 'Win').length;
            const losses = mockMatches.filter(m => m.result === 'Loss').length;
            const draws = mockMatches.filter(m => m.result === 'Draw').length;

            return {
                labels: ['勝利', '敗北', '引き分け'],
                datasets: [{
                    backgroundColor: ['#4CAF50', '#F44336', '#FFC107'],
                    data: [wins, losses, draws],
                }],
            };
        },
        hasChartData() {
            return this.chartData.datasets[0].data.some(d => d > 0);
        }
    },

    watch: {
        // searchが変更されたらテーブルをリロード
        search() {
            this.onSearch();
        }
    },

    created() {
        // 初期化時に必要な処理があればここに記述
        console.log('MatchRecordsView created');
    },

    methods: {
        // データ読み込み用（v-data-table-serverから呼ばれる）
        loadItems({ page, itemsPerPage, sortBy, search }: any) {
            this.loading = true;
            let items: MatchRecord[] = [...mockMatches];

            // 検索フィルタ
            const query = search || this.search;
            if (query) {
                const lowerSearch = query.toLowerCase();
                items = items.filter(item =>
                    item.opponent.toLowerCase().includes(lowerSearch) ||
                    item.MVP?.toLowerCase().includes(lowerSearch) ||
                    item.result.toLowerCase().includes(lowerSearch)
                );
            }

            this.totalItems = items.length;

            // ソート処理
            if (sortBy && sortBy.length > 0) {
                const sortKey = sortBy[0].key as keyof MatchRecord;
                const sortOrder = sortBy[0].order;
                items.sort((a, b) => {
                    const aValue = a[sortKey] ?? '';
                    const bValue = b[sortKey] ?? '';
                    if (typeof aValue === 'string' && typeof bValue === 'string') {
                        return sortOrder === 'asc' ? aValue.localeCompare(bValue) : bValue.localeCompare(aValue);
                    }
                    return 0;
                });
            }

            // ページネーション
            const start = (page - 1) * itemsPerPage;
            const end = start + itemsPerPage;
            this.serverItems = items.slice(start, end);

            this.itemsPerPage = itemsPerPage;
            this.loading = false;
        },

        // 検索ボタンクリック時
        onSearch() {
            // ページ1に戻して再読み込みを走らせる
            this.loadItems({
                page: 1,
                itemsPerPage: this.itemsPerPage,
                sortBy: [],
                search: this.search
            });
        },

        getResultColor(result: 'Win' | 'Loss' | 'Draw') {
            if (result === 'Win') return 'success';
            if (result === 'Loss') return 'error';
            return 'warning';
        },

        editItem(item: MatchRecord | null) {
            this.$router.push("/statsenrty");
            //   window.dispatchEvent(new CustomEvent('show-modal', {
            //     detail: {
            //       title: '編集モード',
            //       message: `${item.opponent} との試合記録は、現在編集できません。`,
            //       color: 'info',
            //       icon: 'mdi-pencil-lock'
            //     }
            //   }));
        },

        async deleteItem(item: MatchRecord) {
            const confirmed = await confirmAction({
                title: '削除の確認',
                message: `削除してもよろしいですか？`,
                color: 'red-darken-1',
                icon: 'mdi-delete-alert'
            });
            console.log(confirmed)

            if (confirmed) {
                // 削除実行処理

                showToast("削除しました")
            }
        }
    }
});
</script>