import { useState } from 'react';
import { useParams } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import toast from 'react-hot-toast';
import { useProduct, useRelatedProducts } from '@/hooks/useProducts';
import { useProductReviews } from '@/hooks/useReviews';
import { useCartStore } from '@/store/cartStore';
import { useAddToWishlist, useRemoveFromWishlist } from '@/hooks/useWishlist';
import { useAuthStore } from '@/store/authStore';
import { isInWishlist as checkWishlist } from '@/api/wishlist';
import { useQuery } from '@tanstack/react-query';
import {
  Breadcrumb,
  Rating,
  QuantityStepper,
  Button,
  ProductCard,
  Spinner,
} from '@/components/common';
import { formatCurrency, getDiscountPercentage } from '@/utils/format';

export default function ProductDetailPage() {
  const { slug } = useParams<{ slug: string }>();
  const { data, isLoading } = useProduct(slug ?? '');
  const product = data?.data;

  const [selectedVariant, setSelectedVariant] = useState(0);
  const [selectedImage, setSelectedImage] = useState(0);
  const [quantity, setQuantity] = useState(1);
  const addItem = useCartStore((s) => s.addItem);

  const { isAuthenticated } = useAuthStore();
  const addToWishlist = useAddToWishlist();
  const removeFromWishlistMutation = useRemoveFromWishlist();
  const { data: wishlistCheck } = useQuery({
    queryKey: ['wishlist-check', product?.id],
    queryFn: () => checkWishlist(String(product!.id)),
    enabled: !!product && isAuthenticated,
  });
  const isWishlisted = wishlistCheck?.data?.inWishlist ?? false;

  const { data: relatedData } = useRelatedProducts(product?.id ?? '');
  const { data: reviewsData } = useProductReviews(product?.id ?? '');
  const relatedProducts = relatedData?.data ?? [];
  const reviews = reviewsData?.data ?? [];

  if (isLoading) return <Spinner className="py-32" size="lg" />;
  if (!product) return <div className="py-32 text-center text-muted">Product not found</div>;

  // Fallback to a synthetic variant when the product has none (legacy data)
  const variant = product.variants[selectedVariant] ?? (product.variants.length === 0
    ? { id: `${product.id}-default`, name: 'Default', price: product.basePrice, compareAtPrice: product.compareAtPrice, stock: product.stockQuantity }
    : undefined);
  const discount = variant?.compareAtPrice
    ? getDiscountPercentage(variant.price, variant.compareAtPrice)
    : 0;

  const handleAddToCart = () => {
    if (!variant) return;
    addItem({
      productId: product.id,
      variantId: variant.id,
      name: product.name,
      variantName: variant.name,
      image: product.images[0]?.url,
      price: variant.price,
      quantity,
      giftWrap: false,
      slug: product.slug,
    });
    toast.success('Added to cart!');
  };

  const handleWishlistToggle = () => {
    if (!isAuthenticated) {
      toast.error('Please sign in to use wishlist');
      return;
    }
    if (isWishlisted) {
      removeFromWishlistMutation.mutate(String(product!.id), {
        onSuccess: () => toast.success('Removed from wishlist'),
      });
    } else {
      addToWishlist.mutate(String(product!.id), {
        onSuccess: () => toast.success('Added to wishlist!'),
      });
    }
  };

  return (
    <>
      <Helmet>
        <title>{product.name} | Indian Art Villa</title>
        <meta name="description" content={product.shortDescription || product.description.slice(0, 160)} />
      </Helmet>
      <div className="max-w-7xl mx-auto px-4">
        <Breadcrumb
          items={[
            { label: product.categoryName, href: `/collections/${product.categoryId}` },
            { label: product.name },
          ]}
        />

        <div className="grid lg:grid-cols-2 gap-10 pb-16">
          {/* Images */}
          <div>
            <div className="aspect-square rounded-xl overflow-hidden bg-gold-50 mb-4">
              <img
                src={product.images[selectedImage]?.url || '/images/placeholder-600.jpg'}
                alt={product.images[selectedImage]?.alt || product.name}
                className="w-full h-full object-cover"
              />
            </div>
            <div className="flex gap-3 overflow-x-auto">
              {product.images.map((img, i) => (
                <button
                  key={img.id}
                  onClick={() => setSelectedImage(i)}
                  className={`w-20 h-20 rounded-lg overflow-hidden border-2 shrink-0 ${
                    i === selectedImage ? 'border-copper' : 'border-gray-200'
                  }`}
                >
                  <img src={img.url} alt={img.alt} className="w-full h-full object-cover" />
                </button>
              ))}
            </div>
          </div>

          {/* Info */}
          <div>
            <p className="text-sm text-copper uppercase tracking-wider mb-2">{product.categoryName}</p>
            <h1 className="font-heading text-2xl lg:text-3xl text-dark mb-3">{product.name}</h1>
            <div className="flex items-center gap-3 mb-4">
              <Rating value={product.averageRating} size="md" showValue />
              <span className="text-sm text-muted">({product.reviewCount} reviews)</span>
            </div>

            <div className="flex items-baseline gap-3 mb-6">
              <span className="text-3xl font-bold text-dark">{formatCurrency(variant?.price ?? product.basePrice)}</span>
              {variant?.compareAtPrice && (
                <>
                  <span className="text-lg text-muted line-through">{formatCurrency(variant.compareAtPrice)}</span>
                  <span className="text-sm font-medium text-red-600">-{discount}% OFF</span>
                </>
              )}
            </div>

            {product.shortDescription && (
              <p className="text-muted mb-6">{product.shortDescription}</p>
            )}

            {/* Variants */}
            {product.variants.length > 1 && (
              <div className="mb-6">
                <p className="text-sm font-medium text-dark mb-2">Variant</p>
                <div className="flex flex-wrap gap-2">
                  {product.variants.map((v, i) => (
                    <button
                      key={v.id}
                      onClick={() => setSelectedVariant(i)}
                      className={`px-4 py-2 rounded-lg border text-sm ${
                        i === selectedVariant
                          ? 'border-copper bg-copper/5 text-copper font-medium'
                          : 'border-gray-300 text-dark hover:border-copper'
                      }`}
                    >
                      {v.name}
                    </button>
                  ))}
                </div>
              </div>
            )}

            {/* Quantity & Add to Cart */}
            <div className="flex items-center gap-4 mb-8">
              <QuantityStepper value={quantity} onChange={setQuantity} max={variant?.stock ?? 10} />
              <Button onClick={handleAddToCart} size="lg" fullWidth disabled={!variant || variant.stock === 0}>
                {variant && variant.stock === 0 ? 'Out of Stock' : 'Add to Cart'}
              </Button>
              <button
                onClick={handleWishlistToggle}
                className={`shrink-0 p-3 rounded-lg border transition-colors ${
                  isWishlisted
                    ? 'border-red-300 bg-red-50 text-red-500'
                    : 'border-gray-300 text-muted hover:border-copper hover:text-copper'
                }`}
                title={isWishlisted ? 'Remove from Wishlist' : 'Add to Wishlist'}
              >
                <svg className="w-5 h-5" fill={isWishlisted ? 'currentColor' : 'none'} viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
                </svg>
              </button>
            </div>

            {/* Details */}
            {product.material && (
              <div className="border-t border-gray-200 pt-4 mb-4">
                <p className="text-sm"><span className="font-medium text-dark">Material:</span> <span className="text-muted">{product.material}</span></p>
              </div>
            )}

            <div className="border-t border-gray-200 pt-4">
              <h3 className="font-medium text-dark mb-2">Description</h3>
              <p className="text-sm text-muted leading-relaxed">{product.description}</p>
            </div>

            {product.tags.length > 0 && (
              <div className="flex flex-wrap gap-2 mt-4">
                {product.tags.map((tag) => (
                  <span key={tag} className="text-xs bg-gold-50 text-copper px-2 py-1 rounded">{tag}</span>
                ))}
              </div>
            )}
          </div>
        </div>

        {/* Reviews */}
        {reviews.length > 0 && (
          <section className="py-12 border-t border-gray-200">
            <h2 className="font-heading text-2xl text-dark mb-6">Customer Reviews</h2>
            <div className="space-y-6">
              {reviews.map((review) => (
                <div key={review.id} className="border-b border-gray-100 pb-6">
                  <div className="flex items-center gap-3 mb-2">
                    <Rating value={review.rating} />
                    {review.title && <span className="font-medium text-dark text-sm">{review.title}</span>}
                  </div>
                  <p className="text-sm text-muted mb-2">{review.comment}</p>
                  <p className="text-xs text-muted">
                    {review.userName} {review.isVerifiedPurchase && <span className="text-green-600">Verified Purchase</span>}
                  </p>
                </div>
              ))}
            </div>
          </section>
        )}

        {/* Related Products */}
        {relatedProducts.length > 0 && (
          <section className="py-12 border-t border-gray-200">
            <h2 className="font-heading text-2xl text-dark mb-6">You May Also Like</h2>
            <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4 lg:gap-6">
              {relatedProducts.slice(0, 4).map((p) => (
                <ProductCard key={p.id} product={p} />
              ))}
            </div>
          </section>
        )}
      </div>
    </>
  );
}
