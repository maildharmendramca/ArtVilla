import { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useCategoryTree } from '@/hooks/useCategories';
import { createProduct, updateProduct, getAdminProduct, uploadImage } from '@/api/admin';
import { Button, Input, Spinner } from '@/components/common';
import type { Category } from '@/types';

// ── Types ────────────────────────────────────────────────────

interface VariantFormData {
  name: string;
  sku: string;
  priceAdjustment: number;
  stockQuantity: number;
}

interface ImageEntry {
  url: string;
  alt: string;
  isPrimary: boolean;
}

interface ProductFormData {
  name: string;
  sku: string;
  description: string;
  shortDescription: string;
  price: number;
  compareAtPrice: number;
  stockQuantity: number;
  weightGrams: number;
  dimensions: string;
  material: string;
  finish: string;
  capacity: string;
  categoryId: string;
  isFeatured: boolean;
  isGiftable: boolean;
  isCustomizable: boolean;
  isActive: boolean;
  tags: string;
  metaTitle: string;
  metaDescription: string;
  images: ImageEntry[];
  variants: VariantFormData[];
}

const emptyVariant: VariantFormData = { name: '', sku: '', priceAdjustment: 0, stockQuantity: 0 };

const emptyForm: ProductFormData = {
  name: '',
  sku: '',
  description: '',
  shortDescription: '',
  price: 0,
  compareAtPrice: 0,
  stockQuantity: 0,
  weightGrams: 0,
  dimensions: '',
  material: '',
  finish: '',
  capacity: '',
  categoryId: '',
  isFeatured: false,
  isGiftable: false,
  isCustomizable: false,
  isActive: true,
  tags: '',
  metaTitle: '',
  metaDescription: '',
  images: [],
  variants: [],
};

// ── Helpers ──────────────────────────────────────────────────

// Extract subcategories grouped by their parent
function getSubcategories(categories: Category[]): { parent: string; subs: { id: string; name: string }[] }[] {
  return categories
    .filter((cat) => cat.children?.length)
    .map((cat) => ({
      parent: cat.name,
      subs: cat.children!.map((sub) => ({ id: sub.id, name: sub.name })),
    }));
}

// ── Component ────────────────────────────────────────────────

export default function ProductForm() {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);

  const { data: categoryData, isLoading: categoriesLoading } = useCategoryTree();
  const categories = categoryData?.data ?? [];
  const subcategories = getSubcategories(categories);

  // Load existing product for edit mode
  const { data: existingRes, isLoading: productLoading } = useQuery({
    queryKey: ['admin', 'product-edit', id],
    queryFn: () => getAdminProduct(id!),
    enabled: isEdit,
  });
  const existingData = existingRes?.data ?? null;

  const [form, setForm] = useState<ProductFormData>({ ...emptyForm });
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Populate form when editing
  useEffect(() => {
    if (isEdit && existingData) {
      setForm({
        name: existingData.name,
        sku: existingData.sku ?? existingData.slug,
        description: existingData.description ?? '',
        shortDescription: existingData.shortDescription ?? '',
        price: existingData.basePrice ?? existingData.price ?? 0,
        compareAtPrice: existingData.compareAtPrice ?? 0,
        stockQuantity: existingData.stockQuantity ?? 0,
        weightGrams: existingData.weightGrams ?? 0,
        dimensions: existingData.dimensions ?? '',
        material: existingData.material ?? '',
        finish: existingData.finish ?? '',
        capacity: existingData.capacity ?? '',
        categoryId: String(existingData.categoryId ?? existingData.categoryIdValue ?? ''),
        isFeatured: existingData.isFeatured ?? false,
        isGiftable: existingData.isGiftable ?? false,
        isCustomizable: existingData.isCustomizable ?? false,
        isActive: existingData.isActive ?? true,
        tags: existingData.tags?.join(', ') ?? '',
        metaTitle: existingData.metaTitle ?? '',
        metaDescription: existingData.metaDescription ?? '',
        images: existingData.images?.map((img: any) => ({
          url: img.url,
          alt: img.alt ?? img.altText ?? '',
          isPrimary: img.isPrimary ?? false,
        })) ?? [],
        variants: existingData.variants?.map((v: any) => ({
          name: v.name,
          sku: v.sku ?? '',
          priceAdjustment: v.priceAdjustment ?? 0,
          stockQuantity: v.stock ?? v.stockQuantity ?? 0,
        })) ?? [],
      });
    }
  }, [isEdit, existingData]);

  // ── Mutations ────────────────────────────────────────────

  const saveMutation = useMutation({
    mutationFn: (data: ProductFormData) => {
      const payload = {
        name: data.name,
        sku: data.sku,
        description: data.description,
        shortDescription: data.shortDescription || null,
        price: data.price,
        compareAtPrice: data.compareAtPrice || null,
        stockQuantity: data.stockQuantity,
        weightGrams: data.weightGrams,
        dimensions: data.dimensions || null,
        material: data.material || null,
        finish: data.finish || null,
        capacity: data.capacity || null,
        categoryId: Number(data.categoryId),
        isFeatured: data.isFeatured,
        isGiftable: data.isGiftable,
        isCustomizable: data.isCustomizable,
        isActive: data.isActive,
        metaTitle: data.metaTitle || null,
        metaDescription: data.metaDescription || null,
        tags: data.tags ? data.tags.split(',').map((t) => t.trim()).filter(Boolean) : [],
        variants: data.variants.length
          ? data.variants.map((v) => ({
              name: v.name,
              sku: v.sku || null,
              priceAdjustment: v.priceAdjustment,
              stockQuantity: v.stockQuantity,
            }))
          : null,
        images: data.images.length
          ? data.images.map((img, i) => ({
              url: img.url,
              alt: img.alt || null,
              isPrimary: img.isPrimary,
              sortOrder: i + 1,
            }))
          : null,
      };

      if (isEdit) return updateProduct(id!, payload);
      return createProduct(payload);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin', 'products'] });
      navigate('/admin/products');
    },
  });

  // ── Validation ───────────────────────────────────────────

  function validate(): boolean {
    const e: Record<string, string> = {};
    if (!form.name.trim()) e.name = 'Product name is required.';
    if (!form.sku.trim()) e.sku = 'SKU is required.';
    if (!form.description.trim()) e.description = 'Description is required.';
    if (form.price <= 0) e.price = 'Price must be greater than 0.';
    if (!form.categoryId) e.categoryId = 'Please select a category.';
    if (form.stockQuantity < 0) e.stockQuantity = 'Stock cannot be negative.';
    setErrors(e);
    return Object.keys(e).length === 0;
  }

  function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (validate()) saveMutation.mutate(form);
  }

  function update(field: keyof ProductFormData, value: string | number | boolean) {
    setForm((prev) => ({ ...prev, [field]: value }));
    setErrors((prev) => ({ ...prev, [field]: '' }));
  }

  // ── Image helpers ───────────────────────────────────────

  const [imageUploading, setImageUploading] = useState(false);

  async function handleImageUpload(e: React.ChangeEvent<HTMLInputElement>) {
    const files = e.target.files;
    if (!files?.length) return;

    setImageUploading(true);
    try {
      const newImages: ImageEntry[] = [];
      for (const file of Array.from(files)) {
        const res = await uploadImage(file);
        if (res.data?.url) {
          newImages.push({
            url: res.data.url,
            alt: file.name.replace(/\.[^.]+$/, ''),
            isPrimary: form.images.length === 0 && newImages.length === 0,
          });
        }
      }
      setForm((prev) => ({ ...prev, images: [...prev.images, ...newImages] }));
    } catch {
      // silent fail
    }
    setImageUploading(false);
    e.target.value = '';
  }

  function removeImage(index: number) {
    setForm((prev) => {
      const images = prev.images.filter((_, i) => i !== index);
      // If removed image was primary, make first one primary
      if (images.length > 0 && !images.some((img) => img.isPrimary)) {
        images[0].isPrimary = true;
      }
      return { ...prev, images };
    });
  }

  function setPrimaryImage(index: number) {
    setForm((prev) => ({
      ...prev,
      images: prev.images.map((img, i) => ({ ...img, isPrimary: i === index })),
    }));
  }

  // ── Variant helpers ──────────────────────────────────────

  function addVariant() {
    setForm((prev) => ({ ...prev, variants: [...prev.variants, { ...emptyVariant }] }));
  }

  function updateVariant(index: number, field: keyof VariantFormData, value: string | number) {
    setForm((prev) => {
      const variants = [...prev.variants];
      variants[index] = { ...variants[index], [field]: value };
      return { ...prev, variants };
    });
  }

  function removeVariant(index: number) {
    setForm((prev) => ({ ...prev, variants: prev.variants.filter((_, i) => i !== index) }));
  }

  // ── Loading state ────────────────────────────────────────

  if (categoriesLoading || (isEdit && productLoading)) {
    return <Spinner className="py-16" />;
  }

  // ── Render ───────────────────────────────────────────────

  return (
    <div className="max-w-4xl">
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-heading text-2xl text-dark">
          {isEdit ? 'Edit Product' : 'Add Product'}
        </h1>
        <Button variant="ghost" onClick={() => navigate('/admin/products')}>
          Back to Products
        </Button>
      </div>

      <form onSubmit={handleSubmit} className="space-y-8">
        {/* ── Basic Info ─────────────────────────────────── */}
        <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h2 className="text-lg font-semibold text-dark mb-4">Basic Information</h2>
          <div className="space-y-4">
            <div className="grid grid-cols-2 gap-4">
              <Input
                label="Product Name"
                placeholder="e.g. Pure Copper Water Bottle"
                value={form.name}
                onChange={(e) => update('name', e.target.value)}
                error={errors.name}
                required
              />
              <Input
                label="SKU"
                placeholder="e.g. COP-BTL-001"
                value={form.sku}
                onChange={(e) => update('sku', e.target.value)}
                error={errors.sku}
                required
              />
            </div>

            <div className="w-full">
              <label className="block text-sm font-medium text-dark mb-1">Description</label>
              <textarea
                value={form.description}
                onChange={(e) => update('description', e.target.value)}
                placeholder="Full product description"
                rows={4}
                className={`w-full rounded-lg border px-4 py-2.5 text-dark placeholder-muted transition-colors focus:outline-none focus:ring-2 focus:ring-copper focus:border-copper ${errors.description ? 'border-red-500' : 'border-gray-300'}`}
              />
              {errors.description && <p className="mt-1 text-sm text-red-600">{errors.description}</p>}
            </div>

            <Input
              label="Short Description"
              placeholder="One-line summary"
              value={form.shortDescription}
              onChange={(e) => update('shortDescription', e.target.value)}
            />
          </div>
        </section>

        {/* ── Images ─────────────────────────────────────── */}
        <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h2 className="text-lg font-semibold text-dark mb-4">Product Images</h2>

          {/* Image grid */}
          <div className="flex flex-wrap gap-4 mb-4">
            {form.images.map((img, i) => (
              <div key={i} className="relative group w-28 h-28 rounded-lg border-2 border-gray-200 overflow-hidden">
                <img src={img.url} alt={img.alt} className="w-full h-full object-cover" />
                {img.isPrimary && (
                  <span className="absolute top-1 left-1 bg-copper text-white text-[10px] font-bold px-1.5 py-0.5 rounded">
                    Primary
                  </span>
                )}
                {/* Hover overlay */}
                <div className="absolute inset-0 bg-black/50 opacity-0 group-hover:opacity-100 transition-opacity flex flex-col items-center justify-center gap-1">
                  {!img.isPrimary && (
                    <button
                      type="button"
                      onClick={() => setPrimaryImage(i)}
                      className="text-white text-xs hover:text-copper"
                    >
                      Set Primary
                    </button>
                  )}
                  <button
                    type="button"
                    onClick={() => removeImage(i)}
                    className="text-red-400 text-xs hover:text-red-300"
                  >
                    Remove
                  </button>
                </div>
              </div>
            ))}

            {/* Upload button */}
            <label className={`w-28 h-28 rounded-lg border-2 border-dashed border-gray-300 flex flex-col items-center justify-center cursor-pointer hover:border-copper hover:bg-gold-50 transition-colors ${imageUploading ? 'opacity-50 pointer-events-none' : ''}`}>
              {imageUploading ? (
                <svg className="animate-spin w-6 h-6 text-copper" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z" />
                </svg>
              ) : (
                <>
                  <svg className="w-6 h-6 text-gray-400" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                  </svg>
                  <span className="text-xs text-muted mt-1">Add Image</span>
                </>
              )}
              <input
                type="file"
                accept="image/*"
                multiple
                className="hidden"
                onChange={handleImageUpload}
              />
            </label>
          </div>

          <p className="text-xs text-muted">Upload product images. First image is set as primary. Hover to change or remove. JPG, PNG, WebP — max 5MB each.</p>
        </section>

        {/* ── Category ───────────────────────────────────── */}
        <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h2 className="text-lg font-semibold text-dark mb-4">Category</h2>
          <div className="w-full">
            <label className="block text-sm font-medium text-dark mb-1">Select Category</label>
            <select
              value={form.categoryId}
              onChange={(e) => update('categoryId', e.target.value)}
              className={`w-full rounded-lg border px-4 py-2.5 text-dark transition-colors focus:outline-none focus:ring-2 focus:ring-copper focus:border-copper ${errors.categoryId ? 'border-red-500' : 'border-gray-300'}`}
            >
              <option value="">-- Select a category --</option>
              {subcategories.map((group) => (
                <optgroup key={group.parent} label={group.parent}>
                  {group.subs.map((sub) => (
                    <option key={sub.id} value={sub.id}>
                      {sub.name}
                    </option>
                  ))}
                </optgroup>
              ))}
            </select>
            {errors.categoryId && <p className="mt-1 text-sm text-red-600">{errors.categoryId}</p>}
          </div>
        </section>

        {/* ── Pricing & Stock ────────────────────────────── */}
        <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h2 className="text-lg font-semibold text-dark mb-4">Pricing & Stock</h2>
          <div className="grid grid-cols-3 gap-4">
            <Input
              label="Price (INR)"
              type="number"
              min="0"
              step="0.01"
              value={String(form.price)}
              onChange={(e) => update('price', Number(e.target.value))}
              error={errors.price}
              required
            />
            <Input
              label="Compare at Price"
              type="number"
              min="0"
              step="0.01"
              value={String(form.compareAtPrice)}
              onChange={(e) => update('compareAtPrice', Number(e.target.value))}
              helperText="Original MRP for showing discount"
            />
            <Input
              label="Stock Quantity"
              type="number"
              min="0"
              value={String(form.stockQuantity)}
              onChange={(e) => update('stockQuantity', Number(e.target.value))}
              error={errors.stockQuantity}
            />
          </div>
        </section>

        {/* ── Specifications ─────────────────────────────── */}
        <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h2 className="text-lg font-semibold text-dark mb-4">Specifications</h2>
          <div className="grid grid-cols-2 gap-4">
            <Input
              label="Material"
              placeholder="e.g. Pure Copper"
              value={form.material}
              onChange={(e) => update('material', e.target.value)}
            />
            <Input
              label="Finish"
              placeholder="e.g. Hammered"
              value={form.finish}
              onChange={(e) => update('finish', e.target.value)}
            />
            <Input
              label="Capacity"
              placeholder="e.g. 1 Litre"
              value={form.capacity}
              onChange={(e) => update('capacity', e.target.value)}
            />
            <Input
              label="Weight (grams)"
              type="number"
              min="0"
              value={String(form.weightGrams)}
              onChange={(e) => update('weightGrams', Number(e.target.value))}
            />
            <div className="col-span-2">
              <Input
                label="Dimensions"
                placeholder="e.g. 12 x 4 x 3 inches"
                value={form.dimensions}
                onChange={(e) => update('dimensions', e.target.value)}
              />
            </div>
          </div>
        </section>

        {/* ── Variants ───────────────────────────────────── */}
        <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-dark">Variants</h2>
            <Button type="button" variant="outline" size="sm" onClick={addVariant}>
              Add Variant
            </Button>
          </div>

          {form.variants.length === 0 ? (
            <p className="text-sm text-muted">No variants. A default variant will be created automatically.</p>
          ) : (
            <div className="space-y-3">
              {form.variants.map((v, i) => (
                <div key={i} className="flex items-end gap-3 p-3 bg-gray-50 rounded-lg">
                  <Input
                    label="Variant Name"
                    placeholder="e.g. Large"
                    value={v.name}
                    onChange={(e) => updateVariant(i, 'name', e.target.value)}
                  />
                  <Input
                    label="SKU"
                    placeholder="Optional"
                    value={v.sku}
                    onChange={(e) => updateVariant(i, 'sku', e.target.value)}
                  />
                  <Input
                    label="Price +/-"
                    type="number"
                    value={String(v.priceAdjustment)}
                    onChange={(e) => updateVariant(i, 'priceAdjustment', Number(e.target.value))}
                  />
                  <Input
                    label="Stock"
                    type="number"
                    min="0"
                    value={String(v.stockQuantity)}
                    onChange={(e) => updateVariant(i, 'stockQuantity', Number(e.target.value))}
                  />
                  <button
                    type="button"
                    onClick={() => removeVariant(i)}
                    className="mb-1 text-red-500 hover:text-red-700 text-sm font-medium shrink-0"
                  >
                    Remove
                  </button>
                </div>
              ))}
            </div>
          )}
        </section>

        {/* ── Tags & Flags ───────────────────────────────── */}
        <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <h2 className="text-lg font-semibold text-dark mb-4">Tags & Options</h2>
          <Input
            label="Tags"
            placeholder="Comma-separated e.g. Gift, Featured, Copper"
            value={form.tags}
            onChange={(e) => update('tags', e.target.value)}
            helperText="Separate tags with commas"
          />
          <div className="flex flex-wrap gap-6 mt-4">
            {(
              [
                ['isFeatured', 'Featured Product'],
                ['isGiftable', 'Giftable'],
                ['isCustomizable', 'Customizable'],
                ['isActive', 'Active'],
              ] as const
            ).map(([field, label]) => (
              <label key={field} className="flex items-center gap-2 cursor-pointer">
                <input
                  type="checkbox"
                  checked={form[field]}
                  onChange={(e) => update(field, e.target.checked)}
                  className="w-4 h-4 rounded border-gray-300 text-copper focus:ring-copper"
                />
                <span className="text-sm font-medium text-dark">{label}</span>
              </label>
            ))}
          </div>
        </section>

        {/* ── SEO ────────────────────────────────────────── */}
        <section className="bg-white rounded-xl shadow-sm border border-gray-100 p-6">
          <details className="group">
            <summary className="cursor-pointer text-lg font-semibold text-dark select-none">
              SEO Settings (optional)
            </summary>
            <div className="mt-4 space-y-4">
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
        </section>

        {/* ── Error ──────────────────────────────────────── */}
        {saveMutation.isError && (
          <div className="bg-red-50 border border-red-200 rounded-lg p-4">
            <p className="text-sm text-red-700">
              {(saveMutation.error as Error)?.message || 'Failed to save product. Please try again.'}
            </p>
          </div>
        )}

        {/* ── Actions ────────────────────────────────────── */}
        <div className="flex justify-end gap-3">
          <Button variant="ghost" type="button" onClick={() => navigate('/admin/products')}>
            Cancel
          </Button>
          <Button type="submit" size="lg" isLoading={saveMutation.isPending}>
            {isEdit ? 'Update Product' : 'Create Product'}
          </Button>
        </div>
      </form>
    </div>
  );
}
