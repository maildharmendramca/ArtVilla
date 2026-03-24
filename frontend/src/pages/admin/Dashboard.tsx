import { useQuery } from '@tanstack/react-query';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { getDashboardStats } from '@/api/admin';
import { formatCurrency, formatDate } from '@/utils/format';
import { Spinner, Badge } from '@/components/common';

export default function Dashboard() {
  const { data, isLoading } = useQuery({
    queryKey: ['admin', 'dashboard'],
    queryFn: getDashboardStats,
  });

  if (isLoading) return <Spinner className="py-32" size="lg" />;

  const stats = data?.data;
  if (!stats) return null;

  const cards = [
    { label: 'Total Revenue', value: formatCurrency(stats.totalRevenue), change: stats.revenueChange, color: 'text-copper' },
    { label: 'Total Orders', value: stats.totalOrders.toLocaleString(), change: stats.ordersChange, color: 'text-blue-600' },
    { label: 'Total Customers', value: stats.totalCustomers.toLocaleString(), change: 0, color: 'text-green-600' },
    { label: 'Total Products', value: stats.totalProducts.toLocaleString(), change: 0, color: 'text-purple-600' },
  ];

  return (
    <div className="space-y-8">
      {/* Stats cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
        {cards.map((card) => (
          <div key={card.label} className="bg-white rounded-xl p-6 shadow-sm border border-gray-100">
            <p className="text-sm text-muted mb-1">{card.label}</p>
            <p className={`text-2xl font-bold ${card.color}`}>{card.value}</p>
            {card.change !== 0 && (
              <p className={`text-xs mt-1 ${card.change > 0 ? 'text-green-600' : 'text-red-600'}`}>
                {card.change > 0 ? '+' : ''}{card.change}% from last month
              </p>
            )}
          </div>
        ))}
      </div>

      {/* Revenue chart */}
      <div className="bg-white rounded-xl p-6 shadow-sm border border-gray-100">
        <h2 className="font-heading text-xl text-dark mb-6">Revenue Overview</h2>
        <div className="h-80">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart data={stats.revenueByMonth}>
              <CartesianGrid strokeDasharray="3 3" stroke="#f0f0f0" />
              <XAxis dataKey="month" tick={{ fontSize: 12 }} />
              <YAxis tick={{ fontSize: 12 }} tickFormatter={(v) => `${(v / 1000).toFixed(0)}k`} />
              <Tooltip formatter={(value) => formatCurrency(Number(value))} />
              <Bar dataKey="revenue" fill="#b87333" radius={[4, 4, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>

      <div className="grid lg:grid-cols-2 gap-6">
        {/* Top products */}
        <div className="bg-white rounded-xl p-6 shadow-sm border border-gray-100">
          <h2 className="font-heading text-lg text-dark mb-4">Top Products</h2>
          <div className="space-y-3">
            {stats.topProducts.slice(0, 5).map((item, i) => (
              <div key={i} className="flex items-center justify-between py-2 border-b border-gray-50 last:border-0">
                <div className="flex items-center gap-3">
                  <span className="text-xs text-muted w-5">#{i + 1}</span>
                  <div>
                    <p className="text-sm font-medium text-dark">{item.product.name}</p>
                    <p className="text-xs text-muted">{item.soldCount} sold</p>
                  </div>
                </div>
                <span className="text-sm font-medium text-dark">{formatCurrency(item.revenue)}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Recent orders */}
        <div className="bg-white rounded-xl p-6 shadow-sm border border-gray-100">
          <h2 className="font-heading text-lg text-dark mb-4">Recent Orders</h2>
          <div className="space-y-3">
            {stats.recentOrders.slice(0, 5).map((order) => (
              <div key={order.id} className="flex items-center justify-between py-2 border-b border-gray-50 last:border-0">
                <div>
                  <p className="text-sm font-medium text-dark">#{order.orderNumber}</p>
                  <p className="text-xs text-muted">{formatDate(order.createdAt)}</p>
                </div>
                <div className="flex items-center gap-3">
                  <Badge variant={order.status === 'delivered' ? 'success' : order.status === 'cancelled' ? 'danger' : 'info'}>
                    {order.status}
                  </Badge>
                  <span className="text-sm font-medium">{formatCurrency(order.total)}</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
