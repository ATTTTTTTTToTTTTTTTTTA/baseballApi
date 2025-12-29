import type { AxiosInstance } from 'axios'
import apiClient from './client'

export class BaseService {
  private client: AxiosInstance
  private endpoint: string

  constructor(endpoint: string) {
    this.client = apiClient
    this.endpoint = endpoint
  }

  // GET ALL
  async getAll<T>() {
    const response = await this.client.get<T[]>(`${this.endpoint}`)
    return response.data
  }

  // GET (ID指定)
  async get<T>(id: string | number) {
    const response = await this.client.get<T>(`${this.endpoint}/${id}`)
    return response.data
  }

  // POST (新規作成)
  async post<T>(data: any) {
    // ログイン処理などで使う想定
    const response = await this.client.post<T>(`${this.endpoint}`, data)
    return response.data
  }

  // PUT (更新)
  async put<T>(id: string | number, data: any) {
    const response = await this.client.put<T>(`${this.endpoint}/${id}`, data)
    return response.data
  }

  // DELETE
  async delete(id: string | number) {
    const response = await this.client.delete(`${this.endpoint}/${id}`)
    return response.data
  }
}