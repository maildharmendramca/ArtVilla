import { useQuery } from '@tanstack/react-query';
import { getCategories, getCategoryBySlug, getCategorySummaries, getCategoryTree } from '@/api/categories';

export function useCategories() {
  return useQuery({
    queryKey: ['categories'],
    queryFn: getCategories,
    staleTime: 5 * 60 * 1000,
  });
}

export function useCategory(slug: string) {
  return useQuery({
    queryKey: ['category', slug],
    queryFn: () => getCategoryBySlug(slug),
    enabled: !!slug,
  });
}

export function useCategorySummaries() {
  return useQuery({
    queryKey: ['categories', 'summaries'],
    queryFn: getCategorySummaries,
    staleTime: 5 * 60 * 1000,
  });
}

export function useCategoryTree() {
  return useQuery({
    queryKey: ['categories', 'tree'],
    queryFn: getCategoryTree,
    staleTime: 5 * 60 * 1000,
  });
}
