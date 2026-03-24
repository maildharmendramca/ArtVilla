import client from './client';
import type { ApiResponse } from '@/types';

export interface ApplyCouponResponse {
  discount: number;
  couponCode: string;
  message: string;
}

export async function applyCoupon(code: string): Promise<ApiResponse<ApplyCouponResponse>> {
  const { data } = await client.post('/cart/coupon', { code });
  return data;
}

export async function removeCoupon(): Promise<ApiResponse<null>> {
  const { data } = await client.delete('/cart/coupon');
  return data;
}

export async function validateCart(
  items: { productId: string; variantId: string; quantity: number }[],
): Promise<ApiResponse<{ valid: boolean; errors?: string[] }>> {
  const { data } = await client.post('/cart/validate', { items });
  return data;
}
