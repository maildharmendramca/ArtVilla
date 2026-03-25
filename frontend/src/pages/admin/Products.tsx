import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useQuery } from '@tanstack/react-query';
import { getAdminProducts } from '@/api/admin';
import { formatCurrency } from '@/utils/format';
import { Button, Badge, Spinner } from '@/components/common';

export default function AdminProducts() {
  const navigate = useNavigate();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['admin', 'products', page, search],
    queryFn: () => getAdminProducts(page, 20, search || undefined),
  });

  const products = data?.data ?? [];
  const pagination = data?.pagination;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-heading text-2xl text-dark">Products</h1>
        <Button onClick={() => navigate('/admin/products/new')}>Add Product</Button>
      </div>

      <div className="mb-4">
        <input
          type="text"
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(1); }}
          placeholder="Search products..."
          className="w-full max-w-sm text-sm border border-gray-300 rounded-lg px-4 py-2 focus:outline-none focus:ring-2 focus:ring-copper"
        />
      </div>

      {isLoading ? (
        <Spinner className="py-16" />
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-left">
              <tr>
                <th className="px-4 py-3 text-muted font-medium">Product</th>
                <th className="px-4 py-3 text-muted font-medium">Category</th>
                <th className="px-4 py-3 text-muted font-medium">Price</th>
                <th className="px-4 py-3 text-muted font-medium">Stock</th>
                <th className="px-4 py-3 text-muted font-medium">Status</th>
                <th className="px-4 py-3 text-muted font-medium">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {products.map((product: any) => (
                <tr key={product.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3">
                    <div className="flex items-center gap-3">
                      <div className="w-10 h-10 rounded bg-gold-50 overflow-hidden shrink-0">
                        {(product.primaryImage || product.images?.[0]?.url) && (
                          <img src={product.primaryImage || product.images[0].url} alt="" className="w-full h-full object-cover" />
                        )}
                      </div>
                      <span className="font-medium text-dark truncate max-w-[200px]">{product.name}</span>
                    </div>
                  </td>
                  <td className="px-4 py-3 text-muted">{product.categoryName}</td>
                  <td className="px-4 py-3">{formatCurrency(product.basePrice)}</td>
                  <td className="px-4 py-3">
                    {product.stockQuantity ?? product.variants?.reduce((sum: number, v: any) => sum + v.stock, 0) ?? 0}
                  </td>
                  <td className="px-4 py-3">
                    <Badge variant={product.isActive !== false ? 'success' : 'danger'}>
                      {product.isActive !== false ? 'Active' : 'Inactive'}
                    </Badge>
                  </td>
                  <td className="px-4 py-3">
                    <button onClick={() => navigate(`/admin/products/${product.id}/edit`)} className="text-copper hover:text-gold-700 text-sm">Edit</button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {pagination && pagination.totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-6">
          <button disabled={!pagination.hasPrevPage} onClick={() => setPage((p) => p - 1)} className="px-3 py-1 text-sm border rounded disabled:opacity-40">Prev</button>
          <span className="px-3 py-1 text-sm text-muted">Page {pagination.currentPage} of {pagination.totalPages}</span>
          <button disabled={!pagination.hasNextPage} onClick={() => setPage((p) => p + 1)} className="px-3 py-1 text-sm border rounded disabled:opacity-40">Next</button>
        </div>
      )}
    </div>
  );
}
