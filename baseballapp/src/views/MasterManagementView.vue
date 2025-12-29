<template>
    <v-container fluid class="pa-4">
        <h1 class="text-h4 mb-6">マスタ管理</h1>

        <v-tabs v-model="activeTab" bg-color="primary" color="white" @update:model-value="resetSelection">
            <v-tab value="players" prepend-icon="mdi-account-group">選手一覧</v-tab>
            <v-tab value="teams" prepend-icon="mdi-shield-home">チーム一覧</v-tab>
        </v-tabs>

        <v-row class="mt-2">
            <v-col cols="12" md="7">
                <v-card class="rounded-lg elevation-4 fill-height">
                    <v-toolbar color="transparent" density="compact">
                        <v-text-field v-model="search" prepend-inner-icon="mdi-magnify"
                            :label="activeTab === 'players' ? '選手検索' : 'チーム検索'" variant="solo" density="compact"
                            hide-details flat class="px-4"></v-text-field>
                        <v-spacer></v-spacer>
                        <v-btn color="primary" variant="flat" prepend-icon="mdi-plus" class="mr-4"
                            @click="resetSelection">
                            新規作成
                        </v-btn>
                    </v-toolbar>

                    <v-divider></v-divider>

                    <v-data-table :headers="activeTab === 'players' ? playerHeaders : teamHeaders"
                        :items="activeTab === 'players' ? players : teams" :search="search" density="comfortable" hover
                        class="master-list-table">
                        <template v-slot:item="{ item }">
                            <tr :class="{ 'selected-row': selectedId === item.id }" @click="selectItem(item)"
                                style="cursor: pointer">
                                <template v-if="activeTab === 'players'">
                                    <td>{{ item.number }}</td>
                                    <td>{{ item.name }}</td>
                                    <td>{{ item.position }}</td>
                                </template>
                                <template v-else>
                                    <td>{{ item.name }}</td>
                                    <td>{{ item.location }}</td>
                                </template>
                                <td class="text-end">
                                    <v-btn icon="mdi-delete" size="x-small" color="red-lighten-1" variant="text"
                                        @click.stop="confirmDelete(item)"></v-btn>
                                </td>
                            </tr>
                        </template>
                    </v-data-table>
                </v-card>
            </v-col>

            <v-col cols="12" md="5">
                <v-card class="rounded-lg elevation-4 fill-height">
                    <v-card-title class="bg-grey-lighten-4 py-3 d-flex align-center">
                        <v-icon :icon="selectedId ? 'mdi-pencil' : 'mdi-plus'" class="mr-2"></v-icon>
                        {{ selectedId ? '情報を編集' : '新規データ追加' }}
                        <v-spacer></v-spacer>
                        <v-chip v-if="selectedId" size="small" color="info">編集モード</v-chip>
                        <v-chip v-else size="small" color="success">新規モード</v-chip>
                    </v-card-title>

                    <v-divider></v-divider>

                    <v-card-text class="pa-6">
                        <v-form ref="form">
                            <v-row dense>
                                <template v-if="activeTab === 'players'">
                                    <v-col cols="12">
                                        <v-text-field v-model="formData.name" label="選手名"
                                            variant="outlined"></v-text-field>
                                    </v-col>
                                    <v-col cols="6">
                                        <v-text-field v-model.number="formData.number" label="背番号" type="number"
                                            variant="outlined"></v-text-field>
                                    </v-col>
                                    <v-col cols="6">
                                        <v-select v-model="formData.position" :items="positions" label="ポジション"
                                            variant="outlined"></v-select>
                                    </v-col>
                                </template>

                                <template v-else>
                                    <v-col cols="12">
                                        <v-text-field v-model="formData.name" label="チーム名"
                                            variant="outlined"></v-text-field>
                                    </v-col>
                                    <v-col cols="12">
                                        <v-text-field v-model="formData.location" label="活動場所"
                                            variant="outlined"></v-text-field>
                                    </v-col>
                                </template>
                            </v-row>
                        </v-form>
                    </v-card-text>

                    <v-divider></v-divider>

                    <v-card-actions class="pa-4">
                        <v-btn v-if="selectedId" variant="outlined" color="grey" @click="resetSelection">キャンセル</v-btn>
                        <v-spacer></v-spacer>
                        <v-btn color="primary" variant="flat" size="large" block @click="save"
                            style="max-width: 200px;">
                            {{ selectedId ? '更新を保存' : '新規登録' }}
                        </v-btn>
                    </v-card-actions>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
    name: 'MasterManagementView',

    data() {
        return {
            activeTab: 'players',
            search: '',
            selectedId: null as number | null,

            // フォーム用データ
            formData: {
                id: null as number | null,
                name: '',
                number: null as number | null,
                position: '',
                location: ''
            },

            // マスタデータ（本来はAPIから取得）
            players: [
                { id: 1, name: '山田 太郎', number: 1, position: '投手' },
                { id: 2, name: '佐藤 勝利', number: 10, position: '遊撃手' },
            ],
            teams: [
                { id: 1, name: '東京ファイターズ', location: '東京都' },
                { id: 2, name: '千葉マリーンズ', location: '千葉県' },
            ],

            // 定数
            positions: ['投手', '捕手', '一塁手', '二塁手', '三塁手', '遊撃手', '左翼手', '中堅手', '右翼手'],
            playerHeaders: [
                { title: '番号', key: 'number', width: '80px' },
                { title: '選手名', key: 'name' },
                { title: '役割', key: 'position' },
                { title: '', key: 'actions', sortable: false, align: 'end' as const },
            ],
            teamHeaders: [
                { title: 'チーム名', key: 'name' },
                { title: '場所', key: 'location' },
                { title: '', key: 'actions', sortable: false, align: 'end' as const },
            ],
        }
    },

    created() {
        // ページ読み込み時の処理が必要な場合はここに記述
        console.log('Master Management View Initialized')
    },

    methods: {
        // アイテムを選択して右側のフォームに反映
        selectItem(item: any) {
            this.selectedId = item.id
            // データのコピー（参照を切るためにスプレッド演算子を使用）
            this.formData = { ...item }
        },

        // 選択解除（新規作成モードへ）
        resetSelection() {
            this.selectedId = null
            this.formData = {
                id: null,
                name: '',
                number: null,
                position: '',
                location: ''
            }
        },

        // 保存処理
        save() {
            const mode = this.selectedId ? '更新' : '登録'

            // カスタムイベントの発火（通知用）
            window.dispatchEvent(new CustomEvent('show-modal', {
                detail: {
                    title: `${mode}完了`,
                    message: `${this.formData.name} を${mode}しました。`,
                    color: 'success',
                    icon: 'mdi-check-circle'
                }
            }))

            // 保存後の後処理
            if (!this.selectedId) {
                this.resetSelection()
            }
        },

        // 削除確認
        confirmDelete(item: any) {
            if (confirm(`${item.name} を削除しますか？`)) {
                this.resetSelection()
                // ここに削除APIの呼び出しを記述
                console.log('Delete item:', item.id)
            }
        }
    }
})
</script>

<style scoped>
/* 選択された行のハイライト */
.selected-row {
    background-color: #e3f2fd !important;
    font-weight: bold;
}

.master-list-table :deep(tr:hover:not(.selected-row)) {
    background-color: #f5f5f5 !important;
}

.fill-height {
    height: 100%;
}
</style>