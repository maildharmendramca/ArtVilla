import { Link } from 'react-router-dom';

const collections = [
  { name: 'Copper Ware', slug: 'copper-ware', image: '/images/copper-ware.jpg', count: '50+ Products' },
  { name: 'Brass Ware', slug: 'brass-ware', image: '/images/brass-ware.jpg', count: '80+ Products' },
  { name: 'Bronze Ware', slug: 'bronze-ware', image: '/images/bronze-ware.jpg', count: '40+ Products' },
  { name: 'Home Decor', slug: 'home-decor', image: '/images/home-decor.jpg', count: '120+ Products' },
];

export default function CollectionSpotlights() {
  return (
    <section className="max-w-7xl mx-auto px-4 py-16">
      <h2 className="font-heading text-3xl text-dark text-center mb-10">Shop by Collection</h2>
      <div className="grid grid-cols-2 lg:grid-cols-4 gap-4 lg:gap-6">
        {collections.map((col) => (
          <Link
            key={col.slug}
            to={`/collections/${col.slug}`}
            className="group relative aspect-[3/4] rounded-xl overflow-hidden bg-gold-50"
          >
            <img
              src={col.image}
              alt={col.name}
              className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-500"
            />
            <div className="absolute inset-0 bg-gradient-to-t from-black/60 to-transparent" />
            <div className="absolute bottom-4 left-4">
              <h3 className="text-white font-heading text-lg font-semibold">{col.name}</h3>
              <p className="text-gold-200 text-xs mt-1">{col.count}</p>
            </div>
          </Link>
        ))}
      </div>
    </section>
  );
}
