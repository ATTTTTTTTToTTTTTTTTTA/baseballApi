<template>
    <div>
        <v-snackbar v-model="toast.show" :color="toast.color" :timeout="toast.timeout" location="top right"
            elevation="24">
            <div class="d-flex align-center">
                <v-icon :icon="toast.icon" class="mr-3"></v-icon>
                {{ toast.message }}
            </div>
            <template v-slot:actions>
                <v-btn variant="text" @click="toast.show = false">閉じる</v-btn>
            </template>
        </v-snackbar>

        <v-dialog v-model="modal.show" max-width="400" persistent>
            <v-card class="rounded-xl pa-4">
                <v-card-item>
                    <div class="text-center mb-4">
                        <v-icon :icon="modal.icon" size="64" :color="modal.color"></v-icon>
                    </div>
                    <v-card-title class="text-h5 text-center font-weight-bold">
                        {{ modal.title }}
                    </v-card-title>
                </v-card-item>

                <v-card-text class="text-center text-body-1 text-medium-emphasis">
                    {{ modal.message }}
                </v-card-text>

                <v-card-actions class="justify-center mt-4 px-4">
                    <v-btn v-if="modal.isConfirm" variant="outlined" color="grey-darken-1" class="rounded-lg mr-2"
                        flex-grow-1 @click="handleCancel">
                        キャンセル
                    </v-btn>

                    <v-btn :color="modal.color" variant="flat" class="rounded-lg" :class="{ 'flex-grow-1': true }"
                        @click="handleOk">
                        {{ modal.isConfirm ? '実行する' : 'OK' }}
                    </v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue'

export default defineComponent({
    name: 'NotificationManager',
    data() {
        return {
            toast: {
                show: false,
                message: '',
                color: 'success',
                icon: 'mdi-check-circle',
                timeout: 3000
            },
            modal: {
                show: false,
                title: '',
                message: '',
                color: 'primary',
                icon: 'mdi-information',
                isConfirm: false, // 確認モードかどうか
                onOk: null as Function | null, // OK押下時のコールバック
                onCancel: null as Function | null
            }
        }
    },
    mounted() {
        // トースト表示イベント
        window.addEventListener('show-toast', ((e: CustomEvent) => {
            this.toast = { ...this.toast, ...e.detail, show: true };
        }) as EventListener);

        // モーダル表示イベント
        window.addEventListener('show-modal', ((e: CustomEvent) => {
            this.modal = {
                ...this.modal,
                isConfirm: false, // デフォルトは通知モード
                onOk: null,
                ...e.detail,
                show: true
            };
        }) as EventListener);
    },
    methods: {
        handleOk() {
            if (this.modal.onOk) {
                this.modal.onOk(); // 確認モード時の実行処理
            }
            this.modal.show = false;
        },
        handleCancel() {
            if (this.modal.onCancel) this.modal.onCancel(); // ★追加
            this.modal.show = false;
        }
    },
    watch: {
        // persistent属性がない場合に備え、showがfalseになったらCancel扱いにする
        'modal.show'(val) {
            if (!val && this.modal.onCancel) {
                this.modal.onCancel();
                this.modal.onCancel = null; // 二重実行防止
            }
        }
    }
})
</script>