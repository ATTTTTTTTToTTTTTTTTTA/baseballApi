<template>
    <v-app-bar color="white" elevation="1" app>
        <v-app-bar-nav-icon @click.stop="drawer = !drawer" color="primary" />
        <v-app-bar-title class="app-title">
            BASEBALL <span class="text-primary">API</span>
        </v-app-bar-title>
    </v-app-bar>

    <v-navigation-drawer v-model="drawer" app permanent border="none" class="sidebar-bg">
        <div class="pa-6 text-overline text-grey-darken-1">MENU</div>

        <v-list nav class="px-3">
            <v-list-item prepend-icon="mdi-account-group" title="試合一覧" to="/main" class="menu-item mb-2"
                active-class="menu-item-active" />
            <v-list-item prepend-icon="mdi-whistle" title="試合結果" to="/statsenrty" class="menu-item mb-2"
                active-class="menu-item-active" />
            <v-list-item prepend-icon="mdi-wrench" title="マスタ" to="/master" class="menu-item mb-2"
                active-class="menu-item-active" />
        </v-list>
    </v-navigation-drawer>
</template>

<script lang="ts">
import { defineComponent, ref } from 'vue'
export default defineComponent({
    setup() {
        const drawer = ref(true)
        return { drawer }
    }
})
</script>

<style scoped>
/* アプリタイトルの強調 */
.app-title {
    font-weight: 800;
    letter-spacing: 1px;
    font-size: 1.25rem;
    color: #2c3e50;
}

/* サイドバーの背景色：爽やかな薄い青 */
.sidebar-bg {
    background-color: #f0f7ff !important;
    border-right: 1px solid #ddecff !important;
}

/* メニュー全体のカスタマイズ */
.menu-item {
    border-radius: 12px !important;
    transition: all 0.3s ease;
    /* 動きを少し滑らかに */
    padding: 12px 16px !important;
    color: #455a64 !important;
    position: relative;
    /* アクセントバー表示用 */
    overflow: hidden;
}

/* メニューの文字：大きく、太く */
:deep(.v-list-item-title) {
    font-size: 1.1rem !important;
    font-weight: 600 !important;
    transition: color 0.3s ease;
}

/* アイコンのサイズと色 */
:deep(.v-list-item__prepend .v-icon) {
    font-size: 1.4rem !important;
    margin-inline-end: 20px !important;
    transition: color 0.3s ease;
}

/* --- ★ 選択中（アクティブ）のスタイル --- */
.menu-item-active {
    background: linear-gradient(135deg, #1976D2, #42A5F5) !important;
    box-shadow: 0 4px 15px rgba(25, 118, 210, 0.35) !important;
}

/* 選択中の文字とアイコンを白く太くする */
.menu-item-active :deep(.v-list-item-title) {
    color: white !important;
    font-weight: 800 !important;
    /* より強調 */
}

.menu-item-active :deep(.v-icon) {
    color: white !important;
}

/* 選択中に左端に白いアクセントバーを表示（視覚的な現在地） */
.menu-item-active::before {
    content: "";
    position: absolute;
    left: 0;
    top: 20%;
    height: 60%;
    width: 5px;
    background-color: white;
    border-radius: 0 4px 4px 0;
}

/* --- ホバー時の挙動 --- */
.menu-item:not(.menu-item-active):hover {
    background-color: #e1efff !important;
    transform: translateX(5px);
    /* 右に少しスライドして「押せる」感を出す */
}
</style>