import { useState, Component, type ReactNode, type ErrorInfo } from 'react';
import { Link } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import toast from 'react-hot-toast';
import { useCartStore } from '@/store/cartStore';
import { applyCoupon as applyCouponApi } from '@/api/cart';
import { formatCurrency } from '@/utils/format';
import { Button, QuantityStepper, EmptyState, Breadcrumb } from '@/components/common';

class CartErrorBoundary extends Component<{ children: ReactNode }, { error: Error | null }> {
  state = { error: null as Error | null };
  static getDerivedStateFromError(error: Error) { return { error }; }
  componentDidCatch(error: Error, info: ErrorInfo) { console.error('CartPage crash:', error, info); }
  render() {
    if (this.state.error) {
      return (
        <div className="max-w-7xl mx-auto px-4 py-16 text-center">
          <h2 className="text-xl font-bold text-red-600 mb-4">Something went wrong in Cart</h2>
          <p className="text-muted mb-4">{this.state.error.message}</p>
          <button onClick={() => { localStorage.removeItem('cart-storage'); window.location.reload(); }}
            className="px-4 py-2 bg-copper text-white rounded-lg">Clear Cart & Retry</button>
        </div>
      );
    }
    return this.props.children;
  }
}

function CartPageInner() {
  const {
    items, couponCode, couponDiscount,
    removeItem, updateQuantity, toggleGiftWrap, setGiftMessage,
    applyCoupon, removeCoupon,
    getSubtotal, getGiftWrapTotal, getShipping, getTax, getTotal,
  } = useCartStore();

  const [couponInput, setCouponInput] = useState('');
  const [applyingCoupon, setApplyingCoupon] = useState(false);

  const handleApplyCoupon = async () => {
    if (!couponInput.trim()) return;
    setApplyingCoupon(true);
    try {
      const res = await applyCouponApi(couponInput.trim());
      applyCoupon(res.data.couponCode, res.data.discount);
      toast.success(res.data.message);
      setCouponInput('');
    } catch {
      toast.error('Invalid coupon code');
    } finally {
      setApplyingCoupon(false);
    }
  };

  if (items.length === 0) {
    return (
      <>
        <Helmet><title>Cart | Indian Art Villa</title></Helmet>
        <div className="max-w-7xl mx-auto px-4">
          <Breadcrumb items={[{ label: 'Cart' }]} />
          <EmptyState
            title="Your cart is empty"
            description="Looks like you haven't added anything to your cart yet."
            action={<Link to="/"><Button>Continue Shopping</Button></Link>}
          />
        </div>
      </>
    );
  }

  return (
    <>
      <Helmet><title>{`Cart (${items.length}) | Indian Art Villa`}</title></Helmet>
      <div className="max-w-7xl mx-auto px-4 pb-16">
        <Breadcrumb items={[{ label: 'Shopping Cart' }]} />
        <h1 className="font-heading text-3xl text-dark mb-8">Shopping Cart</h1>

        <div className="grid lg:grid-cols-3 gap-10">
          {/* Items */}
          <div className="lg:col-span-2 space-y-6">
            {items.map((item) => (
              <div key={`${item.productId}-${item.variantId}`} className="flex gap-4 border-b border-gray-100 pb-6">
                <Link to={`/products/${item.slug}`} className="w-24 h-24 rounded-lg overflow-hidden bg-gold-50 shrink-0">
                  <img src={item.image || '/images/placeholder-400.jpg'} alt={item.name} className="w-full h-full object-cover" />
                </Link>
                <div className="flex-1 min-w-0">
                  <Link to={`/products/${item.slug}`} className="font-medium text-dark hover:text-copper line-clamp-1">{item.name}</Link>
                  <p className="text-sm text-muted">{item.variantName}</p>
                  <p className="font-semibold text-dark mt-1">{formatCurrency(item.price)}</p>
                  <div className="flex items-center gap-4 mt-3">
                    <QuantityStepper value={item.quantity} onChange={(q) => updateQuantity(item.productId, item.variantId, q)} />
                    <button onClick={() => removeItem(item.productId, item.variantId)} className="text-sm text-red-500 hover:text-red-700">
                      Remove
                    </button>
                  </div>
                  <div className="mt-3">
                    <label className="flex items-center gap-2 text-sm text-muted cursor-pointer">
                      <input
                        type="checkbox"
                        checked={item.giftWrap}
                        onChange={() => toggleGiftWrap(item.productId, item.variantId)}
                        className="rounded border-gray-300 text-copper focus:ring-copper"
                      />
                      Gift wrap (+Rs. 50)
                    </label>
                    {item.giftWrap && (
                      <input
                        type="text"
                        value={item.giftMessage ?? ''}
                        onChange={(e) => setGiftMessage(item.productId, item.variantId, e.target.value)}
                        placeholder="Gift message (optional)"
                        className="mt-2 w-full text-sm border border-gray-300 rounded-lg px-3 py-1.5 focus:outline-none focus:ring-1 focus:ring-copper"
                      />
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>

          {/* Summary */}
          <div className="bg-gold-50 rounded-xl p-6 h-fit sticky top-28">
            <h3 className="font-heading text-xl text-dark mb-4">Order Summary</h3>
            <div className="space-y-3 text-sm">
              <div className="flex justify-between"><span className="text-muted">Subtotal</span><span>{formatCurrency(getSubtotal())}</span></div>
              {getGiftWrapTotal() > 0 && (
                <div className="flex justify-between"><span className="text-muted">Gift Wrap</span><span>{formatCurrency(getGiftWrapTotal())}</span></div>
              )}
              <div className="flex justify-between"><span className="text-muted">Shipping</span><span>{getShipping() === 0 ? 'FREE' : formatCurrency(getShipping())}</span></div>
              <div className="flex justify-between"><span className="text-muted">Tax (GST 18%)</span><span>{formatCurrency(getTax())}</span></div>
              {couponDiscount > 0 && (
                <div className="flex justify-between text-green-600">
                  <span>Discount ({couponCode})</span>
                  <span>-{formatCurrency(couponDiscount)}</span>
                </div>
              )}
              <div className="border-t border-gray-200 pt-3 flex justify-between font-semibold text-lg text-dark">
                <span>Total</span><span>{formatCurrency(getTotal())}</span>
              </div>
            </div>

            {/* Coupon */}
            {!couponCode ? (
              <div className="flex gap-2 mt-4">
                <input
                  type="text"
                  value={couponInput}
                  onChange={(e) => setCouponInput(e.target.value)}
                  placeholder="Coupon code"
                  className="flex-1 text-sm border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-1 focus:ring-copper"
                />
                <Button size="sm" onClick={handleApplyCoupon} isLoading={applyingCoupon}>Apply</Button>
              </div>
            ) : (
              <button onClick={removeCoupon} className="text-sm text-red-500 mt-3">Remove coupon</button>
            )}

            <Link to="/checkout">
              <Button fullWidth size="lg" className="mt-6">Proceed to Checkout</Button>
            </Link>
          </div>
        </div>
      </div>
    </>
  );
}

export default function CartPage() {
  return (
    <CartErrorBoundary>
      <CartPageInner />
    </CartErrorBoundary>
  );
}
