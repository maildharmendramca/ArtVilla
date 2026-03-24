import { Link } from 'react-router-dom';
import type { Category } from '@/types';

interface MegaMenuProps {
  category: Category;
}

export default function MegaMenu({ category }: MegaMenuProps) {
  const children = category.children ?? [];
  const columns = [
    children.slice(0, Math.ceil(children.length / 3)),
    children.slice(Math.ceil(children.length / 3), Math.ceil((children.length * 2) / 3)),
    children.slice(Math.ceil((children.length * 2) / 3)),
  ].filter((col) => col.length > 0);

  return (
    <div className="absolute top-full left-1/2 -translate-x-1/2 w-[700px] bg-white rounded-xl shadow-xl border border-gray-100 p-6 z-50">
      <div className="grid grid-cols-3 gap-8">
        {columns.map((col, ci) => (
          <div key={ci} className="space-y-3">
            {col.map((child) => (
              <div key={child.id}>
                <Link
                  to={`/collections/${child.slug}`}
                  className="text-sm font-semibold text-dark hover:text-copper transition-colors"
                >
                  {child.name}
                </Link>
                {child.children && child.children.length > 0 && (
                  <ul className="mt-1 space-y-1">
                    {child.children.map((sub) => (
                      <li key={sub.id}>
                        <Link
                          to={`/collections/${sub.slug}`}
                          className="text-xs text-muted hover:text-copper transition-colors"
                        >
                          {sub.name}
                        </Link>
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            ))}
          </div>
        ))}
      </div>
      <div className="mt-6 pt-4 border-t border-gray-100">
        <Link
          to={`/collections/${category.slug}`}
          className="text-sm font-medium text-copper hover:text-gold-700 transition-colors"
        >
          View All {category.name} &rarr;
        </Link>
      </div>
    </div>
  );
}
