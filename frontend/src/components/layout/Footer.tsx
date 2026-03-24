import { useState } from 'react';
import { Link } from 'react-router-dom';

export default function Footer() {
  const [email, setEmail] = useState('');

  const handleNewsletter = (e: React.FormEvent) => {
    e.preventDefault();
    // TODO: newsletter subscription
    setEmail('');
  };

  return (
    <footer className="bg-dark text-gray-300">
      {/* Newsletter */}
      <div className="bg-gold-900 py-10">
        <div className="max-w-7xl mx-auto px-4 text-center">
          <h3 className="font-heading text-2xl text-white mb-2">Join Our Community</h3>
          <p className="text-gold-200 mb-6 text-sm">Get exclusive offers, new arrival updates, and artisan stories.</p>
          <form onSubmit={handleNewsletter} className="flex max-w-md mx-auto gap-2">
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="Enter your email"
              required
              className="flex-1 rounded-lg px-4 py-2.5 text-sm text-dark focus:outline-none focus:ring-2 focus:ring-copper"
            />
            <button type="submit" className="bg-copper text-white px-6 py-2.5 rounded-lg text-sm font-medium hover:bg-gold-700 transition-colors">
              Subscribe
            </button>
          </form>
        </div>
      </div>

      <div className="max-w-7xl mx-auto px-4 py-12">
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-8">
          {/* Brand */}
          <div>
            <h4 className="font-heading text-xl text-white mb-4">Indian Art Villa</h4>
            <p className="text-sm leading-relaxed mb-4">
              Premium handcrafted copper, brass, and bronze products. Each piece tells a story of Indian artisanship passed down through generations.
            </p>
          </div>

          {/* Quick Links */}
          <div>
            <h5 className="text-white font-medium mb-4 text-sm uppercase tracking-wider">Quick Links</h5>
            <ul className="space-y-2 text-sm">
              <li><Link to="/collections/copper" className="hover:text-copper transition-colors">Copper Products</Link></li>
              <li><Link to="/collections/brass" className="hover:text-copper transition-colors">Brass Products</Link></li>
              <li><Link to="/collections/bronze" className="hover:text-copper transition-colors">Bronze Products</Link></li>
              <li><Link to="/collections/new-arrivals" className="hover:text-copper transition-colors">New Arrivals</Link></li>
              <li><Link to="/collections/best-sellers" className="hover:text-copper transition-colors">Best Sellers</Link></li>
            </ul>
          </div>

          {/* Customer Service */}
          <div>
            <h5 className="text-white font-medium mb-4 text-sm uppercase tracking-wider">Customer Service</h5>
            <ul className="space-y-2 text-sm">
              <li><Link to="/account" className="hover:text-copper transition-colors">My Account</Link></li>
              <li><Link to="/account/orders" className="hover:text-copper transition-colors">Track Order</Link></li>
              <li><a href="#" className="hover:text-copper transition-colors">Shipping Policy</a></li>
              <li><a href="#" className="hover:text-copper transition-colors">Returns & Exchanges</a></li>
              <li><a href="#" className="hover:text-copper transition-colors">FAQ</a></li>
            </ul>
          </div>

          {/* Contact */}
          <div>
            <h5 className="text-white font-medium mb-4 text-sm uppercase tracking-wider">Contact Us</h5>
            <ul className="space-y-3 text-sm">
              <li className="flex items-start gap-2">
                <svg className="w-4 h-4 mt-0.5 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
                </svg>
                Moradabad, Uttar Pradesh, India
              </li>
              <li className="flex items-center gap-2">
                <svg className="w-4 h-4 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
                support@indianartvilla.com
              </li>
              <li className="flex items-center gap-2">
                <svg className="w-4 h-4 shrink-0" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                </svg>
                +91 98765 43210
              </li>
            </ul>
          </div>
        </div>

        <div className="mt-12 pt-8 border-t border-gray-700 flex flex-col md:flex-row items-center justify-between gap-4 text-xs text-gray-500">
          <p>&copy; {new Date().getFullYear()} Indian Art Villa. All rights reserved.</p>
          <div className="flex gap-4">
            <a href="#" className="hover:text-copper transition-colors">Privacy Policy</a>
            <a href="#" className="hover:text-copper transition-colors">Terms of Service</a>
          </div>
        </div>
      </div>
    </footer>
  );
}
