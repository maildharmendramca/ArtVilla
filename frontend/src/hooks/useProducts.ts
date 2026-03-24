import { useQuery } from '@tanstack/react-query';
import {
  getProducts,
  getProductBySlug,
  getFeaturedProducts,
  getNewArrivals,
  getBestSellers,
  getRelatedProducts,
  searchProducts,
  type ProductFilters,
} from '@/api/products';

export function useProducts(filters: ProductFilters = {}) {
  return useQuery({
    queryKey: ['products', filters],
    queryFn: () => getProducts(filters),
  });
}

export function useProduct(slug: string) {
  return useQuery({
    queryKey: ['product', slug],
    queryFn: () => getProductBySlug(slug),
    enabled: !!slug,
  });
}

export function useFeaturedProducts() {
  return useQuery({
    queryKey: ['products', 'featured'],
    queryFn: getFeaturedProducts,
  });
}

export function useNewArrivals() {
  return useQuery({
    queryKey: ['products', 'new-arrivals'],
    queryFn: getNewArrivals,
  });
}

export function useBestSellers() {
  return useQuery({
    queryKey: ['products', 'best-sellers'],
    queryFn: getBestSellers,
  });
}

export function useRelatedProducts(productId: string) {
  return useQuery({
    queryKey: ['products', 'related', productId],
    queryFn: () => getRelatedProducts(productId),
    enabled: !!productId,
  });
}

export function useProductSearch(query: string) {
  return useQuery({
    queryKey: ['products', 'search', query],
    queryFn: () => searchProducts(query),
    enabled: query.length >= 2,
  });
}
