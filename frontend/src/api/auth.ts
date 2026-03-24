import client from './client';
import type { User, ApiResponse } from '@/types';

export interface LoginPayload {
  email: string;
  password: string;
}

export interface RegisterPayload {
  name: string;
  email: string;
  password: string;
  phone?: string;
}

export interface AuthResponse {
  user: User;
  token: string;
}

export async function login(payload: LoginPayload): Promise<ApiResponse<AuthResponse>> {
  const { data } = await client.post('/auth/login', payload);
  return data;
}

export async function register(payload: RegisterPayload): Promise<ApiResponse<AuthResponse>> {
  const { data } = await client.post('/auth/register', payload);
  return data;
}

export async function logout(): Promise<ApiResponse<null>> {
  const { data } = await client.post('/auth/logout');
  return data;
}

export async function getProfile(): Promise<ApiResponse<User>> {
  const { data } = await client.get('/auth/me');
  return data;
}

export async function updateProfile(payload: Partial<User>): Promise<ApiResponse<User>> {
  const { data } = await client.patch('/auth/me', payload);
  return data;
}

export async function changePassword(oldPassword: string, newPassword: string): Promise<ApiResponse<null>> {
  const { data } = await client.post('/auth/change-password', { oldPassword, newPassword });
  return data;
}

export async function forgotPassword(email: string): Promise<ApiResponse<null>> {
  const { data } = await client.post('/auth/forgot-password', { email });
  return data;
}

export async function resetPassword(token: string, password: string): Promise<ApiResponse<null>> {
  const { data } = await client.post('/auth/reset-password', { token, password });
  return data;
}
