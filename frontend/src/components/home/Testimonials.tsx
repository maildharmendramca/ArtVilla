import { useState, useEffect } from 'react';
import { Rating } from '@/components/common';

const testimonials = [
  {
    name: 'Priya Sharma',
    location: 'Mumbai',
    rating: 5,
    text: 'The copper water jug I purchased is absolutely beautiful. The craftsmanship is exceptional and it keeps water fresh and cool.',
  },
  {
    name: 'Rajesh Kumar',
    location: 'Delhi',
    rating: 5,
    text: 'Bought a brass pooja thali set. The intricate detailing is remarkable. Truly a piece of art. Delivery was prompt and packaging was excellent.',
  },
  {
    name: 'Anita Desai',
    location: 'Bangalore',
    rating: 4,
    text: 'Love my bronze Ganesha statue! It is a centerpiece of my living room. The patina finish gives it an antique charm.',
  },
  {
    name: 'Vikram Patel',
    location: 'Ahmedabad',
    rating: 5,
    text: 'Ordered a complete copper dinner set. Every piece is handmade with such precision. Indian Art Villa never disappoints.',
  },
];

export default function Testimonials() {
  const [current, setCurrent] = useState(0);

  useEffect(() => {
    const timer = setInterval(() => setCurrent((c) => (c + 1) % testimonials.length), 4000);
    return () => clearInterval(timer);
  }, []);

  return (
    <section className="bg-gold-50 py-16">
      <div className="max-w-3xl mx-auto px-4 text-center">
        <h2 className="font-heading text-3xl text-dark mb-10">What Our Customers Say</h2>
        <div className="relative min-h-[200px]">
          {testimonials.map((t, i) => (
            <div
              key={i}
              className={`absolute inset-0 transition-opacity duration-500 ${
                i === current ? 'opacity-100' : 'opacity-0 pointer-events-none'
              }`}
            >
              <div className="flex justify-center mb-4">
                <Rating value={t.rating} size="md" />
              </div>
              <p className="text-lg text-dark italic mb-6 leading-relaxed">&ldquo;{t.text}&rdquo;</p>
              <p className="font-medium text-dark">{t.name}</p>
              <p className="text-sm text-muted">{t.location}</p>
            </div>
          ))}
        </div>
        <div className="flex justify-center gap-2 mt-8">
          {testimonials.map((_, i) => (
            <button
              key={i}
              onClick={() => setCurrent(i)}
              className={`w-2.5 h-2.5 rounded-full transition-colors ${
                i === current ? 'bg-copper' : 'bg-gray-300'
              }`}
            />
          ))}
        </div>
      </div>
    </section>
  );
}
