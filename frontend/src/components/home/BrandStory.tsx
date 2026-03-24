export default function BrandStory() {
  return (
    <section className="max-w-7xl mx-auto px-4 py-16">
      <div className="grid lg:grid-cols-2 gap-12 items-center">
        <div>
          <h2 className="font-heading text-3xl lg:text-4xl text-dark mb-6">
            A Legacy of Indian Metalcraft
          </h2>
          <p className="text-muted leading-relaxed mb-4">
            Indian Art Villa carries forward a centuries-old tradition of metal craftsmanship
            from the heart of Moradabad, India&apos;s &ldquo;Brass City.&rdquo; Each piece in
            our collection is handcrafted by master artisans whose skills have been passed down
            through generations.
          </p>
          <p className="text-muted leading-relaxed mb-6">
            From pure copper water bottles that promote wellness to ornate brass temple
            accessories, every item tells a story of dedication, precision, and artistic
            excellence. We bridge the gap between traditional Indian artistry and modern homes
            worldwide.
          </p>
          <div className="flex gap-8">
            <div>
              <p className="font-heading text-3xl text-copper font-bold">500+</p>
              <p className="text-sm text-muted">Artisan Partners</p>
            </div>
            <div>
              <p className="font-heading text-3xl text-copper font-bold">10K+</p>
              <p className="text-sm text-muted">Products Crafted</p>
            </div>
            <div>
              <p className="font-heading text-3xl text-copper font-bold">50K+</p>
              <p className="text-sm text-muted">Happy Customers</p>
            </div>
          </div>
        </div>
        <div className="relative">
          <div className="aspect-[4/5] rounded-xl overflow-hidden bg-gold-100">
            <img
              src="/images/artisan-craft.jpg"
              alt="Artisan crafting copper vessel"
              className="w-full h-full object-cover"
            />
          </div>
          <div className="absolute -bottom-4 -left-4 w-32 h-32 bg-copper/10 rounded-xl -z-10" />
        </div>
      </div>
    </section>
  );
}
