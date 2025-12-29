import axios from 'axios'
import { getAccessToken, getRefreshToken, setTokens, clearTokens } from '../utils/auth';

const apiClient = axios.create({
  baseURL: 'http://localhost:5204/baseballapi', // 実際のAPIのURLに変更してください
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 100000000000, // 10秒でタイムアウト
})

// リクエスト送信時にアクセストークンを付与
apiClient.interceptors.request.use((config) => {
  const token = getAccessToken();
  if (token && config.headers) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// レスポンス受信時のエラーハンドリング（401検知）
apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    // 401エラー（期限切れ）かつ、リフレッシュ試行中でない場合
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = getRefreshToken();
        
        // リフレッシュ用APIを通常のaxios（apiClientではない）で呼び出す
        // ※apiClientを使うとインターセプターが無限ループするため
        const res = await axios.post('http://localhost:5204/baseballapi/Refresh', {
          token: refreshToken
        });

        if (res.status === 200) {
          // 新しいトークンをセット
          setTokens(res.data.accessToken, res.data.refreshToken);

          // 元のリクエストを再実行
          originalRequest.headers.Authorization = `Bearer ${res.data.accessToken}`;
          return apiClient(originalRequest);
        }
      } catch (refreshError) {
        // リフレッシュ失敗時は強制ログアウト
        clearTokens();
        window.location.href = '/';
        return Promise.reject(refreshError);
      }
    }
    return Promise.reject(error);
  }
);

export default apiClient;