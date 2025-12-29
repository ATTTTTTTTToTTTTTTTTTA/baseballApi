<template>
    <v-dialog v-model="internalShow" max-width="500px" persistent>
        <v-card class="rounded-lg">
            <v-card-title class="bg-blue-darken-2 text-white pa-4">
                <v-icon start>{{ isTeam ? 'mdi-shield-home' : 'mdi-account-edit' }}</v-icon>
                {{ isTeam ? 'チーム情報の編集' : '選手情報の編集' }}
            </v-card-title>

            <v-card-text class="pa-4">
                <v-form ref="form">
                    <v-container>
                        <v-row dense>
                            <template v-if="!isTeam">
                                <v-col cols="12">
                                    <v-text-field v-model="localItem.name" label="選手名" variant="outlined"
                                        density="comfortable"></v-text-field>
                                </v-col>
                                <v-col cols="6">
                                    <v-text-field v-model="localItem.number" label="背番号" type="number"
                                        variant="outlined" density="comfortable"></v-text-field>
                                </v-col>
                                <v-col cols="6">
                                    <v-select v-model="localItem.position" :items="positions" label="ポジション"
                                        variant="outlined" density="comfortable"></v-select>
                                </v-col>
                            </template>

                            <template v-else>
                                <v-col cols="12">
                                    <v-text-field v-model="localItem.name" label="チーム名" variant="outlined"
                                        density="comfortable"></v-text-field>
                                </v-col>
                                <v-col cols="12">
                                    <v-text-field v-model="localItem.location" label="活動場所" variant="outlined"
                                        density="comfortable"></v-text-field>
                                </v-col>
                            </template>
                        </v-row>
                    </v-container>
                </v-form>
            </v-card-text>

            <v-card-actions class="pa-4 pt-0">
                <v-spacer></v-spacer>
                <v-btn variant="text" @click="close">キャンセル</v-btn>
                <v-btn color="primary" variant="flat" class="px-6" @click="save">更新する</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
</template>

<script setup lang="ts">
import { ref, watch, computed } from 'vue';

const props = defineProps<{
    show: boolean;
    type: 'player' | 'team';
    item: any;
}>();

const emit = defineEmits(['update:show', 'save']);

// Propsを直接変更できないため、ローカルな変数にコピーする
const internalShow = computed({
    get: () => props.show,
    set: (val) => emit('update:show', val)
});

const localItem = ref({ ...props.item });
const isTeam = computed(() => props.type === 'team');
const positions = ['投手', '捕手', '一塁手', '二塁手', '三塁手', '遊撃手', '左翼手', '中堅手', '右翼手'];

// props.itemが更新されたらローカルに反映
watch(() => props.item, (newVal) => {
    localItem.value = { ...newVal };
}, { deep: true });

const close = () => {
    internalShow.value = false;
};

const save = () => {
    emit('save', { ...localItem.value });
    close();
};
</script>