import client from './client';
import type { Order, OrderDetail, ApiResponse, PaginatedResponse } from '@/types';

export interface CreateOrderPayload {
  items: { productId: string; variantId: string; quantity: number; giftWrap: boolean; giftMessage?: string }[];
  shippingAddressId: string;
  billingAddressId: string;
  paymentMethod: string;
  couponCode?: string;
  notes?: string;
}

export async function createOrder(payload: CreateOrderPayload): Promise<ApiResponse<Order>> {
  const { data } = await client.post('/orders', payload);
  return data;
}

export async function getOrders(page = 1, limit = 10): Promise<PaginatedResponse<Order>> {
  const { data } = await client.get('/orders', { params: { page, limit } });
  return data;
}

export async function getOrderById(id: string): Promise<ApiResponse<OrderDetail>> {
  const { data } = await client.get(`/orders/${id}`);
  return data;
}

export async function cancelOrder(id: string): Promise<ApiResponse<Order>> {
  const { data } = await client.patch(`/orders/${id}/cancel`);
  return data;
}

export async function initiatePayment(orderId: string): Promise<ApiResponse<{ paymentUrl: string; orderId: string }>> {
  const { data } = await client.post(`/orders/${orderId}/pay`);
  return data;
}

export async function verifyPayment(paymentId: string, signature: string): Promise<ApiResponse<Order>> {
  const { data } = await client.post('/orders/verify-payment', { paymentId, signature });
  return data;
}
