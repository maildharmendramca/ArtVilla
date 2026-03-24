import { Helmet } from 'react-helmet-async';
import {
  HeroBanner,
  CollectionSpotlights,
  FeaturedProducts,
  WhyChooseUs,
  TrendingProducts,
  BrandStory,
  Testimonials,
} from '@/components/home';

export default function HomePage() {
  return (
    <>
      <Helmet>
        <title>Indian Art Villa | Premium Copper, Brass & Bronze Craft</title>
        <meta name="description" content="Shop premium handcrafted copper, brass, and bronze products. Authentic Indian metalware for your home and kitchen." />
      </Helmet>
      <HeroBanner />
      <CollectionSpotlights />
      <FeaturedProducts />
      <WhyChooseUs />
      <TrendingProducts />
      <BrandStory />
      <Testimonials />
    </>
  );
}
