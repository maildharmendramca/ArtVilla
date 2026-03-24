import { Link } from 'react-router-dom';
import { useNewArrivals } from '@/hooks/useProducts';
import { ProductCard, ProductCardSkeleton } from '@/components/common';

export default function NewArrivals() {
  const { data, isLoading } = useNewArrivals();
  const products = data?.data?.slice(0, 8) ?? [];

  return (
    <section className="max-w-7xl mx-auto px-4 py-16">
      <div className="flex items-center justify-between mb-8">
        <h2 className="font-heading text-3xl text-dark">New Arrivals</h2>
        <Link to="/collections/new-arrivals" className="text-sm text-copper font-medium hover:text-gold-700 transition-colors">
          View All &rarr;
        </Link>
      </div>
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 lg:gap-6">
        {isLoading
          ? Array.from({ length: 8 }).map((_, i) => <ProductCardSkeleton key={i} />)
          : products.map((product) => <ProductCard key={product.id} product={product} />)}
      </div>
    </section>
  );
}
