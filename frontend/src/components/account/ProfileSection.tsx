import { useState } from 'react';
import toast from 'react-hot-toast';
import { useAuthStore } from '@/store/authStore';
import { updateProfile } from '@/api/auth';
import { Button, Input } from '@/components/common';

export default function ProfileSection() {
  const { user, setUser } = useAuthStore();
  const [name, setName] = useState(user?.name ?? '');
  const [phone, setPhone] = useState(user?.phone ?? '');
  const [saving, setSaving] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    try {
      const res = await updateProfile({ name, phone });
      setUser(res.data);
      toast.success('Profile updated successfully');
    } catch {
      toast.error('Failed to update profile');
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <h2 className="font-heading text-2xl text-dark mb-6">My Profile</h2>
      <form onSubmit={handleSubmit} className="max-w-md space-y-4">
        <Input label="Full Name" value={name} onChange={(e) => setName(e.target.value)} required />
        <Input label="Email" value={user?.email ?? ''} disabled />
        <Input label="Phone" value={phone} onChange={(e) => setPhone(e.target.value)} type="tel" />
        <Button type="submit" isLoading={saving}>Save Changes</Button>
      </form>
    </div>
  );
}
