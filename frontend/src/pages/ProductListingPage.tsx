import { useState } from 'react';
import { useParams, useSearchParams } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import { useProducts } from '@/hooks/useProducts';
import { useCategory } from '@/hooks/useCategories';
import { ProductCard, ProductCardSkeleton, Breadcrumb, EmptyState } from '@/components/common';
import type { ProductFilters } from '@/api/products';

const sortOptions = [
  { label: 'Newest', value: 'newest' },
  { label: 'Price: Low to High', value: 'price_asc' },
  { label: 'Price: High to Low', value: 'price_desc' },
  { label: 'Top Rated', value: 'rating' },
  { label: 'Popularity', value: 'popularity' },
] as const;

export default function ProductListingPage() {
  const { slug } = useParams<{ slug: string }>();
  const [searchParams, setSearchParams] = useSearchParams();
  const [sortBy, setSortBy] = useState<ProductFilters['sortBy']>(
    (searchParams.get('sort') as ProductFilters['sortBy']) || 'newest',
  );
  const page = parseInt(searchParams.get('page') || '1', 10);

  const { data: categoryData } = useCategory(slug ?? '');
  const category = categoryData?.data;

  const { data, isLoading } = useProducts({
    categorySlug: slug,
    sortBy,
    page,
    limit: 12,
  });

  const products = data?.data ?? [];
  const pagination = data?.pagination;

  const handleSort = (value: string) => {
    setSortBy(value as ProductFilters['sortBy']);
    searchParams.set('sort', value);
    searchParams.set('page', '1');
    setSearchParams(searchParams);
  };

  const handlePageChange = (newPage: number) => {
    searchParams.set('page', String(newPage));
    setSearchParams(searchParams);
  };

  return (
    <>
      <Helmet>
        <title>{category?.name ?? 'Products'} | Indian Art Villa</title>
      </Helmet>
      <div className="max-w-7xl mx-auto px-4">
        <Breadcrumb items={[{ label: category?.name ?? 'Products' }]} />

        <div className="mb-8">
          <h1 className="font-heading text-3xl text-dark">{category?.name ?? 'All Products'}</h1>
          {category?.description && (
            <p className="text-muted mt-2 max-w-2xl">{category.description}</p>
          )}
        </div>

        {/* Sort bar */}
        <div className="flex items-center justify-between mb-6">
          <p className="text-sm text-muted">
            {pagination ? `${pagination.totalItems} products` : ''}
          </p>
          <select
            value={sortBy}
            onChange={(e) => handleSort(e.target.value)}
            className="text-sm border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-copper"
          >
            {sortOptions.map((opt) => (
              <option key={opt.value} value={opt.value}>{opt.label}</option>
            ))}
          </select>
        </div>

        {/* Product grid */}
        {isLoading ? (
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 lg:gap-6">
            {Array.from({ length: 12 }).map((_, i) => <ProductCardSkeleton key={i} />)}
          </div>
        ) : products.length === 0 ? (
          <EmptyState title="No products found" description="Try adjusting your filters or browse other collections." />
        ) : (
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 lg:gap-6">
            {products.map((product) => <ProductCard key={product.id} product={product} />)}
          </div>
        )}

        {/* Pagination */}
        {pagination && pagination.totalPages > 1 && (
          <div className="flex justify-center gap-2 py-12">
            <button
              disabled={!pagination.hasPrevPage}
              onClick={() => handlePageChange(page - 1)}
              className="px-4 py-2 text-sm rounded-lg border border-gray-300 hover:border-copper disabled:opacity-40"
            >
              Previous
            </button>
            {Array.from({ length: pagination.totalPages }, (_, i) => i + 1).map((p) => (
              <button
                key={p}
                onClick={() => handlePageChange(p)}
                className={`px-4 py-2 text-sm rounded-lg border ${
                  p === page ? 'bg-copper text-white border-copper' : 'border-gray-300 hover:border-copper'
                }`}
              >
                {p}
              </button>
            ))}
            <button
              disabled={!pagination.hasNextPage}
              onClick={() => handlePageChange(page + 1)}
              className="px-4 py-2 text-sm rounded-lg border border-gray-300 hover:border-copper disabled:opacity-40"
            >
              Next
            </button>
          </div>
        )}
      </div>
    </>
  );
}
