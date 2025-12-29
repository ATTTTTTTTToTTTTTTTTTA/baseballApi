/**
 * カスタム確認ダイアログを呼び出すユーティリティ
 * @param options ダイアログの設定（title, message, color, icon）
 * @returns Promise<boolean> OKならtrue, キャンセルならfalse
 */
export const confirmAction = (options: {
  title: string;
  message: string;
  color?: string;
  icon?: string;
}): Promise<boolean> => {
  return new Promise((resolve) => {
    const event = new CustomEvent('show-modal', {
      detail: {
        ...options,
        isConfirm: true,
        // NotificationManager側で実行されるコールバックを渡す
        onOk: () => resolve(true),
        onCancel: () => resolve(false),
      },
    });
    window.dispatchEvent(event);
  });
};

/**
 * トースト通知を表示するユーティリティ
 */
export const showToast = (message: string, color: string = 'success') => {
  window.dispatchEvent(new CustomEvent('show-toast', {
    detail: { message, color }
  }));
};