// ---- Product ----
export interface ProductImage {
  id: string;
  url: string;
  alt: string;
  isPrimary: boolean;
  sortOrder: number;
}

export interface ProductVariant {
  id: string;
  name: string;
  sku: string;
  price: number;
  compareAtPrice?: number;
  stock: number;
  weight?: number;
  dimensions?: string;
  attributes: Record<string, string>;
}

export interface Product {
  id: string;
  name: string;
  slug: string;
  description: string;
  shortDescription?: string;
  categoryId: string;
  categoryName: string;
  images: ProductImage[];
  variants: ProductVariant[];
  basePrice: number;
  compareAtPrice?: number;
  averageRating: number;
  reviewCount: number;
  stockQuantity: number;
  tags: string[];
  material?: string;
  isFeatured: boolean;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ProductListItem {
  id: string;
  name: string;
  slug: string;
  shortDescription?: string;
  categoryId: string;
  categoryName: string;
  primaryImage?: string;
  basePrice: number;
  compareAtPrice?: number;
  averageRating: number;
  reviewCount: number;
  material?: string;
  isFeatured: boolean;
  isNew: boolean;
}

// ---- Category ----
export interface Category {
  id: string;
  name: string;
  slug: string;
  description?: string;
  image?: string;
  parentId?: string;
  children?: Category[];
  productCount: number;
  sortOrder: number;
  isActive: boolean;
}

export interface CategorySummary {
  id: string;
  name: string;
  slug: string;
  image?: string;
  productCount: number;
}

// ---- Cart ----
export interface CartItem {
  productId: string;
  variantId: string;
  name: string;
  variantName: string;
  image?: string;
  price: number;
  quantity: number;
  giftWrap: boolean;
  giftMessage?: string;
  slug: string;
}

export interface CartState {
  items: CartItem[];
  couponCode?: string;
  couponDiscount: number;
}

// ---- Order ----
export type OrderStatus =
  | 'pending'
  | 'confirmed'
  | 'processing'
  | 'shipped'
  | 'delivered'
  | 'cancelled'
  | 'refunded';

export interface OrderItem {
  productId: string;
  variantId: string;
  name: string;
  variantName: string;
  image?: string;
  price: number;
  quantity: number;
  giftWrap: boolean;
  giftMessage?: string;
}

export interface Order {
  id: string;
  orderNumber: string;
  status: OrderStatus;
  items: OrderItem[];
  subtotal: number;
  shippingCost: number;
  taxAmount: number;
  discount: number;
  total: number;
  shippingAddress: Address;
  billingAddress: Address;
  paymentMethod: string;
  trackingNumber?: string;
  createdAt: string;
  updatedAt: string;
}

export interface OrderDetail extends Order {
  user: { id: string; name: string; email: string };
  notes?: string;
  timeline: { status: OrderStatus; date: string; note?: string }[];
}

// ---- Address ----
export interface Address {
  id: string;
  fullName: string;
  phone: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
  isDefault: boolean;
}

// ---- User / Auth ----
export interface User {
  id: string;
  name: string;
  email: string;
  phone?: string;
  avatar?: string;
  role: 'customer' | 'admin';
  createdAt: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
}

// ---- Review ----
export interface Review {
  id: string;
  productId: string;
  userId: string;
  userName: string;
  rating: number;
  title?: string;
  comment: string;
  images?: string[];
  isVerifiedPurchase: boolean;
  createdAt: string;
}

// ---- API / Generic ----
export interface ApiResponse<T> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginationMeta {
  currentPage: number;
  totalPages: number;
  totalItems: number;
  itemsPerPage: number;
  hasNextPage: boolean;
  hasPrevPage: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  pagination: PaginationMeta;
}

// ---- Dashboard ----
export interface DashboardStats {
  totalRevenue: number;
  totalOrders: number;
  totalCustomers: number;
  totalProducts: number;
  revenueChange: number;
  ordersChange: number;
  recentOrders: Order[];
  topProducts: { product: ProductListItem; soldCount: number; revenue: number }[];
  revenueByMonth: { month: string; revenue: number }[];
}
