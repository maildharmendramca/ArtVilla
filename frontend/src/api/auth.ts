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

export interface AuthResponseRaw {
  token: string;
  refreshToken: string;
  expiration: string;
  user: {
    id: string;
    fullName: string;
    email: string;
    phone?: string;
    roles: string[];
  };
}

export interface AuthResponse {
  user: User;
  token: string;
  refreshToken: string;
}

function mapAuthResponse(raw: ApiResponse<AuthResponseRaw>): ApiResponse<AuthResponse> {
  const { user, token, refreshToken } = raw.data;
  return {
    ...raw,
    data: {
      token,
      refreshToken,
      user: {
        id: user.id,
        name: user.fullName,
        email: user.email,
        phone: user.phone,
        role: user.roles.map((r) => r.toLowerCase()).includes('admin') ? 'admin' : 'customer',
        createdAt: '',
      },
    },
  };
}

export async function login(payload: LoginPayload): Promise<ApiResponse<AuthResponse>> {
  const { data } = await client.post('/auth/login', payload);
  return mapAuthResponse(data);
}

export async function register(payload: RegisterPayload): Promise<ApiResponse<AuthResponse>> {
  const { data } = await client.post('/auth/register', payload);
  return mapAuthResponse(data);
}

export async function logout(): Promise<ApiResponse<null>> {
  const { data } = await client.post('/auth/logout');
  return data;
}

export async function getProfile(): Promise<ApiResponse<User>> {
  const { data } = await client.get('/auth/me');
  const raw = data.data;
  return {
    ...data,
    data: {
      id: raw.id,
      name: raw.fullName ?? raw.name,
      email: raw.email,
      phone: raw.phone,
      role: (raw.roles ?? []).map((r: string) => r.toLowerCase()).includes('admin') ? 'admin' : 'customer',
      createdAt: raw.createdAt ?? '',
    },
  };
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
