import client from './client';
import type {
  Product,
  Order,
  OrderDetail,
  User,
  Category,
  DashboardStats,
  ApiResponse,
  PaginatedResponse,
} from '@/types';

// Dashboard
export async function getDashboardStats(): Promise<ApiResponse<DashboardStats>> {
  const { data } = await client.get('/admin/dashboard');
  return data;
}

// Products
export async function getAdminProducts(
  page = 1,
  limit = 20,
  search?: string,
): Promise<PaginatedResponse<Product>> {
  const { data } = await client.get('/admin/products', { params: { page, limit, search } });
  return data;
}

export async function getAdminProduct(id: string): Promise<ApiResponse<any>> {
  const { data } = await client.get(`/admin/products/${id}`);
  return data;
}

export async function createProduct(payload: Partial<Product>): Promise<ApiResponse<Product>> {
  const { data } = await client.post('/admin/products', payload);
  return data;
}

export async function updateProduct(id: string, payload: Partial<Product>): Promise<ApiResponse<Product>> {
  const { data } = await client.put(`/admin/products/${id}`, payload);
  return data;
}

export async function deleteProduct(id: string): Promise<ApiResponse<null>> {
  const { data } = await client.delete(`/admin/products/${id}`);
  return data;
}

// Orders
export async function getAdminOrders(
  page = 1,
  limit = 20,
  status?: string,
): Promise<PaginatedResponse<Order>> {
  const { data } = await client.get('/admin/orders', { params: { page, limit, status } });
  return data;
}

export async function updateOrderStatus(
  id: string,
  status: string,
  note?: string,
): Promise<ApiResponse<OrderDetail>> {
  const { data } = await client.patch(`/admin/orders/${id}/status`, { status, note });
  return data;
}

// Categories
export async function createCategory(payload: Partial<Category>): Promise<ApiResponse<Category>> {
  const { data } = await client.post('/admin/categories', payload);
  return data;
}

export async function updateCategory(id: string, payload: Partial<Category>): Promise<ApiResponse<Category>> {
  const { data } = await client.put(`/admin/categories/${id}`, payload);
  return data;
}

export async function deleteCategory(id: string): Promise<ApiResponse<null>> {
  const { data } = await client.delete(`/admin/categories/${id}`);
  return data;
}

// Upload
export async function uploadImage(file: File): Promise<ApiResponse<{ url: string }>> {
  const formData = new FormData();
  formData.append('file', file);
  const { data } = await client.post('/upload', formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return data;
}

// Customers
export async function getCustomers(
  page = 1,
  limit = 20,
  search?: string,
): Promise<PaginatedResponse<User>> {
  const { data } = await client.get('/admin/customers', { params: { page, limit, search } });
  return data;
}

export async function getCustomerById(id: string): Promise<ApiResponse<User>> {
  const { data } = await client.get(`/admin/customers/${id}`);
  return data;
}

export async function toggleCustomerActive(id: string): Promise<ApiResponse<string>> {
  const { data } = await client.patch(`/admin/customers/${id}/toggle-active`);
  return data;
}
