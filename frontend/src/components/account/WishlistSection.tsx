import { useWishlist, useRemoveFromWishlist } from '@/hooks/useWishlist';
import { ProductCard, Spinner, EmptyState } from '@/components/common';
import { Link } from 'react-router-dom';

export default function WishlistSection() {
  const { data, isLoading } = useWishlist();
  const removeFromWishlist = useRemoveFromWishlist();
  const products = data?.data ?? [];

  if (isLoading) return <Spinner className="py-16" />;

  if (products.length === 0) {
    return (
      <EmptyState
        title="Your wishlist is empty"
        description="Save items you love to find them later."
        action={<Link to="/" className="text-copper font-medium hover:text-gold-700">Explore Products</Link>}
      />
    );
  }

  return (
    <div>
      <h2 className="font-heading text-2xl text-dark mb-6">My Wishlist</h2>
      <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
        {products.map((product) => (
          <div key={product.id} className="relative">
            <ProductCard product={product} hideWishlist />
            <button
              onClick={(e) => { e.preventDefault(); e.stopPropagation(); removeFromWishlist.mutate(String(product.id)); }}
              className="absolute top-3 right-3 z-10 bg-white rounded-full p-1.5 shadow-md text-red-500 hover:text-red-700"
            >
              <svg className="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path fillRule="evenodd" d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z" clipRule="evenodd" />
              </svg>
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}
