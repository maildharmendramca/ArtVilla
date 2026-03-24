# Indian Art Villa - Developer Documentation

## Project Overview

Indian Art Villa is a full-featured e-commerce platform for Indian handicraft products (copper, brass, bronze items). It consists of a **React SPA frontend** and an **ASP.NET Core 8 Web API backend** with in-memory static data.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 18, Vite, TypeScript, Tailwind CSS v4 |
| State (Server) | TanStack React Query |
| State (Client) | Zustand (persisted to localStorage) |
| Routing | React Router v6 |
| Backend | ASP.NET Core 8 Web API |
| Data | In-memory static collections (InMemoryDataStore) |
| Auth | JWT Bearer tokens |
| Architecture | Clean Architecture (Core / Infrastructure / API) |

---

## How to Run

```bash
# Backend (Terminal 1)
cd backend/IndianArtVilla.API
dotnet run --urls "http://localhost:5000"

# Frontend (Terminal 2)
cd frontend
npm run dev    # runs on http://localhost:3000
```

The Vite dev server proxies `/api` requests to `http://localhost:5000`.

**Test Credentials:**
- Admin: `admin@indianartvilla.in` / `Admin@123`
- Customer: `rahul@example.com` / `Customer@123`

---

## Architecture

```
ArtVillaV1/
├── frontend/                    # React SPA
│   └── src/
│       ├── api/                 # Axios API client & endpoint functions
│       ├── hooks/               # React Query hooks
│       ├── store/               # Zustand stores (auth, cart, ui)
│       ├── pages/               # Route page components
│       ├── components/          # Reusable UI components
│       ├── types/               # TypeScript interfaces
│       └── utils/               # Helpers (formatCurrency, etc.)
│
├── backend/
│   ├── IndianArtVilla.Core/     # Entities, DTOs, Interfaces, Enums
│   ├── IndianArtVilla.Infrastructure/
│   │   ├── Data/                # InMemoryDataStore (static seed data)
│   │   └── Services/            # Service implementations
│   └── IndianArtVilla.API/
│       ├── Controllers/         # REST API endpoints
│       ├── Middleware/           # Exception handling
│       └── Program.cs           # App configuration
```

---

## Frontend Routes

```
Public Routes (Layout wrapper with Header + Footer)
├── /                           → HomePage
├── /collections/:slug          → ProductListingPage
├── /products/:slug             → ProductDetailPage
├── /cart                       → CartPage
├── /checkout                   → CheckoutPage (auth required)
├── /search?q=...               → SearchPage
├── /login                      → LoginPage
└── /register                   → RegisterPage

Account Routes (auth required)
├── /account                    → ProfileSection
├── /account/orders             → OrdersSection
├── /account/addresses          → AddressesSection
└── /account/wishlist           → WishlistSection

Admin Routes (admin role required)
├── /admin                      → Dashboard
├── /admin/products             → AdminProducts
├── /admin/orders               → AdminOrders
├── /admin/categories           → AdminCategories
└── /admin/customers            → AdminCustomers
```

---

## Frontend → Backend Data Flow

### How a Page Loads Data

```
Page Component
  ↓ calls
React Query Hook (src/hooks/)
  ↓ calls
API Function (src/api/)
  ↓ HTTP request via Axios client
Backend Controller (Controllers/)
  ↓ calls
Service (Infrastructure/Services/)
  ↓ queries
InMemoryDataStore (static List<T>)
  ↓ returns DTO
ApiResponse<T> JSON back to frontend
```

### Example: Product Detail Page

```
ProductDetailPage.tsx
  → useProduct(slug)                    // hook
    → getProductBySlug(slug)            // api/products.ts
      → GET /api/products/{slug}        // HTTP
        → ProductsController.GetBySlug  // controller
          → ProductService.GetBySlugAsync  // service
            → InMemoryDataStore.Products   // data
              → ProductDetailDto           // response
```

### Example: Add to Cart

```
ProductDetailPage.tsx → handleAddToCart()
  → cartStore.addItem(item)    // Zustand store (client-side only)
    → persisted to localStorage
    → cart count updates in Header
    → CartPage reads from same store
```

> **Note:** Cart is managed entirely client-side via Zustand. The backend cart API exists but is not called from the frontend store. Cart syncs with backend only at checkout via the order creation API.

---

## State Management

### Zustand Stores (Client State)

| Store | File | Persisted | Purpose |
|-------|------|-----------|---------|
| `useAuthStore` | `store/authStore.ts` | Yes (`auth-storage`) | User session, JWT token |
| `useCartStore` | `store/cartStore.ts` | Yes (`cart-storage`) | Cart items, coupons, totals |
| `useUiStore` | `store/uiStore.ts` | No | Mobile menu, search panel |

### React Query (Server State)

| Hook | File | API Call | Cache Key |
|------|------|----------|-----------|
| `useProducts(filters)` | `hooks/useProducts.ts` | `GET /products` | `['products', filters]` |
| `useProduct(slug)` | `hooks/useProducts.ts` | `GET /products/{slug}` | `['product', slug]` |
| `useFeaturedProducts()` | `hooks/useProducts.ts` | `GET /products/featured` | `['products', 'featured']` |
| `useNewArrivals()` | `hooks/useProducts.ts` | `GET /products/new-arrivals` | `['products', 'new-arrivals']` |
| `useBestSellers()` | `hooks/useProducts.ts` | `GET /products/best-sellers` | `['products', 'best-sellers']` |
| `useRelatedProducts(id)` | `hooks/useProducts.ts` | `GET /products/{id}/related` | `['products', 'related', id]` |
| `useProductSearch(q)` | `hooks/useProducts.ts` | `GET /products/search?q=` | `['products', 'search', q]` |
| `useCategories()` | `hooks/useCategories.ts` | `GET /categories` | `['categories']` |
| `useCategoryTree()` | `hooks/useCategories.ts` | `GET /categories/tree` | `['categories', 'tree']` |
| `useCategory(slug)` | `hooks/useCategories.ts` | `GET /categories/{slug}` | `['category', slug]` |
| `useOrders(page)` | `hooks/useOrders.ts` | `GET /orders` | `['orders', page]` |
| `useOrder(id)` | `hooks/useOrders.ts` | `GET /orders/{id}` | `['order', id]` |
| `useProductReviews(id)` | `hooks/useReviews.ts` | `GET /products/{id}/reviews` | `['reviews', id]` |
| `useWishlist()` | `hooks/useWishlist.ts` | `GET /wishlist` | `['wishlist']` |

---

## Backend API Reference

All responses wrapped in `ApiResponse<T>`:
```json
{
  "success": true,
  "message": "optional message",
  "data": { ... }
}
```

Paginated responses include:
```json
{
  "data": [...],
  "pagination": {
    "currentPage": 1,
    "itemsPerPage": 12,
    "totalItems": 29,
    "totalPages": 3,
    "hasNextPage": true,
    "hasPrevPage": false
  }
}
```

### Auth Endpoints (`/api/auth`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/auth/register` | - | Register new user |
| POST | `/auth/login` | - | Login, returns JWT + user |
| POST | `/auth/refresh-token` | - | Refresh JWT token |
| POST | `/auth/refresh` | - | Alias for refresh-token |
| POST | `/auth/logout` | - | Logout (clears cookie) |
| POST | `/auth/forgot-password` | - | Request password reset |
| POST | `/auth/reset-password` | - | Reset password with token |
| GET | `/auth/me` | Bearer | Get current user profile |
| PATCH | `/auth/me` | Bearer | Update profile |
| POST | `/auth/change-password` | Bearer | Change password |

### Product Endpoints (`/api/products`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/products` | - | List products (filterable, paginated) |
| GET | `/products/{slug}` | - | Get product detail by slug |
| GET | `/products/featured` | - | Featured products |
| GET | `/products/new-arrivals` | - | New arrivals |
| GET | `/products/best-sellers` | - | Best sellers |
| GET | `/products/search?q=` | - | Search products |
| GET | `/products/{id}/related` | - | Related products |
| GET | `/products/{id}/reviews` | - | Product reviews |

**Product Filter Query Params:**
```
?page=1&pageSize=12&categorySlug=copper-ware&material=Pure+Copper
&minPrice=500&maxPrice=2000&sortBy=price_asc&search=bottle&limit=8
```

### Category Endpoints (`/api/categories`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/categories` | - | All categories (flat list) |
| GET | `/categories/tree` | - | Hierarchical category tree |
| GET | `/categories/summaries` | - | Category summaries with counts |
| GET | `/categories/{slug}` | - | Single category by slug |
| GET | `/categories/{slug}/products` | - | Products in category |

### Cart Endpoints (`/api/cart`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/cart` | - | Get cart (uses session/cookie) |
| POST | `/cart/items` | - | Add item to cart |
| PUT | `/cart/items/{id}` | - | Update item quantity |
| DELETE | `/cart/items/{id}` | - | Remove item |
| POST | `/cart/coupon` | - | Apply coupon code |
| DELETE | `/cart/coupon` | - | Remove coupon |
| POST | `/cart/validate` | - | Validate cart items |
| POST | `/cart/merge` | Bearer | Merge guest cart into user cart |

### Order Endpoints (`/api/orders`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/orders` | Bearer | Create order (checkout) |
| GET | `/orders` | Bearer | List user's orders |
| GET | `/orders/{orderNumber}` | Bearer | Get order detail |
| PATCH | `/orders/{id}/cancel` | Bearer | Cancel order |

### Review Endpoints (`/api/reviews`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/reviews/product/{id}` | - | Get product reviews |
| POST | `/reviews` | Bearer | Create review |

### Wishlist Endpoints (`/api/wishlist`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/wishlist` | Bearer | Get wishlist items |
| POST | `/wishlist` | Bearer | Add to wishlist |
| DELETE | `/wishlist/{productId}` | Bearer | Remove from wishlist |
| GET | `/wishlist/check/{productId}` | Bearer | Check if product in wishlist |

### Address Endpoints (`/api/addresses`)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/addresses` | Bearer | List user addresses |
| GET | `/addresses/{id}` | Bearer | Get address by ID |
| POST | `/addresses` | Bearer | Create address |
| PUT | `/addresses/{id}` | Bearer | Update address |
| DELETE | `/addresses/{id}` | Bearer | Delete address |
| PATCH | `/addresses/{id}/default` | Bearer | Set as default |

### Admin Endpoints (`/api/admin`) — Requires Admin Role

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/admin/dashboard` | Dashboard stats |
| GET | `/admin/dashboard/revenue-chart?days=30` | Revenue chart data |
| GET | `/admin/dashboard/top-products?count=10` | Top selling products |
| GET | `/admin/products` | List all products |
| POST | `/admin/products` | Create product |
| GET | `/admin/products/{id}` | Get product |
| PUT | `/admin/products/{id}` | Update product |
| DELETE | `/admin/products/{id}` | Soft-delete product |
| GET | `/admin/categories` | List categories |
| POST | `/admin/categories` | Create category |
| PUT | `/admin/categories/{id}` | Update category |
| DELETE | `/admin/categories/{id}` | Delete category |
| GET | `/admin/orders?status=pending` | List orders (filterable) |
| GET | `/admin/orders/{id}` | Get order detail |
| PATCH | `/admin/orders/{id}/status` | Update order status |
| GET | `/admin/customers` | List customers |
| GET | `/admin/customers/{id}` | Get customer detail |

---

## Key Page Flows

### 1. Home Page Load
```
HomePage
  ├── HeroBanner           (static)
  ├── CollectionSpotlights (static, links to /collections/{slug})
  ├── FeaturedProducts     → useFeaturedProducts() → GET /products/featured
  ├── WhyChooseUs          (static)
  ├── NewArrivals          → useNewArrivals()      → GET /products/new-arrivals
  ├── BrandStory           (static)
  ├── BestSellers          → useBestSellers()      → GET /products/best-sellers
  └── Testimonials         (static)
```

### 2. Browse & Purchase Flow
```
Home Page
  → Click category/product
    → /collections/:slug    → useProducts({categorySlug})   → GET /products?categorySlug=...
    → /products/:slug       → useProduct(slug)              → GET /products/{slug}
      → Click "Add to Cart" → cartStore.addItem()           → (localStorage only)
        → /cart             → cartStore items                → (reads localStorage)
          → /checkout       → useCreateOrder()               → POST /orders
```

### 3. Authentication Flow
```
/login → authStore.login(email, password) → POST /auth/login
  → Response: { token, refreshToken, user }
  → Stored in Zustand (persisted to localStorage)
  → Axios interceptor attaches "Authorization: Bearer {token}" to all requests
  → On 401: interceptor tries POST /auth/refresh-token automatically
  → On refresh fail: redirect to /login
```

### 4. Admin Flow
```
/admin           → GET /admin/dashboard (stats, revenue chart, top products)
/admin/products  → GET /admin/products (paginated, searchable)
/admin/orders    → GET /admin/orders?status=... (filterable by status)
/admin/categories→ GET /admin/categories (tree view)
/admin/customers → GET /admin/customers (paginated, searchable)
```

---

## Cart Calculation Logic

Computed in `cartStore.ts` (client-side):

| Field | Formula |
|-------|---------|
| Subtotal | `sum(item.price * item.quantity)` |
| Gift Wrap | `count(giftWrap items) * Rs. 50` |
| Shipping | `subtotal >= Rs. 2000 ? FREE : Rs. 99` |
| Tax (GST) | `subtotal * 18%` |
| **Total** | `subtotal + giftWrap + shipping + tax - couponDiscount` |

---

## Backend Data Layer

### InMemoryDataStore

Static class at `Infrastructure/Data/InMemoryDataStore.cs` that replaces a database. Uses `List<T>` collections with thread-safe ID generation via `Interlocked.Increment`.

**Seeded Data:**
- 2 users (1 admin, 1 customer)
- 33 categories (3-level hierarchy: root → subcategory → sub-subcategory)
- 29 products with images and tags (each has a "Default" variant)
- 5 coupon codes: `WELCOME10`, `FLAT200`, `FREESHIP`, `DIWALI25`, `GIFT15`

**Swapping to a Real Database:**
All services implement interfaces from `Core/Interfaces/`. To switch to SQL Server:
1. Add EF Core DbContext in Infrastructure
2. Create new service implementations using DbContext
3. Change `AddSingleton` to `AddScoped` in `Program.cs`
4. Register new implementations against the same interfaces

---

## Frontend API Client

`src/api/client.ts` — Axios instance:
- Base URL: `/api` (proxied to backend by Vite)
- Request interceptor: Attaches JWT from `auth-storage` localStorage
- Response interceptor: Auto-refreshes expired tokens, redirects to `/login` on auth failure

---

## Key Configuration

### Backend (`Program.cs`)
- CORS: allows `http://localhost:3000`
- JWT: configured from `appsettings.json` → `Jwt` section
- JSON: camelCase, string enums, ignore nulls
- Services: registered as `Singleton` (swap to `Scoped` for real DB)

### Frontend (`vite.config.ts`)
- Port: 3000
- Proxy: `/api` → `http://localhost:5000`
- Path alias: `@/` → `src/`
- Plugins: React, @tailwindcss/vite

### React Query (`App.tsx`)
- Stale time: 60 seconds
- Retry: 1 attempt
- No refetch on window focus
