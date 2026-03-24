import { Link } from 'react-router-dom';
import type { ProductListItem } from '@/types';
import { formatCurrency, getDiscountPercentage } from '@/utils/format';
import { useAddToWishlist, useRemoveFromWishlist } from '@/hooks/useWishlist';
import { useAuthStore } from '@/store/authStore';
import { isInWishlist as checkWishlist } from '@/api/wishlist';
import { useQuery } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import Rating from './Rating';

interface ProductCardProps {
  product: ProductListItem;
  className?: string;
  hideWishlist?: boolean;
}

export default function ProductCard({ product, className = '', hideWishlist = false }: ProductCardProps) {
  const { isAuthenticated } = useAuthStore();
  const addToWishlist = useAddToWishlist();
  const removeFromWishlist = useRemoveFromWishlist();
  const { data: wishlistCheck } = useQuery({
    queryKey: ['wishlist-check', String(product.id)],
    queryFn: () => checkWishlist(String(product.id)),
    enabled: isAuthenticated && !hideWishlist,
  });
  const isWishlisted = wishlistCheck?.data?.inWishlist ?? false;

  const handleWishlistToggle = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (!isAuthenticated) {
      toast.error('Please sign in to use wishlist');
      return;
    }
    if (isWishlisted) {
      removeFromWishlist.mutate(String(product.id));
    } else {
      addToWishlist.mutate(String(product.id));
    }
  };

  const discount = product.compareAtPrice
    ? getDiscountPercentage(product.basePrice, product.compareAtPrice)
    : 0;

  return (
    <Link
      to={`/products/${product.slug}`}
      className={`group block bg-white rounded-xl overflow-hidden shadow-sm hover:shadow-lg transition-shadow ${className}`}
    >
      <div className="relative aspect-square overflow-hidden bg-gold-50">
        <img
          src={product.primaryImage || '/images/placeholder-400.jpg'}
          alt={product.name}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
        />
        {discount > 0 && (
          <span className="absolute top-3 left-3 bg-red-600 text-white text-xs font-bold px-2 py-1 rounded">
            -{discount}%
          </span>
        )}
        {!hideWishlist && (
          <button
            onClick={handleWishlistToggle}
            className={`absolute top-3 right-3 p-1.5 rounded-full shadow-md transition-colors ${
              isWishlisted ? 'bg-red-50 text-red-500' : 'bg-white text-muted hover:text-copper'
            }`}
          >
            <svg className="w-4 h-4" fill={isWishlisted ? 'currentColor' : 'none'} viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
            </svg>
          </button>
        )}
        {product.isNew && !hideWishlist && (
          <span className="absolute top-3 right-12 bg-copper text-white text-xs font-bold px-2 py-1 rounded">
            NEW
          </span>
        )}
        {product.isNew && hideWishlist && (
          <span className="absolute top-3 right-3 bg-copper text-white text-xs font-bold px-2 py-1 rounded">
            NEW
          </span>
        )}
      </div>
      <div className="p-4">
        <p className="text-xs text-muted uppercase tracking-wider mb-1">{product.categoryName}</p>
        <h3 className="text-sm font-medium text-dark line-clamp-2 mb-2 group-hover:text-copper transition-colors">
          {product.name}
        </h3>
        <div className="flex items-center gap-2 mb-2">
          <Rating value={product.averageRating} />
          <span className="text-xs text-muted">({product.reviewCount})</span>
        </div>
        <div className="flex items-center gap-2">
          <span className="text-lg font-semibold text-dark">{formatCurrency(product.basePrice)}</span>
          {product.compareAtPrice && (
            <span className="text-sm text-muted line-through">{formatCurrency(product.compareAtPrice)}</span>
          )}
        </div>
      </div>
    </Link>
  );
}
