import { useRef } from 'react';
import { useFeaturedProducts } from '@/hooks/useProducts';
import { ProductCard, ProductCardSkeleton } from '@/components/common';

export default function FeaturedProducts() {
  const scrollRef = useRef<HTMLDivElement>(null);
  const { data, isLoading } = useFeaturedProducts();
  const products = data?.data ?? [];

  const scroll = (dir: 'left' | 'right') => {
    if (scrollRef.current) {
      scrollRef.current.scrollBy({ left: dir === 'left' ? -300 : 300, behavior: 'smooth' });
    }
  };

  return (
    <section className="max-w-7xl mx-auto px-4 py-16">
      <div className="flex items-center justify-between mb-8">
        <h2 className="font-heading text-3xl text-dark">Featured Products</h2>
        <div className="flex gap-2">
          <button onClick={() => scroll('left')} className="p-2 rounded-full border border-gray-300 hover:border-copper hover:text-copper transition-colors">
            <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
          </button>
          <button onClick={() => scroll('right')} className="p-2 rounded-full border border-gray-300 hover:border-copper hover:text-copper transition-colors">
            <svg className="w-5 h-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
            </svg>
          </button>
        </div>
      </div>

      <div ref={scrollRef} className="flex gap-6 overflow-x-auto scrollbar-hide pb-4 snap-x snap-mandatory">
        {isLoading
          ? Array.from({ length: 5 }).map((_, i) => (
              <div key={i} className="min-w-[260px] snap-start">
                <ProductCardSkeleton />
              </div>
            ))
          : products.map((product) => (
              <div key={product.id} className="min-w-[260px] snap-start">
                <ProductCard product={product} />
              </div>
            ))}
      </div>
    </section>
  );
}
