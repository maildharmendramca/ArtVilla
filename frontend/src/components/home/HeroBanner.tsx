import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';

const slides = [
  {
    title: 'Timeless Copper Craftsmanship',
    subtitle: 'Discover handcrafted copper vessels that blend tradition with modern elegance',
    cta: 'Shop Copper Collection',
    link: '/collections/copper',
    bgColor: 'from-amber-900 to-amber-800',
  },
  {
    title: 'Brass Heritage Collection',
    subtitle: 'Each piece a testament to the artistry of Indian metalwork',
    cta: 'Explore Brass',
    link: '/collections/brass',
    bgColor: 'from-yellow-900 to-yellow-800',
  },
  {
    title: 'Exclusive Bronze Art',
    subtitle: 'Museum-quality bronze sculptures and decor for the discerning collector',
    cta: 'View Bronze Art',
    link: '/collections/bronze',
    bgColor: 'from-stone-800 to-stone-700',
  },
];

export default function HeroBanner() {
  const [current, setCurrent] = useState(0);

  useEffect(() => {
    const timer = setInterval(() => setCurrent((c) => (c + 1) % slides.length), 5000);
    return () => clearInterval(timer);
  }, []);

  const slide = slides[current];

  return (
    <section className={`relative bg-gradient-to-r ${slide.bgColor} transition-colors duration-700`}>
      <div className="max-w-7xl mx-auto px-4 py-20 lg:py-32">
        <div className="max-w-2xl">
          <h1 className="font-heading text-4xl lg:text-6xl text-white font-bold leading-tight mb-6">
            {slide.title}
          </h1>
          <p className="text-lg text-gold-200 mb-8">{slide.subtitle}</p>
          <Link
            to={slide.link}
            className="inline-block bg-copper text-white px-8 py-3 rounded-lg font-medium hover:bg-gold-700 transition-colors"
          >
            {slide.cta}
          </Link>
        </div>
      </div>

      {/* Dots */}
      <div className="absolute bottom-6 left-1/2 -translate-x-1/2 flex gap-2">
        {slides.map((_, i) => (
          <button
            key={i}
            onClick={() => setCurrent(i)}
            className={`w-3 h-3 rounded-full transition-colors ${
              i === current ? 'bg-copper' : 'bg-white/40'
            }`}
          />
        ))}
      </div>
    </section>
  );
}
