import { NavLink, Outlet, Navigate } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import { useAuthStore } from '@/store/authStore';
import { Button, Breadcrumb } from '@/components/common';

const tabs = [
  { label: 'Profile', to: '/account', end: true },
  { label: 'Orders', to: '/account/orders' },
  { label: 'Addresses', to: '/account/addresses' },
  { label: 'Wishlist', to: '/account/wishlist' },
];

export default function AccountPage() {
  const { isAuthenticated, logout } = useAuthStore();

  if (!isAuthenticated) return <Navigate to="/login" replace />;

  return (
    <>
      <Helmet><title>My Account | Indian Art Villa</title></Helmet>
      <div className="max-w-7xl mx-auto px-4 pb-16">
        <Breadcrumb items={[{ label: 'My Account' }]} />
        <div className="flex items-center justify-between mb-8">
          <h1 className="font-heading text-3xl text-dark">My Account</h1>
          <Button variant="ghost" size="sm" onClick={logout}>Sign Out</Button>
        </div>
        <div className="flex flex-col lg:flex-row gap-8">
          <nav className="flex lg:flex-col gap-2 overflow-x-auto lg:w-48 shrink-0">
            {tabs.map((tab) => (
              <NavLink
                key={tab.to}
                to={tab.to}
                end={tab.end}
                className={({ isActive }) =>
                  `px-4 py-2 rounded-lg text-sm font-medium whitespace-nowrap transition-colors ${
                    isActive ? 'bg-copper text-white' : 'text-dark hover:bg-gold-50'
                  }`
                }
              >
                {tab.label}
              </NavLink>
            ))}
          </nav>
          <div className="flex-1 min-w-0">
            <Outlet />
          </div>
        </div>
      </div>
    </>
  );
}
