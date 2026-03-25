import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { useCategoryTree } from '@/hooks/useCategories';
import { createCategory, updateCategory, deleteCategory, uploadImage } from '@/api/admin';
import { Spinner, Button, Badge, Modal, Input } from '@/components/common';
import type { Category } from '@/types';

interface CategoryFormData {
  name: string;
  parentCategoryId: string;
  description: string;
  imageUrl: string;
  sortOrder: number;
  isActive: boolean;
  metaTitle: string;
  metaDescription: string;
}

const emptyForm: CategoryFormData = {
  name: '',
  parentCategoryId: '',
  description: '',
  imageUrl: '',
  sortOrder: 0,
  isActive: true,
  metaTitle: '',
  metaDescription: '',
};

// No flatten needed — parent dropdown uses only top-level categories

// ── Category Form Modal ──────────────────────────────────────

function CategoryFormModal({
  isOpen,
  onClose,
  editingCategory,
  allCategories,
}: {
  isOpen: boolean;
  onClose: () => void;
  editingCategory: Category | null;
  allCategories: Category[];
}) {
  const queryClient = useQueryClient();
  const isEdit = editingCategory !== null;

  const [form, setForm] = useState<CategoryFormData>(() =>
    editingCategory
      ? {
          name: editingCategory.name,
          parentCategoryId: editingCategory.parentId ?? '',
          description: editingCategory.description ?? '',
          imageUrl: editingCategory.image ?? '',
          sortOrder: editingCategory.sortOrder,
          isActive: editingCategory.isActive,
          metaTitle: '',
          metaDescription: '',
        }
      : { ...emptyForm },
  );

  const [errors, setErrors] = useState<Record<string, string>>({});

  // Only top-level parent categories, excluding the one being edited
  const parentList = allCategories.filter((c) => c.id !== editingCategory?.id);

  const mutation = useMutation({
    mutationFn: (data: CategoryFormData) => {
      const payload = {
        name: data.name,
        parentCategoryId: data.parentCategoryId ? Number(data.parentCategoryId) : null,
        description: data.description || null,
        imageUrl: data.imageUrl || null,
        sortOrder: data.sortOrder,
        isActive: data.isActive,
        metaTitle: data.metaTitle || null,
        metaDescription: data.metaDescription || null,
      };

      if (isEdit) {
        return updateCategory(editingCategory.id, payload);
      }
      return createCategory(payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] });
      onClose();
    },
  });

  function validate(): boolean {
    const newErrors: Record<string, string> = {};
    if (!form.name.trim()) newErrors.name = 'Category name is required.';
    if (form.name.trim().length > 100) newErrors.name = 'Name must be under 100 characters.';
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (validate()) mutation.mutate(form);
  }

  function update(field: keyof CategoryFormData, value: string | number | boolean) {
    setForm((prev) => ({ ...prev, [field]: value }));
    setErrors((prev) => ({ ...prev, [field]: '' }));
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} title={isEdit ? 'Edit Category' : 'Add Category'} size="lg">
      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Name */}
        <Input
          label="Category Name"
          placeholder="e.g. Copper Ware"
          value={form.name}
          onChange={(e) => update('name', e.target.value)}
          error={errors.name}
          required
        />

        {/* Parent Category */}
        <div className="w-full">
          <label className="block text-sm font-medium text-dark mb-1">Parent Category</label>
          <select
            value={form.parentCategoryId}
            onChange={(e) => update('parentCategoryId', e.target.value)}
            className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-dark transition-colors focus:outline-none focus:ring-2 focus:ring-copper focus:border-copper"
          >
            <option value="">None (Top Level)</option>
            {parentList.map((cat) => (
              <option key={cat.id} value={cat.id}>
                {cat.name}
              </option>
            ))}
          </select>
        </div>

        {/* Description */}
        <div className="w-full">
          <label className="block text-sm font-medium text-dark mb-1">Description</label>
          <textarea
            value={form.description}
            onChange={(e) => update('description', e.target.value)}
            placeholder="Brief description of this category"
            rows={3}
            className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-dark placeholder-muted transition-colors focus:outline-none focus:ring-2 focus:ring-copper focus:border-copper"
          />
        </div>

        {/* Image */}
        <div className="w-full">
          <label className="block text-sm font-medium text-dark mb-1">Category Image</label>
          <div className="flex items-start gap-4">
            {/* Preview */}
            <div className="w-24 h-24 rounded-lg border-2 border-dashed border-gray-300 overflow-hidden shrink-0 flex items-center justify-center bg-gray-50">
              {form.imageUrl ? (
                <img src={form.imageUrl} alt="Preview" className="w-full h-full object-cover" onError={(e) => { (e.target as HTMLImageElement).style.display = 'none'; }} />
              ) : (
                <svg className="w-8 h-8 text-gray-300" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z" />
                </svg>
              )}
            </div>
            <div className="flex-1 space-y-2">
              {/* File picker — uploads to backend */}
              <label className="inline-flex items-center gap-2 px-4 py-2 bg-gray-100 hover:bg-gray-200 text-dark text-sm font-medium rounded-lg cursor-pointer transition-colors">
                <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12" />
                </svg>
                Choose Image
                <input
                  type="file"
                  accept="image/*"
                  className="hidden"
                  onChange={async (e) => {
                    const file = e.target.files?.[0];
                    if (!file) return;
                    try {
                      const res = await uploadImage(file);
                      if (res.data?.url) update('imageUrl', res.data.url);
                    } catch {
                      // Fallback to local preview if upload fails
                      const reader = new FileReader();
                      reader.onload = () => update('imageUrl', reader.result as string);
                      reader.readAsDataURL(file);
                    }
                  }}
                />
              </label>
              <p className="text-xs text-muted">JPG, PNG, WebP — max 5MB</p>
              {form.imageUrl && (
                <button
                  type="button"
                  onClick={() => update('imageUrl', '')}
                  className="text-xs text-red-500 hover:text-red-700"
                >
                  Remove image
                </button>
              )}
            </div>
          </div>
        </div>

        {/* Sort Order + Active (side by side) */}
        <div className="grid grid-cols-2 gap-4">
          <Input
            label="Sort Order"
            type="number"
            value={String(form.sortOrder)}
            onChange={(e) => update('sortOrder', Number(e.target.value))}
          />
          <div className="flex items-end pb-1">
            <label className="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                checked={form.isActive}
                onChange={(e) => update('isActive', e.target.checked)}
                className="w-4 h-4 rounded border-gray-300 text-copper focus:ring-copper"
              />
              <span className="text-sm font-medium text-dark">Active</span>
            </label>
          </div>
        </div>

        {/* SEO Fields */}
        <details className="group">
          <summary className="cursor-pointer text-sm font-medium text-copper hover:text-gold-700 select-none">
            SEO Settings (optional)
          </summary>
          <div className="mt-3 space-y-3">
            <Input
              label="Meta Title"
              placeholder="SEO title"
              value={form.metaTitle}
              onChange={(e) => update('metaTitle', e.target.value)}
            />
            <div className="w-full">
              <label className="block text-sm font-medium text-dark mb-1">Meta Description</label>
              <textarea
                value={form.metaDescription}
                onChange={(e) => update('metaDescription', e.target.value)}
                placeholder="SEO description"
                rows={2}
                className="w-full rounded-lg border border-gray-300 px-4 py-2.5 text-dark placeholder-muted transition-colors focus:outline-none focus:ring-2 focus:ring-copper focus:border-copper"
              />
            </div>
          </div>
        </details>

        {/* Error message */}
        {mutation.isError && (
          <p className="text-sm text-red-600">
            {(mutation.error as Error)?.message || 'Something went wrong. Please try again.'}
          </p>
        )}

        {/* Actions */}
        <div className="flex justify-end gap-3 pt-2">
          <Button variant="ghost" type="button" onClick={onClose}>
            Cancel
          </Button>
          <Button type="submit" isLoading={mutation.isPending}>
            {isEdit ? 'Update Category' : 'Add Category'}
          </Button>
        </div>
      </form>
    </Modal>
  );
}

// ── Delete Confirmation Modal ────────────────────────────────

function DeleteConfirmModal({
  isOpen,
  onClose,
  category,
}: {
  isOpen: boolean;
  onClose: () => void;
  category: Category | null;
}) {
  const queryClient = useQueryClient();

  const mutation = useMutation({
    mutationFn: () => deleteCategory(category!.id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['categories'] });
      onClose();
    },
  });

  if (!category) return null;

  const hasChildren = category.children && category.children.length > 0;

  return (
    <Modal isOpen={isOpen} onClose={onClose} title="Delete Category" size="sm">
      <div className="space-y-4">
        <p className="text-sm text-muted">
          Are you sure you want to delete <strong className="text-dark">{category.name}</strong>?
        </p>
        {hasChildren && (
          <p className="text-sm text-red-600">
            This category has {category.children!.length} subcategories. They must be removed or reassigned first.
          </p>
        )}
        {category.productCount > 0 && (
          <p className="text-sm text-red-600">
            This category has {category.productCount} products. They must be reassigned first.
          </p>
        )}

        {mutation.isError && (
          <p className="text-sm text-red-600">
            {(mutation.error as Error)?.message || 'Failed to delete. Please try again.'}
          </p>
        )}

        <div className="flex justify-end gap-3 pt-2">
          <Button variant="ghost" onClick={onClose}>
            Cancel
          </Button>
          <Button
            variant="danger"
            onClick={() => mutation.mutate()}
            isLoading={mutation.isPending}
            disabled={hasChildren || category.productCount > 0}
          >
            Delete
          </Button>
        </div>
      </div>
    </Modal>
  );
}

// ── Category Tree Node ───────────────────────────────────────

function CategoryNode({
  category,
  depth = 0,
  onEdit,
  onDelete,
}: {
  category: Category;
  depth?: number;
  onEdit: (cat: Category) => void;
  onDelete: (cat: Category) => void;
}) {
  const [expanded, setExpanded] = useState(true);
  const hasChildren = category.children && category.children.length > 0;

  return (
    <div>
      <div
        className="flex items-center justify-between py-3 px-4 hover:bg-gray-50 transition-colors"
        style={{ paddingLeft: `${depth * 24 + 16}px` }}
      >
        <div className="flex items-center gap-2">
          {hasChildren ? (
            <button onClick={() => setExpanded(!expanded)} className="text-muted hover:text-dark">
              <svg
                className={`w-4 h-4 transition-transform ${expanded ? 'rotate-90' : ''}`}
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
              </svg>
            </button>
          ) : (
            <span className="w-4" />
          )}
          <span className="text-sm font-medium text-dark">{category.name}</span>
          <Badge>{category.productCount} products</Badge>
        </div>
        <div className="flex items-center gap-2">
          <Badge variant={category.isActive ? 'success' : 'danger'}>
            {category.isActive ? 'Active' : 'Inactive'}
          </Badge>
          <button
            onClick={() => onEdit(category)}
            className="text-copper hover:text-gold-700 text-sm font-medium"
          >
            Edit
          </button>
          <button
            onClick={() => onDelete(category)}
            className="text-red-500 hover:text-red-700 text-sm font-medium"
          >
            Delete
          </button>
        </div>
      </div>
      {hasChildren && expanded && (
        <div>
          {category.children!.map((child) => (
            <CategoryNode key={child.id} category={child} depth={depth + 1} onEdit={onEdit} onDelete={onDelete} />
          ))}
        </div>
      )}
    </div>
  );
}

// ── Main Page ────────────────────────────────────────────────

export default function AdminCategories() {
  const { data, isLoading } = useCategoryTree();
  const categories = data?.data ?? [];

  const [formOpen, setFormOpen] = useState(false);
  const [editingCategory, setEditingCategory] = useState<Category | null>(null);
  const [deleteTarget, setDeleteTarget] = useState<Category | null>(null);

  function handleAdd() {
    setEditingCategory(null);
    setFormOpen(true);
  }

  function handleEdit(cat: Category) {
    setEditingCategory(cat);
    setFormOpen(true);
  }

  function handleCloseForm() {
    setFormOpen(false);
    setEditingCategory(null);
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-heading text-2xl text-dark">Categories</h1>
        <Button onClick={handleAdd}>Add Category</Button>
      </div>

      {isLoading ? (
        <Spinner className="py-16" />
      ) : categories.length === 0 ? (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-12 text-center">
          <p className="text-muted mb-4">No categories yet. Create your first one!</p>
          <Button onClick={handleAdd}>Add Category</Button>
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 divide-y divide-gray-100">
          {categories.map((cat) => (
            <CategoryNode key={cat.id} category={cat} onEdit={handleEdit} onDelete={setDeleteTarget} />
          ))}
        </div>
      )}

      {/* Add / Edit Modal */}
      {formOpen && (
        <CategoryFormModal
          isOpen={formOpen}
          onClose={handleCloseForm}
          editingCategory={editingCategory}
          allCategories={categories}
        />
      )}

      {/* Delete Confirmation Modal */}
      <DeleteConfirmModal
        isOpen={deleteTarget !== null}
        onClose={() => setDeleteTarget(null)}
        category={deleteTarget}
      />
    </div>
  );
}
