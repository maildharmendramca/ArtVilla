import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { getCustomers, toggleCustomerActive } from '@/api/admin';
import { formatDate } from '@/utils/format';
import { Spinner, Badge } from '@/components/common';

export default function AdminCustomers() {
  const queryClient = useQueryClient();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState('');

  const { data, isLoading } = useQuery({
    queryKey: ['admin', 'customers', page, search],
    queryFn: () => getCustomers(page, 20, search || undefined),
  });

  const toggleActive = useMutation({
    mutationFn: (id: string) => toggleCustomerActive(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin', 'customers'] });
      toast.success('Customer status updated');
    },
    onError: () => toast.error('Failed to update status'),
  });

  const customers = data?.data ?? [];
  const pagination = data?.pagination;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-heading text-2xl text-dark">Customers</h1>
      </div>

      <div className="mb-4">
        <input
          type="text"
          value={search}
          onChange={(e) => { setSearch(e.target.value); setPage(1); }}
          placeholder="Search by name or email..."
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
                <th className="px-4 py-3 text-muted font-medium">Customer</th>
                <th className="px-4 py-3 text-muted font-medium">Email</th>
                <th className="px-4 py-3 text-muted font-medium">Phone</th>
                <th className="px-4 py-3 text-muted font-medium">Joined</th>
                <th className="px-4 py-3 text-muted font-medium">Status</th>
                <th className="px-4 py-3 text-muted font-medium">Action</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {customers.map((customer: any) => (
                <tr key={customer.id} className="hover:bg-gray-50">
                  <td className="px-4 py-3">
                    <div className="flex items-center gap-3">
                      <div className="w-8 h-8 rounded-full bg-copper text-white flex items-center justify-center text-xs font-medium shrink-0">
                        {customer.name?.charAt(0).toUpperCase() ?? '?'}
                      </div>
                      <span className="font-medium text-dark">{customer.name}</span>
                    </div>
                  </td>
                  <td className="px-4 py-3 text-muted">{customer.email}</td>
                  <td className="px-4 py-3 text-muted">{customer.phone || '-'}</td>
                  <td className="px-4 py-3 text-muted">{formatDate(customer.createdAt)}</td>
                  <td className="px-4 py-3">
                    <Badge variant={customer.isActive ? 'success' : 'danger'}>
                      {customer.isActive ? 'Active' : 'Inactive'}
                    </Badge>
                  </td>
                  <td className="px-4 py-3">
                    <button
                      onClick={() => toggleActive.mutate(customer.id)}
                      disabled={toggleActive.isPending}
                      className={`text-xs font-medium px-3 py-1.5 rounded-lg border transition-colors ${
                        customer.isActive
                          ? 'border-red-200 text-red-600 hover:bg-red-50'
                          : 'border-green-200 text-green-600 hover:bg-green-50'
                      } disabled:opacity-50`}
                    >
                      {customer.isActive ? 'Deactivate' : 'Activate'}
                    </button>
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
