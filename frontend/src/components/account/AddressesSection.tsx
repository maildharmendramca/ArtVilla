import { useState } from 'react';
import { useQuery, useQueryClient } from '@tanstack/react-query';
import toast from 'react-hot-toast';
import { getAddresses, createAddress, deleteAddress, setDefaultAddress } from '@/api/addresses';
import { Button, Input, Modal, Spinner } from '@/components/common';
import type { Address } from '@/types';

export default function AddressesSection() {
  const queryClient = useQueryClient();
  const { data, isLoading } = useQuery({ queryKey: ['addresses'], queryFn: getAddresses });
  const addresses = data?.data ?? [];
  const [showForm, setShowForm] = useState(false);

  const [form, setForm] = useState({
    fullName: '', phone: '', addressLine1: '', addressLine2: '',
    city: '', state: '', postalCode: '', country: 'India', isDefault: false,
  });

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await createAddress(form as Omit<Address, 'id'>);
      queryClient.invalidateQueries({ queryKey: ['addresses'] });
      setShowForm(false);
      toast.success('Address added');
    } catch {
      toast.error('Failed to add address');
    }
  };

  const handleDelete = async (id: string) => {
    try {
      await deleteAddress(id);
      queryClient.invalidateQueries({ queryKey: ['addresses'] });
      toast.success('Address removed');
    } catch {
      toast.error('Failed to remove address');
    }
  };

  const handleSetDefault = async (id: string) => {
    try {
      await setDefaultAddress(id);
      queryClient.invalidateQueries({ queryKey: ['addresses'] });
      toast.success('Default address updated');
    } catch {
      toast.error('Failed to set default');
    }
  };

  if (isLoading) return <Spinner className="py-16" />;

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h2 className="font-heading text-2xl text-dark">My Addresses</h2>
        <Button size="sm" onClick={() => setShowForm(true)}>Add Address</Button>
      </div>
      <div className="grid md:grid-cols-2 gap-4">
        {addresses.map((addr) => (
          <div key={addr.id} className={`border rounded-lg p-4 ${addr.isDefault ? 'border-copper' : 'border-gray-200'}`}>
            {addr.isDefault && <span className="text-xs text-copper font-medium">Default</span>}
            <p className="font-medium text-dark">{addr.fullName}</p>
            <p className="text-sm text-muted mt-1">{addr.addressLine1}</p>
            {addr.addressLine2 && <p className="text-sm text-muted">{addr.addressLine2}</p>}
            <p className="text-sm text-muted">{addr.city}, {addr.state} {addr.postalCode}</p>
            <p className="text-sm text-muted">{addr.phone}</p>
            <div className="flex gap-3 mt-3">
              {!addr.isDefault && (
                <button onClick={() => handleSetDefault(addr.id)} className="text-xs text-copper hover:text-gold-700">Set as Default</button>
              )}
              <button onClick={() => handleDelete(addr.id)} className="text-xs text-red-500 hover:text-red-700">Delete</button>
            </div>
          </div>
        ))}
      </div>

      <Modal isOpen={showForm} onClose={() => setShowForm(false)} title="Add New Address">
        <form onSubmit={handleCreate} className="space-y-3">
          <Input label="Full Name" value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} required />
          <Input label="Phone" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} required />
          <Input label="Address Line 1" value={form.addressLine1} onChange={(e) => setForm({ ...form, addressLine1: e.target.value })} required />
          <Input label="Address Line 2" value={form.addressLine2} onChange={(e) => setForm({ ...form, addressLine2: e.target.value })} />
          <div className="grid grid-cols-2 gap-3">
            <Input label="City" value={form.city} onChange={(e) => setForm({ ...form, city: e.target.value })} required />
            <Input label="State" value={form.state} onChange={(e) => setForm({ ...form, state: e.target.value })} required />
          </div>
          <Input label="Postal Code" value={form.postalCode} onChange={(e) => setForm({ ...form, postalCode: e.target.value })} required />
          <div className="flex justify-end gap-3 pt-2">
            <Button variant="ghost" onClick={() => setShowForm(false)}>Cancel</Button>
            <Button type="submit">Save Address</Button>
          </div>
        </form>
      </Modal>
    </div>
  );
}
