<script lang="ts">
import { defineComponent } from 'vue'
// @ts-ignore
import baseballLogo from '@/assets/baseballapp.png'
import { BaseService } from '../api/baseService';
import { clearTokens, setTokens } from '../utils/auth';

export default defineComponent({
    name: 'LoginView',
    data() {
        return {
            userId: '',
            password: '',
            visible: false,
            loading: false,
            logoSrc: baseballLogo,
            // エラー表示用
            errorSnackbar: false,
            errorMessage: ''
        }
    },
    async created() {
        // トークン情報をクリア
        clearTokens();

    },
    methods: {
        async handleLogin() {
            // バリデーション
            if (!this.userId || !this.password) return

            this.loading = true

            try {
                // BaseServiceを使用して /login にPOST
                // 第二引数にエンドポイント名 "login" を指定
                const api = new BaseService("login")

                // ログイン情報を送信
                const response = await api.post<any>({
                    LoginId: this.userId,
                    Password: this.password
                })

                // 成功時の処理（APIのレスポンス形式に合わせて調整してください）
                if (response) {
                    console.log("Login Success:", response)
                    setTokens(response.accessToken, response.refreshToken);
                    // ✅ 成功時：トーストを呼び出す
                    window.dispatchEvent(new CustomEvent('show-toast', {
                        detail: {
                            message: 'ログインに成功しました！',
                            color: 'success',
                            icon: 'mdi-check-circle'
                        }
                    }));

                    this.$router.push('/main')
                }
            } catch (e: any) {
                console.error("Login Error:", e)
                // ❌ 失敗時：モーダルを呼び出す
                window.dispatchEvent(new CustomEvent('show-modal', {
                    detail: {
                        title: 'ログイン失敗',
                        message: 'ユーザーIDまたはパスワードが正しくありません。入力内容を確認して再度お試しください。',
                        color: 'error',
                        icon: 'mdi-alert-circle-outline'
                    }
                }));
            } finally {
                this.loading = false
            }
        }
    }
})
</script>

<template>
    <v-container fluid class="pa-0 fill-height bg-grey-lighten-4">
        <v-snackbar v-model="errorSnackbar" color="error" location="top" timeout="3000">
            {{ errorMessage }}
            <template v-slot:actions>
                <v-btn variant="text" @click="errorSnackbar = false">閉じる</v-btn>
            </template>
        </v-snackbar>

        <v-row no-gutters class="fill-height">
            <v-col cols="12" md="6" lg="7"
                class="hidden-sm-and-down d-flex flex-column align-center justify-center bg-indigo-darken-4 custom-gradient">
            </v-col>

            <v-col cols="12" md="6" lg="5" class="d-flex align-center justify-center bg-white">
                <v-card flat width="100%" max-width="440" class="pa-8 pa-md-12">
                    <v-form @submit.prevent="handleLogin">
                        <v-text-field v-model="userId" label="ユーザーID" prepend-inner-icon="mdi-account"
                            variant="outlined" color="primary" density="comfortable" class="mb-4"
                            :disabled="loading"></v-text-field>

                        <v-text-field v-model="password" :type="visible ? 'text' : 'password'" label="パスワード"
                            prepend-inner-icon="mdi-lock" :append-inner-icon="visible ? 'mdi-eye-off' : 'mdi-eye'"
                            variant="outlined" color="primary" density="comfortable" class="mb-6"
                            @click:append-inner="visible = !visible" :disabled="loading"></v-text-field>

                        <v-btn block size="x-large" color="indigo-darken-4" elevation="6" height="56"
                            class="text-none font-weight-bold" :loading="loading" type="submit">
                            ログイン
                        </v-btn>
                    </v-form>

                    <footer class="mt-12 text-center">
                        <v-btn variant="text" size="small" color="grey-darken-1" class="text-none">
                            パスワードをお忘れですか？
                        </v-btn>
                    </footer>
                </v-card>
            </v-col>
        </v-row>
    </v-container>
</template>

<style scoped>
/* 画面いっぱいにする設定 */
.fill-height {
    height: 100vh !important;
    min-height: 100vh;
}

/* 左側の背景グラデーション */
.custom-gradient {
    background: linear-gradient(135deg, #1a237e 0%, #303f9f 100%) !important;
}

/* ロゴの浮遊感とシャドウ */
/* .logo-shadow {
  filter: drop-shadow(0 20px 30px rgba(0,0,0,0.4));
  animation: float 4s infinite ease-in-out;
} */

@keyframes float {

    0%,
    100% {
        transform: translateY(0);
    }

    50% {
        transform: translateY(-15px);
    }
}

/* モバイルではスクロールを許可 */
@media (max-width: 960px) {
    .fill-height {
        height: auto !important;
        min-height: 100vh;
    }
}
</style>