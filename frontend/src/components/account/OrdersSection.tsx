import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useOrders } from '@/hooks/useOrders';
import { formatCurrency, formatDate } from '@/utils/format';
import { Badge, Spinner, EmptyState } from '@/components/common';
import type { OrderStatus } from '@/types';

const statusBadge: Record<OrderStatus, 'default' | 'success' | 'warning' | 'danger' | 'info'> = {
  pending: 'warning',
  confirmed: 'info',
  processing: 'info',
  shipped: 'info',
  delivered: 'success',
  cancelled: 'danger',
  refunded: 'danger',
};

export default function OrdersSection() {
  const [page, setPage] = useState(1);
  const { data, isLoading } = useOrders(page);
  const orders = data?.data ?? [];
  const pagination = data?.pagination;

  if (isLoading) return <Spinner className="py-16" />;

  if (orders.length === 0) {
    return (
      <EmptyState
        title="No orders yet"
        description="Start shopping to see your orders here."
        action={<Link to="/" className="text-copper font-medium hover:text-gold-700">Browse Products</Link>}
      />
    );
  }

  return (
    <div>
      <h2 className="font-heading text-2xl text-dark mb-6">My Orders</h2>
      <div className="space-y-4">
        {orders.map((order) => (
          <div key={order.id} className="border border-gray-200 rounded-lg p-4 hover:border-copper transition-colors">
            <div className="flex items-center justify-between mb-3">
              <div>
                <p className="font-medium text-dark">Order #{order.orderNumber}</p>
                <p className="text-xs text-muted">{formatDate(order.createdAt)}</p>
              </div>
              <Badge variant={statusBadge[order.status]}>{order.status}</Badge>
            </div>
            <div className="flex items-center justify-between">
              <p className="text-sm text-muted">{order.items.length} item(s)</p>
              <p className="font-semibold text-dark">{formatCurrency(order.total)}</p>
            </div>
          </div>
        ))}
      </div>
      {pagination && pagination.totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-8">
          <button
            disabled={!pagination.hasPrevPage}
            onClick={() => setPage((p) => p - 1)}
            className="px-4 py-2 text-sm rounded-lg border border-gray-300 disabled:opacity-40"
          >
            Previous
          </button>
          <span className="px-4 py-2 text-sm text-muted">
            Page {pagination.currentPage} of {pagination.totalPages}
          </span>
          <button
            disabled={!pagination.hasNextPage}
            onClick={() => setPage((p) => p + 1)}
            className="px-4 py-2 text-sm rounded-lg border border-gray-300 disabled:opacity-40"
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}
