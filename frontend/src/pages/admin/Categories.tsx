import { useState } from 'react';
import { useCategoryTree } from '@/hooks/useCategories';
import { Spinner, Button, Badge } from '@/components/common';
import type { Category } from '@/types';

function CategoryNode({ category, depth = 0 }: { category: Category; depth?: number }) {
  const [expanded, setExpanded] = useState(true);
  const hasChildren = category.children && category.children.length > 0;

  return (
    <div>
      <div
        className="flex items-center justify-between py-3 px-4 hover:bg-gray-50 transition-colors"
        style={{ paddingLeft: `${depth * 24 + 16}px` }}
      >
        <div className="flex items-center gap-2">
          {hasChildren && (
            <button
              onClick={() => setExpanded(!expanded)}
              className="text-muted hover:text-dark"
            >
              <svg className={`w-4 h-4 transition-transform ${expanded ? 'rotate-90' : ''}`} fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
              </svg>
            </button>
          )}
          {!hasChildren && <span className="w-4" />}
          <span className="text-sm font-medium text-dark">{category.name}</span>
          <Badge>{category.productCount} products</Badge>
        </div>
        <div className="flex items-center gap-2">
          <Badge variant={category.isActive ? 'success' : 'danger'}>
            {category.isActive ? 'Active' : 'Inactive'}
          </Badge>
          <button className="text-copper hover:text-gold-700 text-sm">Edit</button>
        </div>
      </div>
      {hasChildren && expanded && (
        <div>
          {category.children!.map((child) => (
            <CategoryNode key={child.id} category={child} depth={depth + 1} />
          ))}
        </div>
      )}
    </div>
  );
}

export default function AdminCategories() {
  const { data, isLoading } = useCategoryTree();
  const categories = data?.data ?? [];

  return (
    <div>
      <div className="flex items-center justify-between mb-6">
        <h1 className="font-heading text-2xl text-dark">Categories</h1>
        <Button>Add Category</Button>
      </div>

      {isLoading ? (
        <Spinner className="py-16" />
      ) : (
        <div className="bg-white rounded-xl shadow-sm border border-gray-100 divide-y divide-gray-100">
          {categories.map((cat) => (
            <CategoryNode key={cat.id} category={cat} />
          ))}
        </div>
      )}
    </div>
  );
}
