import client from './client';
import type { Product, ProductListItem, PaginatedResponse, ApiResponse } from '@/types';

export interface ProductFilters {
  categorySlug?: string;
  search?: string;
  minPrice?: number;
  maxPrice?: number;
  material?: string;
  sortBy?: 'price_asc' | 'price_desc' | 'newest' | 'rating' | 'popularity';
  page?: number;
  limit?: number;
}

export async function getProducts(filters: ProductFilters = {}): Promise<PaginatedResponse<ProductListItem>> {
  const { data } = await client.get('/products', { params: filters });
  return data;
}

export async function getProductBySlug(slug: string): Promise<ApiResponse<Product>> {
  const { data } = await client.get(`/products/${slug}`);
  return data;
}

export async function getFeaturedProducts(): Promise<ApiResponse<ProductListItem[]>> {
  const { data } = await client.get('/products/featured');
  return data;
}

export async function getNewArrivals(): Promise<ApiResponse<ProductListItem[]>> {
  const { data } = await client.get('/products/new-arrivals');
  return data;
}

export async function getBestSellers(): Promise<ApiResponse<ProductListItem[]>> {
  const { data } = await client.get('/products/best-sellers');
  return data;
}

export async function getRelatedProducts(productId: string): Promise<ApiResponse<ProductListItem[]>> {
  const { data } = await client.get(`/products/${productId}/related`);
  return data;
}

export async function searchProducts(query: string): Promise<ApiResponse<ProductListItem[]>> {
  const { data } = await client.get('/products/search', { params: { q: query } });
  return data;
}
