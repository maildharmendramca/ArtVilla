import { useSearchParams } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import { useProductSearch } from '@/hooks/useProducts';
import { ProductCard, ProductCardSkeleton, EmptyState, Breadcrumb } from '@/components/common';

export default function SearchPage() {
  const [searchParams] = useSearchParams();
  const query = searchParams.get('q') ?? '';
  const { data, isLoading } = useProductSearch(query);
  const products = data?.data ?? [];

  return (
    <>
      <Helmet><title>Search: {query} | Indian Art Villa</title></Helmet>
      <div className="max-w-7xl mx-auto px-4 pb-16">
        <Breadcrumb items={[{ label: 'Search' }]} />
        <h1 className="font-heading text-3xl text-dark mb-2">Search Results</h1>
        <p className="text-muted mb-8">
          {isLoading ? 'Searching...' : `${products.length} results for "${query}"`}
        </p>
        {isLoading ? (
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 lg:gap-6">
            {Array.from({ length: 8 }).map((_, i) => <ProductCardSkeleton key={i} />)}
          </div>
        ) : products.length === 0 ? (
          <EmptyState
            title="No results found"
            description={`We couldn't find any products matching "${query}". Try different keywords.`}
          />
        ) : (
          <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 lg:gap-6">
            {products.map((product) => <ProductCard key={product.id} product={product} />)}
          </div>
        )}
      </div>
    </>
  );
}
