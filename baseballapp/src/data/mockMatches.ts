// src/data/mockMatches.ts

export interface MatchRecord {
  id: number;
  date: string; // YYYY-MM-DD
  opponent: string;
  ourScore: number;
  opponentScore: number;
  result: 'Win' | 'Loss' | 'Draw';
  MVP?: string;
}

export const mockMatches: MatchRecord[] = [
  { id: 1, date: '2023-10-26', opponent: 'ライオンズ', ourScore: 5, opponentScore: 3, result: 'Win', MVP: '田中' },
  { id: 2, date: '2023-10-22', opponent: 'イーグルス', ourScore: 2, opponentScore: 6, result: 'Loss' },
  { id: 3, date: '2023-10-19', opponent: 'タイガース', ourScore: 8, opponentScore: 7, result: 'Win', MVP: '山田' },
  { id: 4, date: '2023-10-15', opponent: 'ドラゴンズ', ourScore: 4, opponentScore: 4, result: 'Draw' },
  { id: 5, date: '2023-10-11', opponent: 'ジャガーズ', ourScore: 1, opponentScore: 3, result: 'Loss' },
  { id: 6, date: '2023-10-08', opponent: 'フェニックス', ourScore: 7, opponentScore: 0, result: 'Win', MVP: '鈴木' },
  { id: 7, date: '2023-10-05', opponent: 'コブラズ', ourScore: 6, opponentScore: 5, result: 'Win' },
  { id: 8, date: '2023-10-01', opponent: 'パンサーズ', ourScore: 3, opponentScore: 9, result: 'Loss' },
  { id: 9, date: '2023-09-28', opponent: 'ウルブス', ourScore: 5, opponentScore: 5, result: 'Draw' },
  { id: 10, date: '2023-09-24', opponent: 'ベアーズ', ourScore: 10, opponentScore: 2, result: 'Win', MVP: '佐藤' },
];