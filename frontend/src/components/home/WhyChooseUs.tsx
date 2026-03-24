const features = [
  {
    icon: 'M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z',
    title: '100% Authentic',
    description: 'Every product is handcrafted by skilled artisans using traditional techniques',
  },
  {
    icon: 'M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4',
    title: 'Secure Packaging',
    description: 'Premium packaging ensures your delicate metalware arrives in perfect condition',
  },
  {
    icon: 'M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15',
    title: 'Easy Returns',
    description: '30-day hassle-free return policy for complete peace of mind',
  },
  {
    icon: 'M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z',
    title: 'Expert Support',
    description: 'Dedicated support team to help you choose the perfect piece',
  },
];

export default function WhyChooseUs() {
  return (
    <section className="bg-gold-50 py-16">
      <div className="max-w-7xl mx-auto px-4">
        <h2 className="font-heading text-3xl text-dark text-center mb-12">Why Choose Indian Art Villa</h2>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
          {features.map((f) => (
            <div key={f.title} className="text-center">
              <div className="w-14 h-14 bg-copper/10 rounded-full flex items-center justify-center mx-auto mb-4">
                <svg className="w-7 h-7 text-copper" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d={f.icon} />
                </svg>
              </div>
              <h3 className="font-heading text-lg text-dark mb-2">{f.title}</h3>
              <p className="text-sm text-muted">{f.description}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
