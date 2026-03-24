import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { getAdminOrders, updateOrderStatus } from '@/api/admin';
import { formatCurrency, formatDate } from '@/utils/format';
import { Badge, Spinner } from '@/components/common';
import type { OrderStatus } from '@/types';

const statusOptions: OrderStatus[] = ['pending', 'confirmed', 'processing', 'shipped', 'delivered', 'cancelled'];

const statusVariant: Record<OrderStatus, 'default' | 'success' | 'warning' | 'danger' | 'info'> = {
  pending: 'warning', confirmed: 'info', processing: 'info',
  shipped: 'info', delivered: 'success', cancelled: 'danger', refunded: 'danger',
};

export default function AdminOrders() {
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['admin', 'orders', page, statusFilter],
    queryFn: () => getAdminOrders(page, 20, statusFilter || undefined),
  });

  const updateStatus = useMutation({
    mutationFn: ({ id, status }: { id: string; status: string }) => updateOrderStatus(id, status),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin', 'orders'] });
      toast.success('Order status updated');
    },
    onError: () => toast.error('Failed to update status'),
  });

  const orders = data?.data ?? [];
  const pagination = data?.pagination;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-heading text-2xl text-dark">Orders</h1>
        <select
          value={statusFilter}
          onChange={(e) => { setStatusFilter(e.target.value); setPage(1); }}
          className="text-sm border border-gray-300 rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-copper"
        >
          <option value="">All Statuses</option>
          {statusOptions.map((s) => <option key={s} value={s}>{s}</option>)}
        </select>
      </div>

      {isLoading ? (
        <Spinner className="py-16" />
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden">
          <table className="w-full text-sm">
            <thead className="bg-gray-50 text-left">
              <tr>
                <th className="px-4 py-3 text-muted font-medium">Order #</th>
                <th className="px-4 py-3 text-muted font-medium">Date</th>
                <th className="px-4 py-3 text-muted font-medium">Items</th>
                <th className="px-4 py-3 text-muted font-medium">Total</th>
                <th className="px-4 py-3 text-muted font-medium">Status</th>
                <th className="px-4 py-3 text-muted font-medium">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {orders.map((order) => (
                <tr key={order.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3 font-medium text-dark">{order.orderNumber}</td>
                  <td className="px-4 py-3 text-muted">{formatDate(order.createdAt)}</td>
                  <td className="px-4 py-3 text-muted">{order.items.length}</td>
                  <td className="px-4 py-3 font-medium">{formatCurrency(order.total)}</td>
                  <td className="px-4 py-3">
                    <Badge variant={statusVariant[order.status]}>{order.status}</Badge>
                  </td>
                  <td className="px-4 py-3">
                    <select
                      value={order.status}
                      onChange={(e) => updateStatus.mutate({ id: order.id, status: e.target.value })}
                      className="text-xs border border-gray-300 rounded px-2 py-1"
                    >
                      {statusOptions.map((s) => <option key={s} value={s}>{s}</option>)}
                    </select>
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
