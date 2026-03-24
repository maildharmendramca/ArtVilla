import { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import toast from 'react-hot-toast';
import { useAuthStore } from '@/store/authStore';
import { Button, Input } from '@/components/common';

export default function LoginPage() {
  const navigate = useNavigate();
  const location = useLocation();
  const login = useAuthStore((s) => s.login);
  const isLoading = useAuthStore((s) => s.isLoading);

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const from = (location.state as { from?: string })?.from || '/';

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await login(email, password);
      toast.success('Welcome back!');
      navigate(from, { replace: true });
    } catch {
      toast.error('Invalid email or password');
    }
  };

  return (
    <>
      <Helmet><title>Sign In | Indian Art Villa</title></Helmet>
      <div className="min-h-[70vh] flex items-center justify-center px-4">
        <div className="w-full max-w-md">
          <div className="text-center mb-8">
            <h1 className="font-heading text-3xl text-dark mb-2">Welcome Back</h1>
            <p className="text-muted">Sign in to your Indian Art Villa account</p>
          </div>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Input label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
            <Input label="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
            <div className="flex items-center justify-between text-sm">
              <label className="flex items-center gap-2 text-muted">
                <input type="checkbox" className="rounded border-gray-300 text-copper focus:ring-copper" />
                Remember me
              </label>
              <a href="#" className="text-copper hover:text-gold-700">Forgot password?</a>
            </div>
            <Button type="submit" fullWidth size="lg" isLoading={isLoading}>Sign In</Button>
          </form>
          <p className="text-center text-sm text-muted mt-6">
            Don&apos;t have an account?{' '}
            <Link to="/register" className="text-copper font-medium hover:text-gold-700">Create Account</Link>
          </p>
        </div>
      </div>
    </>
  );
}
