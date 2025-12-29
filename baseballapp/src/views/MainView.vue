<script>
import { BaseService } from '../api/baseService';
export default {
    data() {
        return {
            items: ['Vue.jsの学習', 'Options APIでの開発', 'Routerの移行']
        }
    },
    async created() {
        console.log('メイン画面へようこそ！');


        // TODO: 後で消す
        try {
            console.log("AAAAAAAAAAAAAAA")
            const api = new BaseService("player")
            const res = await api.getAll();
            console.log(res);
        }
        catch (e) {
            console.log("失敗")
            console.log(e)
        }
    }
}
</script>

<template>
    <div class="main-container">
        <h1>メイン画面</h1>
        <ul>
            <li v-for="item in items" :key="item">{{ item }}</li>
        </ul>
        <button @click="$router.push('/')">ログアウト</button>
    </div>
</template>