import client from './client';
import type { Address, ApiResponse } from '@/types';

export async function getAddresses(): Promise<ApiResponse<Address[]>> {
  const { data } = await client.get('/addresses');
  return data;
}

export async function getAddressById(id: string): Promise<ApiResponse<Address>> {
  const { data } = await client.get(`/addresses/${id}`);
  return data;
}

export async function createAddress(payload: Omit<Address, 'id'>): Promise<ApiResponse<Address>> {
  const { data } = await client.post('/addresses', payload);
  return data;
}

export async function updateAddress(id: string, payload: Partial<Address>): Promise<ApiResponse<Address>> {
  const { data } = await client.put(`/addresses/${id}`, payload);
  return data;
}

export async function deleteAddress(id: string): Promise<ApiResponse<null>> {
  const { data } = await client.delete(`/addresses/${id}`);
  return data;
}

export async function setDefaultAddress(id: string): Promise<ApiResponse<Address>> {
  const { data } = await client.patch(`/addresses/${id}/default`);
  return data;
}
