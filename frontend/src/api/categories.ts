import client from './client';
import type { Category, CategorySummary, ApiResponse } from '@/types';

export async function getCategories(): Promise<ApiResponse<Category[]>> {
  const { data } = await client.get('/categories');
  return data;
}

export async function getCategoryBySlug(slug: string): Promise<ApiResponse<Category>> {
  const { data } = await client.get(`/categories/${slug}`);
  return data;
}

export async function getCategorySummaries(): Promise<ApiResponse<CategorySummary[]>> {
  const { data } = await client.get('/categories/summaries');
  return data;
}

export async function getCategoryTree(): Promise<ApiResponse<Category[]>> {
  const { data } = await client.get('/categories/tree');
  return data;
}
