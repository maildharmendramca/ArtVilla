import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Helmet } from 'react-helmet-async';
import toast from 'react-hot-toast';
import { useAuthStore } from '@/store/authStore';
import { Button, Input } from '@/components/common';

export default function RegisterPage() {
  const navigate = useNavigate();
  const register = useAuthStore((s) => s.register);
  const isLoading = useAuthStore((s) => s.isLoading);

  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [phone, setPhone] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await register(name, email, password, phone || undefined);
      toast.success('Account created successfully!');
      navigate('/');
    } catch {
      toast.error('Registration failed. Please try again.');
    }
  };

  return (
    <>
      <Helmet><title>Create Account | Indian Art Villa</title></Helmet>
      <div className="min-h-[70vh] flex items-center justify-center px-4">
        <div className="w-full max-w-md">
          <div className="text-center mb-8">
            <h1 className="font-heading text-3xl text-dark mb-2">Create Account</h1>
            <p className="text-muted">Join Indian Art Villa for exclusive offers</p>
          </div>
          <form onSubmit={handleSubmit} className="space-y-4">
            <Input label="Full Name" value={name} onChange={(e) => setName(e.target.value)} required />
            <Input label="Email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required />
            <Input label="Phone (optional)" type="tel" value={phone} onChange={(e) => setPhone(e.target.value)} />
            <Input label="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={6} />
            <Button type="submit" fullWidth size="lg" isLoading={isLoading}>Create Account</Button>
          </form>
          <p className="text-center text-sm text-muted mt-6">
            Already have an account?{' '}
            <Link to="/login" className="text-copper font-medium hover:text-gold-700">Sign In</Link>
          </p>
        </div>
      </div>
    </>
  );
}
