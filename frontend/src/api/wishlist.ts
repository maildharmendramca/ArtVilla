import client from './client';
import type { ProductListItem, ApiResponse } from '@/types';

export async function getWishlist(): Promise<ApiResponse<ProductListItem[]>> {
  const { data } = await client.get('/wishlist');
  return data;
}

export async function addToWishlist(productId: string): Promise<ApiResponse<null>> {
  const { data } = await client.post('/wishlist', { productId });
  return data;
}

export async function removeFromWishlist(productId: string): Promise<ApiResponse<null>> {
  const { data } = await client.delete(`/wishlist/${productId}`);
  return data;
}

export async function isInWishlist(productId: string): Promise<ApiResponse<{ inWishlist: boolean }>> {
  const { data } = await client.get(`/wishlist/check/${productId}`);
  return data;
}
