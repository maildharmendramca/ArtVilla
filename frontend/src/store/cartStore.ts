import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import type { CartItem } from '@/types';

const GIFT_WRAP_COST = 50;
const FREE_SHIPPING_THRESHOLD = 2000;
const SHIPPING_COST = 99;
const TAX_RATE = 0.18;

interface CartStore {
  items: CartItem[];
  couponCode: string | null;
  couponDiscount: number;

  addItem: (item: CartItem) => void;
  removeItem: (productId: string, variantId: string) => void;
  updateQuantity: (productId: string, variantId: string, quantity: number) => void;
  toggleGiftWrap: (productId: string, variantId: string) => void;
  setGiftMessage: (productId: string, variantId: string, message: string) => void;
  applyCoupon: (code: string, discount: number) => void;
  removeCoupon: () => void;
  clearCart: () => void;

  getSubtotal: () => number;
  getGiftWrapTotal: () => number;
  getShipping: () => number;
  getTax: () => number;
  getTotal: () => number;
  getItemCount: () => number;
}

export const useCartStore = create<CartStore>()(
  persist(
    (set, get) => ({
      items: [],
      couponCode: null,
      couponDiscount: 0,

      addItem: (item) =>
        set((state) => {
          // Ensure price is valid
          if (!item.price && item.price !== 0) return state;
          const existing = state.items.find(
            (i) => i.productId === item.productId && i.variantId === item.variantId,
          );
          if (existing) {
            return {
              items: state.items.map((i) =>
                i.productId === item.productId && i.variantId === item.variantId
                  ? { ...i, quantity: i.quantity + item.quantity }
                  : i,
              ),
            };
          }
          return { items: [...state.items, item] };
        }),

      removeItem: (productId, variantId) =>
        set((state) => ({
          items: state.items.filter(
            (i) => !(i.productId === productId && i.variantId === variantId),
          ),
        })),

      updateQuantity: (productId, variantId, quantity) =>
        set((state) => ({
          items:
            quantity <= 0
              ? state.items.filter(
                  (i) => !(i.productId === productId && i.variantId === variantId),
                )
              : state.items.map((i) =>
                  i.productId === productId && i.variantId === variantId
                    ? { ...i, quantity }
                    : i,
                ),
        })),

      toggleGiftWrap: (productId, variantId) =>
        set((state) => ({
          items: state.items.map((i) =>
            i.productId === productId && i.variantId === variantId
              ? { ...i, giftWrap: !i.giftWrap }
              : i,
          ),
        })),

      setGiftMessage: (productId, variantId, message) =>
        set((state) => ({
          items: state.items.map((i) =>
            i.productId === productId && i.variantId === variantId
              ? { ...i, giftMessage: message }
              : i,
          ),
        })),

      applyCoupon: (code, discount) => set({ couponCode: code, couponDiscount: discount }),
      removeCoupon: () => set({ couponCode: null, couponDiscount: 0 }),
      clearCart: () => set({ items: [], couponCode: null, couponDiscount: 0 }),

      getSubtotal: () => get().items.reduce((sum, i) => sum + i.price * i.quantity, 0),
      getGiftWrapTotal: () =>
        get().items.filter((i) => i.giftWrap).reduce((sum, i) => sum + GIFT_WRAP_COST * i.quantity, 0),
      getShipping: () => {
        const subtotal = get().getSubtotal();
        return subtotal >= FREE_SHIPPING_THRESHOLD || get().items.length === 0 ? 0 : SHIPPING_COST;
      },
      getTax: () => Math.round(get().getSubtotal() * TAX_RATE),
      getTotal: () => {
        const s = get();
        return s.getSubtotal() + s.getGiftWrapTotal() + s.getShipping() + s.getTax() - s.couponDiscount;
      },
      getItemCount: () => get().items.reduce((sum, i) => sum + i.quantity, 0),
    }),
    {
      name: 'cart-storage',
      version: 2,
      migrate: () => ({ items: [], couponCode: null, couponDiscount: 0 }),
      merge: (persisted, current) => {
        const p = persisted as Partial<CartStore> | undefined;
        if (!p || !Array.isArray(p.items)) return current;
        // Filter out any items missing required fields
        const validItems = p.items.filter(
          (i) => i && typeof i.price === 'number' && i.productId != null && i.variantId != null,
        );
        return { ...current, ...p, items: validItems };
      },
    },
  ),
);
