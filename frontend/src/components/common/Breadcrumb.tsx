import { Link } from 'react-router-dom';

export interface BreadcrumbItem {
  label: string;
  href?: string;
}

interface BreadcrumbProps {
  items: BreadcrumbItem[];
}

export default function Breadcrumb({ items }: BreadcrumbProps) {
  return (
    <nav className="flex items-center gap-2 text-sm text-muted py-4">
      <Link to="/" className="hover:text-copper transition-colors">Home</Link>
      {items.map((item, index) => (
        <span key={index} className="flex items-center gap-2">
          <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M9 5l7 7-7 7" />
          </svg>
          {item.href ? (
            <Link to={item.href} className="hover:text-copper transition-colors">{item.label}</Link>
          ) : (
            <span className="text-dark font-medium">{item.label}</span>
          )}
        </span>
      ))}
    </nav>
  );
}
