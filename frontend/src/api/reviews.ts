import client from './client';
import type { Review, ApiResponse, PaginatedResponse } from '@/types';

export interface CreateReviewPayload {
  productId: string;
  rating: number;
  title?: string;
  comment: string;
  images?: string[];
}

export async function getProductReviews(
  productId: string,
  page = 1,
  limit = 10,
): Promise<PaginatedResponse<Review>> {
  const { data } = await client.get(`/products/${productId}/reviews`, { params: { page, limit } });
  return data;
}

export async function createReview(payload: CreateReviewPayload): Promise<ApiResponse<Review>> {
  const { data } = await client.post('/reviews', payload);
  return data;
}

export async function deleteReview(id: string): Promise<ApiResponse<null>> {
  const { data } = await client.delete(`/reviews/${id}`);
  return data;
}
