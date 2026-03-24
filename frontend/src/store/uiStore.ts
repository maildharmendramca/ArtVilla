import { create } from 'zustand';

interface UiStore {
  isMobileMenuOpen: boolean;
  isCartDrawerOpen: boolean;
  isSearchOpen: boolean;

  toggleMobileMenu: () => void;
  setMobileMenuOpen: (open: boolean) => void;
  toggleCartDrawer: () => void;
  setCartDrawerOpen: (open: boolean) => void;
  toggleSearch: () => void;
  setSearchOpen: (open: boolean) => void;
  closeAll: () => void;
}

export const useUiStore = create<UiStore>((set) => ({
  isMobileMenuOpen: false,
  isCartDrawerOpen: false,
  isSearchOpen: false,

  toggleMobileMenu: () => set((s) => ({ isMobileMenuOpen: !s.isMobileMenuOpen })),
  setMobileMenuOpen: (open) => set({ isMobileMenuOpen: open }),
  toggleCartDrawer: () => set((s) => ({ isCartDrawerOpen: !s.isCartDrawerOpen })),
  setCartDrawerOpen: (open) => set({ isCartDrawerOpen: open }),
  toggleSearch: () => set((s) => ({ isSearchOpen: !s.isSearchOpen })),
  setSearchOpen: (open) => set({ isSearchOpen: open }),
  closeAll: () => set({ isMobileMenuOpen: false, isCartDrawerOpen: false, isSearchOpen: false }),
}));
