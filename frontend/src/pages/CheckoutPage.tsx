import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import toast from 'react-hot-toast';
import { useQuery } from '@tanstack/react-query';
import { useCartStore } from '@/store/cartStore';
import { useAuthStore } from '@/store/authStore';
import { getAddresses } from '@/api/addresses';
import { createOrder } from '@/api/orders';
import { formatCurrency } from '@/utils/format';
import { Button, Breadcrumb, Spinner } from '@/components/common';

export default function CheckoutPage() {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuthStore();
  const { items, couponCode, getSubtotal, getGiftWrapTotal, getShipping, getTax, getTotal, clearCart } = useCartStore();
  const { data: addressesData, isLoading: loadingAddresses } = useQuery({
    queryKey: ['addresses'],
    queryFn: getAddresses,
    enabled: isAuthenticated,
  });

  const addresses = addressesData?.data ?? [];
  const defaultAddr = addresses.find((a) => a.isDefault) || addresses[0];

  const [selectedAddressId, setSelectedAddressId] = useState(defaultAddr?.id ?? '');
  const [paymentMethod, setPaymentMethod] = useState('razorpay');
  const [placing, setPlacing] = useState(false);

  if (!isAuthenticated) {
    navigate('/login', { state: { from: '/checkout' } });
    return null;
  }

  if (items.length === 0) {
    navigate('/cart');
    return null;
  }

  const handlePlaceOrder = async () => {
    if (!selectedAddressId) {
      toast.error('Please select a delivery address');
      return;
    }
    setPlacing(true);
    try {
      const res = await createOrder({
        items: items.map((i) => ({
          productId: i.productId,
          variantId: i.variantId,
          quantity: i.quantity,
          giftWrap: i.giftWrap,
          giftMessage: i.giftMessage,
        })),
        shippingAddressId: selectedAddressId,
        billingAddressId: selectedAddressId,
        paymentMethod,
        couponCode: couponCode ?? undefined,
      });
      clearCart();
      toast.success('Order placed successfully!');
      navigate(`/account/orders`, { state: { orderId: res.data.id } });
    } catch {
      toast.error('Failed to place order. Please try again.');
    } finally {
      setPlacing(false);
    }
  };

  return (
    <>
      <Helmet><title>Checkout | Indian Art Villa</title></Helmet>
      <div className="max-w-7xl mx-auto px-4 pb-16">
        <Breadcrumb items={[{ label: 'Cart', href: '/cart' }, { label: 'Checkout' }]} />
        <h1 className="font-heading text-3xl text-dark mb-8">Checkout</h1>

        <div className="grid lg:grid-cols-3 gap-10">
          <div className="lg:col-span-2 space-y-8">
            {/* Address selection */}
            <div>
              <h2 className="font-heading text-xl text-dark mb-4">Delivery Address</h2>
              {loadingAddresses ? (
                <Spinner />
              ) : addresses.length === 0 ? (
                <p className="text-muted text-sm">No addresses found. Please add an address in your account settings.</p>
              ) : (
                <div className="grid md:grid-cols-2 gap-3">
                  {addresses.map((addr) => (
                    <label
                      key={addr.id}
                      className={`border rounded-lg p-4 cursor-pointer transition-colors ${
                        selectedAddressId === addr.id ? 'border-copper bg-copper/5' : 'border-gray-200 hover:border-copper'
                      }`}
                    >
                      <input
                        type="radio"
                        name="address"
                        value={addr.id}
                        checked={selectedAddressId === addr.id}
                        onChange={() => setSelectedAddressId(addr.id)}
                        className="sr-only"
                      />
                      <p className="font-medium text-dark text-sm">{addr.fullName}</p>
                      <p className="text-xs text-muted mt-1">{addr.addressLine1}, {addr.city}, {addr.state} {addr.postalCode}</p>
                      <p className="text-xs text-muted">{addr.phone}</p>
                    </label>
                  ))}
                </div>
              )}
            </div>

            {/* Payment */}
            <div>
              <h2 className="font-heading text-xl text-dark mb-4">Payment Method</h2>
              <div className="space-y-2">
                {[
                  { value: 'razorpay', label: 'Razorpay (UPI, Cards, Net Banking)' },
                  { value: 'cod', label: 'Cash on Delivery' },
                ].map((pm) => (
                  <label
                    key={pm.value}
                    className={`flex items-center gap-3 border rounded-lg p-4 cursor-pointer transition-colors ${
                      paymentMethod === pm.value ? 'border-copper bg-copper/5' : 'border-gray-200'
                    }`}
                  >
                    <input
                      type="radio"
                      name="payment"
                      value={pm.value}
                      checked={paymentMethod === pm.value}
                      onChange={() => setPaymentMethod(pm.value)}
                      className="text-copper focus:ring-copper"
                    />
                    <span className="text-sm text-dark">{pm.label}</span>
                  </label>
                ))}
              </div>
            </div>
          </div>

          {/* Summary */}
          <div className="bg-gold-50 rounded-xl p-6 h-fit sticky top-28">
            <h3 className="font-heading text-xl text-dark mb-4">Order Summary</h3>
            <div className="space-y-2 mb-4">
              {items.map((item) => (
                <div key={`${item.productId}-${item.variantId}`} className="flex justify-between text-sm">
                  <span className="text-muted truncate max-w-[200px]">{item.name} x{item.quantity}</span>
                  <span>{formatCurrency(item.price * item.quantity)}</span>
                </div>
              ))}
            </div>
            <div className="space-y-2 text-sm border-t border-gray-200 pt-3">
              <div className="flex justify-between"><span className="text-muted">Subtotal</span><span>{formatCurrency(getSubtotal())}</span></div>
              {getGiftWrapTotal() > 0 && <div className="flex justify-between"><span className="text-muted">Gift Wrap</span><span>{formatCurrency(getGiftWrapTotal())}</span></div>}
              <div className="flex justify-between"><span className="text-muted">Shipping</span><span>{getShipping() === 0 ? 'FREE' : formatCurrency(getShipping())}</span></div>
              <div className="flex justify-between"><span className="text-muted">Tax (GST)</span><span>{formatCurrency(getTax())}</span></div>
              <div className="border-t border-gray-200 pt-3 flex justify-between font-semibold text-lg text-dark">
                <span>Total</span><span>{formatCurrency(getTotal())}</span>
              </div>
            </div>
            <Button fullWidth size="lg" className="mt-6" onClick={handlePlaceOrder} isLoading={placing}>
              Place Order
            </Button>
          </div>
        </div>
      </div>
    </>
  );
}
