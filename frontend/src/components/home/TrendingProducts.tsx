import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useNewArrivals, useBestSellers } from '@/hooks/useProducts';
import { ProductCard, ProductCardSkeleton } from '@/components/common';

const tabs = [
  { key: 'new-arrivals', label: 'New Arrivals', link: '/collections/new-arrivals' },
  { key: 'best-sellers', label: 'Best Sellers', link: '/collections/best-sellers' },
] as const;

type TabKey = (typeof tabs)[number]['key'];

export default function TrendingProducts() {
  const [activeTab, setActiveTab] = useState<TabKey>('new-arrivals');

  const { data: newData, isLoading: newLoading } = useNewArrivals();
  const { data: bestData, isLoading: bestLoading } = useBestSellers();

  const isLoading = activeTab === 'new-arrivals' ? newLoading : bestLoading;
  const products =
    activeTab === 'new-arrivals'
      ? (newData?.data?.slice(0, 8) ?? [])
      : (bestData?.data?.slice(0, 8) ?? []);

  const activeTabConfig = tabs.find((t) => t.key === activeTab)!;

  return (
    <section className="max-w-7xl mx-auto px-4 py-16">
      <div className="flex items-center justify-between mb-8">
        {/* Tabs */}
        <div className="flex gap-1 rounded-lg bg-gray-100 p-1">
          {tabs.map((tab) => (
            <button
              key={tab.key}
              onClick={() => setActiveTab(tab.key)}
              className={`px-5 py-2 rounded-md text-sm font-medium transition-all duration-200 ${
                activeTab === tab.key
                  ? 'bg-white text-copper shadow-sm'
                  : 'text-gray-500 hover:text-dark'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </div>

        <Link
          to={activeTabConfig.link}
          className="text-sm text-copper font-medium hover:text-gold-700 transition-colors"
        >
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
