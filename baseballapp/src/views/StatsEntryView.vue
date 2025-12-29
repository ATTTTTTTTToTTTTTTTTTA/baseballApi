<template>
    <v-container fluid class="pa-4">
        <div class="d-flex align-center mb-6">
            <h1 class="text-h4">成績入力</h1>
            <v-chip class="ml-4" color="primary" variant="tonal">2023年度 シーズン</v-chip>
            <v-spacer></v-spacer>
            <v-btn color="success" prepend-icon="mdi-content-save" elevation="2" @click="saveStats">
                成績を保存
            </v-btn>
        </div>

        <v-card class="mb-6 pa-4 rounded-lg elevation-4">
            <v-row dense>
                <v-col cols="12" sm="3">
                    <v-text-field v-model="matchInfo.date" label="試合の日付" type="date" variant="outlined"
                        density="compact" hide-details></v-text-field>
                </v-col>
                <v-col cols="12" sm="3">
                    <v-text-field v-model="matchInfo.opponent" label="対戦相手" placeholder="チーム名" variant="outlined"
                        density="compact" hide-details></v-text-field>
                </v-col>
                <v-col cols="12" sm="3">
                    <v-row dense align="center">
                        <v-col><v-text-field v-model.number="matchInfo.ourScore" label="自軍" type="number"
                                variant="outlined" density="compact" hide-details></v-text-field></v-col>
                        <v-col cols="auto" class="font-weight-bold">-</v-col>
                        <v-col><v-text-field v-model.number="matchInfo.opponentScore" label="相手" type="number"
                                variant="outlined" density="compact" hide-details /></v-col>
                    </v-row>
                </v-col>
                <v-col cols="12" sm="3">
                    <v-text-field v-model="matchInfo.mvp" label="MVP" placeholder="選手名" variant="outlined"
                        density="compact" hide-details prepend-inner-icon="mdi-trophy-variant"></v-text-field>
                </v-col>
            </v-row>
        </v-card>

        <v-card class="rounded-lg elevation-4 overflow-hidden">
            <div class="stats-table-wrapper">
                <v-data-table :headers="headers" :items="players" class="stats-table" density="compact"
                    hide-default-footer :items-per-page="-1">
                    <template v-slot:item.name="{ item }">
                        <div class="player-name-fixed font-weight-bold">{{ item.name }}</div>
                    </template>

                    <template v-slot:item.plate_appearances="{ item }"><input v-model.number="item.pa" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.at_bats="{ item }"><input v-model.number="item.ab" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.hits="{ item }">{{ calculateHits(item) }}</template>
                    <template v-slot:item.b1="{ item }"><input v-model.number="item.b1" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.b2="{ item }"><input v-model.number="item.b2" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.b3="{ item }"><input v-model.number="item.b3" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.hr="{ item }"><input v-model.number="item.hr" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.runs="{ item }"><input v-model.number="item.runs" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.rbi="{ item }"><input v-model.number="item.rbi" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.sh="{ item }"><input v-model.number="item.sh" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.sf="{ item }"><input v-model.number="item.sf" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.bb_hbp="{ item }"><input v-model.number="item.bb_hbp" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.so="{ item }"><input v-model.number="item.so" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.sb_att="{ item }"><input v-model.number="item.sb_att" type="number"
                            class="stats-input"></template>
                    <template v-slot:item.sb_suc="{ item }"><input v-model.number="item.sb_suc" type="number"
                            class="stats-input"></template>

                    <template v-slot:item.tb="{ item }"><span class="calculated-cell font-weight-bold">{{
                            calculateTB(item) }}</span></template>
                    <template v-slot:item.slg="{ item }"><span class="calculated-cell">{{ calculateSLG(item)
                            }}</span></template>
                    <template v-slot:item.avg="{ item }"><span class="calculated-cell">{{ calculateAVG(item)
                            }}</span></template>
                    <template v-slot:item.obp="{ item }"><span class="calculated-cell">{{ calculateOBP(item)
                            }}</span></template>
                    <template v-slot:item.ops="{ item }">
                        <div class="ops-cell-fixed">
                            <v-chip size="x-small" :color="getOpsColor(calculateOPS(item))" label
                                class="font-weight-black">
                                {{ calculateOPS(item) }}
                            </v-chip>
                        </div>
                    </template>
                </v-data-table>
            </div>
        </v-card>
    </v-container>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
    name: 'StatsInputView',

    data() {
        return {
            // 試合基本情報
            matchInfo: {
                date: new Date().toISOString().substr(0, 10),
                opponent: '',
                ourScore: 0,
                opponentScore: 0,
                mvp: ''
            },
            // 選手データ
            players: [
                { name: '山田 太郎', pa: 0, ab: 0, b1: 0, b2: 0, b3: 0, hr: 0, runs: 0, rbi: 0, sh: 0, sf: 0, bb_hbp: 0, so: 0, sb_att: 0, sb_suc: 0 },
                { name: '佐藤 勝利', pa: 0, ab: 0, b1: 0, b2: 0, b3: 0, hr: 0, runs: 0, rbi: 0, sh: 0, sf: 0, bb_hbp: 0, so: 0, sb_att: 0, sb_suc: 0 },
                { name: '鈴木 一朗', pa: 0, ab: 0, b1: 0, b2: 0, b3: 0, hr: 0, runs: 0, rbi: 0, sh: 0, sf: 0, bb_hbp: 0, so: 0, sb_att: 0, sb_suc: 0 },
            ],
            // テーブルヘッダー
            headers: [
                { title: '選手名', key: 'name', sortable: false },
                { title: '打席', key: 'plate_appearances', sortable: false },
                { title: '打数', key: 'at_bats', sortable: false },
                { title: '安打', key: 'hits', sortable: false },
                { title: '1塁打', key: 'b1', sortable: false },
                { title: '2塁打', key: 'b2', sortable: false },
                { title: '3塁打', key: 'b3', sortable: false },
                { title: '本塁打', key: 'hr', sortable: false },
                { title: '得点', key: 'runs', sortable: false },
                { title: '打点', key: 'rbi', sortable: false },
                { title: '犠打', key: 'sh', sortable: false },
                { title: '犠飛', key: 'sf', sortable: false },
                { title: '四死球', key: 'bb_hbp', sortable: false },
                { title: '三振', key: 'so', sortable: false },
                { title: '盗塁企', key: 'sb_att', sortable: false },
                { title: '成功', key: 'sb_suc', sortable: false },
                { title: '塁打', key: 'tb', sortable: false },
                { title: '長打率', key: 'slg', sortable: false },
                { title: '打率', key: 'avg', sortable: false },
                { title: '出塁率', key: 'obp', sortable: false },
                { title: 'OPS', key: 'ops', sortable: false },
            ]
        }
    },

    methods: {
        // 計算ロジック
        calculateHits(p: any) { return p.b1 + p.b2 + p.b3 + p.hr },
        calculateTB(p: any) { return (p.b1 * 1) + (p.b2 * 2) + (p.b3 * 3) + (p.hr * 4) },
        calculateAVG(p: any) {
            return p.ab > 0 ? (this.calculateHits(p) / p.ab).toFixed(3) : '.000'
        },
        calculateSLG(p: any) {
            return p.ab > 0 ? (this.calculateTB(p) / p.ab).toFixed(3) : '.000'
        },
        calculateOBP(p: any) {
            const denom = p.ab + p.bb_hbp + p.sf
            return denom > 0 ? ((this.calculateHits(p) + p.bb_hbp) / denom).toFixed(3) : '.000'
        },
        calculateOPS(p: any) {
            return (parseFloat(this.calculateOBP(p)) + parseFloat(this.calculateSLG(p))).toFixed(3)
        },
        getOpsColor(ops: string) {
            const val = parseFloat(ops)
            if (val >= 0.9) return 'deep-orange'
            if (val >= 0.8) return 'orange'
            if (val >= 0.7) return 'blue'
            return 'grey'
        },
        // 保存処理
        saveStats() {
            window.dispatchEvent(new CustomEvent('show-modal', {
                detail: {
                    title: '保存成功',
                    message: `${this.matchInfo.opponent} 戦の選手成績を保存しました。`,
                    color: 'success',
                    icon: 'mdi-cloud-check'
                }
            }))
        }
    }
})
</script>

<style scoped>
/* スタイル部分は元のコードを継承 */
.stats-table-wrapper {
    overflow-x: auto;
    position: relative;
}

.stats-table {
    min-width: 1400px;
}

:deep(.v-data-table__table) {
    border-collapse: separate;
    border-spacing: 0;
}

:deep(th:first-child),
:deep(td:first-child) {
    position: sticky !important;
    left: 0;
    z-index: 2;
    background-color: #f8fafd !important;
    border-right: 2px solid #ddecff !important;
}

:deep(th) {
    background-color: #f0f7ff !important;
    font-weight: bold !important;
    font-size: 0.7rem !important;
    white-space: nowrap;
    text-align: center !important;
    border-bottom: 2px solid #e0e0e0 !important;
}

:deep(th:nth-child(5)),
:deep(th:nth-child(6)),
:deep(th:nth-child(7)),
:deep(th:nth-child(8)) {
    background-color: #fff9e6 !important;
}

.stats-input {
    width: 42px;
    border: 1px solid #ced4da;
    border-radius: 4px;
    padding: 2px;
    text-align: center;
    font-size: 0.85rem;
    background: white;
    transition: all 0.2s;
}

.stats-input:focus {
    border-color: #1976D2;
    outline: none;
}

.calculated-cell {
    color: #666;
    font-size: 0.85rem;
}

:deep(tr:hover td) {
    background-color: #e3f2fd !important;
}

.stats-input::-webkit-outer-spin-button,
.stats-input::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
}
</style>