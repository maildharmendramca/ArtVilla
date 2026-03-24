using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Enums;
using IndianArtVilla.Infrastructure.Identity;

namespace IndianArtVilla.Infrastructure.Data;

public static class InMemoryDataStore
{
    // ── Collections ────────────────────────────────────────────────
    public static List<ApplicationUser> Users { get; } = new();
    public static List<Category> Categories { get; } = new();
    public static List<Product> Products { get; } = new();
    public static List<ProductImage> ProductImages { get; } = new();
    public static List<ProductTag> ProductTags { get; } = new();
    public static List<ProductVariant> ProductVariants { get; } = new();
    public static List<Cart> Carts { get; } = new();
    public static List<CartItem> CartItems { get; } = new();
    public static List<Order> Orders { get; } = new();
    public static List<OrderItem> OrderItems { get; } = new();
    public static List<OrderStatusHistory> OrderStatusHistories { get; } = new();
    public static List<Review> Reviews { get; } = new();
    public static List<Address> Addresses { get; } = new();
    public static List<WishlistItem> WishlistItems { get; } = new();
    public static List<Coupon> Coupons { get; } = new();
    public static List<CorporateInquiry> CorporateInquiries { get; } = new();

    // userId -> list of role names
    public static Dictionary<string, List<string>> UserRoles { get; } = new();

    // userId -> plain-text password (for simple auth check)
    public static Dictionary<string, string> UserPasswords { get; } = new();

    // ── Thread-safe ID counters ────────────────────────────────────
    private static int _nextCategoryId = 0;
    private static int _nextProductId = 0;
    private static int _nextProductImageId = 0;
    private static int _nextProductTagId = 0;
    private static int _nextProductVariantId = 0;
    private static int _nextCartItemId = 0;
    private static int _nextOrderId = 0;
    private static int _nextOrderItemId = 0;
    private static int _nextOrderStatusHistoryId = 0;
    private static int _nextReviewId = 0;
    private static int _nextAddressId = 0;
    private static int _nextWishlistItemId = 0;
    private static int _nextCouponId = 0;
    private static int _nextCorporateInquiryId = 0;

    public static int NextCategoryId() => Interlocked.Increment(ref _nextCategoryId);
    public static int NextProductId() => Interlocked.Increment(ref _nextProductId);
    public static int NextProductImageId() => Interlocked.Increment(ref _nextProductImageId);
    public static int NextProductTagId() => Interlocked.Increment(ref _nextProductTagId);
    public static int NextProductVariantId() => Interlocked.Increment(ref _nextProductVariantId);
    public static int NextCartItemId() => Interlocked.Increment(ref _nextCartItemId);
    public static int NextOrderId() => Interlocked.Increment(ref _nextOrderId);
    public static int NextOrderItemId() => Interlocked.Increment(ref _nextOrderItemId);
    public static int NextOrderStatusHistoryId() => Interlocked.Increment(ref _nextOrderStatusHistoryId);
    public static int NextReviewId() => Interlocked.Increment(ref _nextReviewId);
    public static int NextAddressId() => Interlocked.Increment(ref _nextAddressId);
    public static int NextWishlistItemId() => Interlocked.Increment(ref _nextWishlistItemId);
    public static int NextCouponId() => Interlocked.Increment(ref _nextCouponId);
    public static int NextCorporateInquiryId() => Interlocked.Increment(ref _nextCorporateInquiryId);

    public static int NextId(ref int counter) => Interlocked.Increment(ref counter);

    // ── Static constructor – seeds all data ────────────────────────
    static InMemoryDataStore()
    {
        SeedUsers();
        SeedCategories();
        SeedProducts();
        SeedCoupons();
    }

    // ── Users ──────────────────────────────────────────────────────
    private static void SeedUsers()
    {
        var admin = new ApplicationUser
        {
            Id = "admin-user-id-001",
            FullName = "Admin",
            Email = "admin@indianartvilla.in",
            UserName = "admin@indianartvilla.in",
            NormalizedEmail = "ADMIN@INDIANARTVILLA.IN",
            NormalizedUserName = "ADMIN@INDIANARTVILLA.IN",
            Phone = "+919876543210",
            PhoneNumber = "+919876543210",
            PasswordHash = "Admin@123",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var customer = new ApplicationUser
        {
            Id = "customer-user-id-001",
            FullName = "Rahul Sharma",
            Email = "rahul@example.com",
            UserName = "rahul@example.com",
            NormalizedEmail = "RAHUL@EXAMPLE.COM",
            NormalizedUserName = "RAHUL@EXAMPLE.COM",
            Phone = "+919876543211",
            PhoneNumber = "+919876543211",
            PasswordHash = "Customer@123",
            EmailConfirmed = true,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        Users.Add(admin);
        Users.Add(customer);

        UserRoles["admin-user-id-001"] = new List<string> { "Admin" };
        UserRoles["customer-user-id-001"] = new List<string> { "Customer" };

        UserPasswords["admin-user-id-001"] = "Admin@123";
        UserPasswords["customer-user-id-001"] = "Customer@123";
    }

    // ── Categories ─────────────────────────────────────────────────
    private static void SeedCategories()
    {
        // Level 1: Root categories
        var copperWare = MakeCategory("Copper Ware", "copper-ware", null, "Handcrafted copper utensils and decor", "https://picsum.photos/seed/copper-ware/400/400", 1);
        var brassWare = MakeCategory("Brass Ware", "brass-ware", null, "Traditional brass items and utensils", "https://picsum.photos/seed/brass-ware/400/400", 2);
        var bronzeWare = MakeCategory("Bronze Ware", "bronze-ware", null, "Elegant bronze artifacts and statues", "https://picsum.photos/seed/bronze-ware/400/400", 3);
        var steelware = MakeCategory("Steelware", "steelware", null, "Premium stainless steel kitchenware", "https://picsum.photos/seed/steelware/400/400", 4);
        var silverPlated = MakeCategory("Silver Plated", "silver-plated", null, "Exquisite silver plated gift items", "https://picsum.photos/seed/silver-plated/400/400", 5);
        var homeDecor = MakeCategory("Home Decor", "home-decor", null, "Artisan home decor and accessories", "https://picsum.photos/seed/home-decor/400/400", 6);

        // Level 2: Subcategories
        var copperBottles = MakeCategory("Copper Bottles", "copper-bottles", copperWare.Id, null, null, 1);
        var copperGlasses = MakeCategory("Copper Glasses", "copper-glasses", copperWare.Id, null, null, 2);
        var copperJugs = MakeCategory("Copper Jugs & Pitchers", "copper-jugs-pitchers", copperWare.Id, null, null, 3);
        var copperPuja = MakeCategory("Copper Puja Items", "copper-puja-items", copperWare.Id, null, null, 4);

        var brassDiya = MakeCategory("Brass Diyas & Lamps", "brass-diyas-lamps", brassWare.Id, null, null, 1);
        var brassIdols = MakeCategory("Brass Idols", "brass-idols", brassWare.Id, null, null, 2);
        var brassUtensils = MakeCategory("Brass Utensils", "brass-utensils", brassWare.Id, null, null, 3);

        var bronzeStatues = MakeCategory("Bronze Statues", "bronze-statues", bronzeWare.Id, null, null, 1);
        var bronzeBells = MakeCategory("Bronze Bells", "bronze-bells", bronzeWare.Id, null, null, 2);

        var steelThali = MakeCategory("Steel Thali Sets", "steel-thali-sets", steelware.Id, null, null, 1);
        var steelServing = MakeCategory("Steel Serving Ware", "steel-serving-ware", steelware.Id, null, null, 2);

        var silverGifts = MakeCategory("Silver Gift Sets", "silver-gift-sets", silverPlated.Id, null, null, 1);
        var silverPuja = MakeCategory("Silver Puja Items", "silver-puja-items", silverPlated.Id, null, null, 2);

        var wallDecor = MakeCategory("Wall Decor", "wall-decor", homeDecor.Id, null, null, 1);
        var tableDecor = MakeCategory("Table Decor", "table-decor", homeDecor.Id, null, null, 2);
        var candleHolders = MakeCategory("Candle Holders", "candle-holders", homeDecor.Id, null, null, 3);

        // Level 3: Sub-subcategories
        MakeCategory("Plain Copper Bottles", "plain-copper-bottles", copperBottles.Id, null, null, 1);
        MakeCategory("Printed Copper Bottles", "printed-copper-bottles", copperBottles.Id, null, null, 2);
        MakeCategory("Hammered Copper Bottles", "hammered-copper-bottles", copperBottles.Id, null, null, 3);

        MakeCategory("Traditional Diyas", "traditional-diyas", brassDiya.Id, null, null, 1);
        MakeCategory("Hanging Lamps", "hanging-lamps", brassDiya.Id, null, null, 2);

        MakeCategory("Ganesh Idols", "ganesh-idols", brassIdols.Id, null, null, 1);
        MakeCategory("Laxmi Idols", "laxmi-idols", brassIdols.Id, null, null, 2);

        MakeCategory("Nataraja Statues", "nataraja-statues", bronzeStatues.Id, null, null, 1);
        MakeCategory("Buddha Statues", "buddha-statues", bronzeStatues.Id, null, null, 2);

        MakeCategory("Metal Wall Art", "metal-wall-art", wallDecor.Id, null, null, 1);
        MakeCategory("Wall Hangings", "wall-hangings", wallDecor.Id, null, null, 2);

        // Wire up navigation properties
        foreach (var cat in Categories)
        {
            if (cat.ParentCategoryId.HasValue)
            {
                var parent = Categories.First(c => c.Id == cat.ParentCategoryId.Value);
                cat.ParentCategory = parent;
                parent.SubCategories.Add(cat);
            }
        }
    }

    private static Category MakeCategory(string name, string slug, int? parentId, string? description, string? imageUrl, int sortOrder)
    {
        var cat = new Category
        {
            Id = NextCategoryId(),
            Name = name,
            Slug = slug,
            ParentCategoryId = parentId,
            Description = description,
            ImageUrl = imageUrl,
            SortOrder = sortOrder,
            IsActive = true
        };
        Categories.Add(cat);
        return cat;
    }

    // ── Products ───────────────────────────────────────────────────
    private static void SeedProducts()
    {
        var catBySlug = Categories.ToDictionary(c => c.Slug, c => c);

        // Copper Ware products
        AddProduct("Pure Copper Water Bottle - Hammered", "pure-copper-water-bottle-hammered", "COP-BTL-001",
            "Handcrafted pure copper water bottle with elegant hammered finish. Ayurvedic benefits of drinking water stored in copper vessel. Leak-proof cap and 1 litre capacity.",
            "Hammered copper bottle, 1L capacity", 899, 1299, null, 50, 350, null, "Pure Copper", "Hammered", "1 Litre",
            true, true, false, catBySlug["hammered-copper-bottles"]);

        AddProduct("Copper Water Bottle - Printed Paisley", "copper-water-bottle-printed-paisley", "COP-BTL-002",
            "Beautiful paisley printed copper water bottle. Traditional Indian design meets modern functionality. Helps purify water naturally.",
            "Paisley print copper bottle, 1L", 999, 1499, null, 40, 340, null, "Pure Copper", "Printed", "1 Litre",
            true, true, false, catBySlug["printed-copper-bottles"]);

        AddProduct("Plain Copper Water Bottle - Matte", "plain-copper-water-bottle-matte", "COP-BTL-003",
            "Sleek matte finish plain copper water bottle for everyday use. Joint-free design ensures no leakage. Ideal for office and gym.",
            "Matte plain copper bottle, 950ml", 699, null, null, 60, 320, null, "Pure Copper", "Matte", "950 ml",
            false, true, false, catBySlug["plain-copper-bottles"]);

        AddProduct("Copper Moscow Mule Mugs - Set of 2", "copper-moscow-mule-mugs-set-2", "COP-GLS-001",
            "Set of 2 solid copper Moscow Mule mugs with brass handles. Perfect for serving cocktails, lassi, or buttermilk. Handcrafted with lacquer coating.",
            "Set of 2 copper mugs, 500ml each", 1299, 1799, null, 30, 600, null, "Copper with Brass Handle", "Polished", "500 ml each",
            true, true, false, catBySlug["copper-glasses"]);

        AddProduct("Copper Jug with Lid - Traditional", "copper-jug-with-lid-traditional", "COP-JUG-001",
            "Pure copper water jug with fitted lid. Store water overnight for maximum health benefits. Elegant traditional design suitable for dining table.",
            "Traditional copper jug, 1.5L", 1499, 1999, null, 25, 550, null, "Pure Copper", "Polished", "1.5 Litres",
            true, true, false, catBySlug["copper-jugs-pitchers"]);

        // Brass Ware products
        AddProduct("Brass Akhand Diya - Large", "brass-akhand-diya-large", "BRS-DYA-001",
            "Traditional brass Akhand Diya for continuous lighting during puja and festivals. Intricately designed base with oil reservoir. Symbol of prosperity.",
            "Large brass Akhand Diya for puja", 599, 899, null, 45, 450, null, "Pure Brass", "Antique", null,
            true, false, false, catBySlug["traditional-diyas"]);

        AddProduct("Brass Hanging Diya - Peacock Design", "brass-hanging-diya-peacock-design", "BRS-DYA-002",
            "Ornate brass hanging diya with peacock motif. Perfect for temple decoration and festive occasions. Chain included for hanging.",
            "Peacock design brass hanging lamp", 1899, 2499, null, 15, 800, null, "Pure Brass", "Antique Gold", null,
            false, true, false, catBySlug["hanging-lamps"]);

        AddProduct("Brass Ganesh Idol - Sitting Pose", "brass-ganesh-idol-sitting-pose", "BRS-IDL-001",
            "Beautifully crafted brass Ganesh idol in sitting pose. Fine detailing on trunk and crown. Ideal for home temple, office desk, or gifting.",
            "Brass Ganesh idol, 6 inches", 2499, 3499, null, 20, 900, "6 x 4 x 3 inches", "Pure Brass", "Polished Gold", null,
            true, true, false, catBySlug["ganesh-idols"]);

        AddProduct("Brass Laxmi Idol - Standing", "brass-laxmi-idol-standing", "BRS-IDL-002",
            "Exquisite standing Laxmi idol in pure brass. Goddess of wealth and prosperity. Beautiful details on saree and lotus base.",
            "Standing Laxmi idol, 8 inches", 3299, 4499, null, 12, 1200, "8 x 3 x 2.5 inches", "Pure Brass", "Polished Gold", null,
            false, true, false, catBySlug["laxmi-idols"]);

        AddProduct("Brass Handi with Lid - Cooking", "brass-handi-with-lid-cooking", "BRS-UTN-001",
            "Traditional brass Handi for authentic Indian cooking. Tin-lined interior for safe food preparation. Enhances flavor of dal and curries.",
            "Brass cooking Handi, 2L capacity", 1799, null, null, 18, 1100, null, "Brass with Tin Lining", "Hammered", "2 Litres",
            false, false, false, catBySlug["brass-utensils"]);

        // Bronze Ware products
        AddProduct("Bronze Nataraja - Dancing Shiva", "bronze-nataraja-dancing-shiva", "BRZ-STT-001",
            "Magnificent bronze Nataraja statue depicting Lord Shiva in cosmic dance. Lost-wax casting technique from South India. Museum quality craftsmanship.",
            "Bronze Nataraja, 12 inches", 4999, null, null, 8, 2500, "12 x 10 x 4 inches", "Bronze", "Antique Patina", null,
            true, true, true, catBySlug["nataraja-statues"]);

        AddProduct("Bronze Buddha - Meditating", "bronze-buddha-meditating", "BRZ-STT-002",
            "Serene meditating Buddha statue in bronze. Dhyana mudra pose symbolizing meditation and enlightenment. Perfect for zen garden or meditation room.",
            "Meditating Buddha, 8 inches", 3499, 4299, null, 10, 1800, "8 x 5 x 4 inches", "Bronze", "Oxidized", null,
            true, true, false, catBySlug["buddha-statues"]);

        AddProduct("Bronze Temple Bell - Hanging", "bronze-temple-bell-hanging", "BRZ-BEL-001",
            "Resonant bronze temple bell with chain. Deep, melodious sound ideal for puja room. Traditional Mandir bell with Nandi mount.",
            "Bronze temple bell, 5 inches", 1299, 1699, null, 22, 700, "5 x 3 inches", "Bronze", "Antique", null,
            false, true, false, catBySlug["bronze-bells"]);

        AddProduct("Bronze Ganesha - Tribal Style", "bronze-ganesha-tribal-style", "BRZ-STT-003",
            "Unique tribal style Ganesha in bronze. Dokra art form from Bastar. Each piece is one-of-a-kind with rustic tribal charm.",
            "Tribal bronze Ganesha, 7 inches", 2799, null, null, 6, 1400, "7 x 4 x 3 inches", "Bronze", "Rustic", null,
            false, true, true, catBySlug["bronze-statues"]);

        // Steelware products
        AddProduct("Stainless Steel Thali Set - Royal", "stainless-steel-thali-set-royal", "STL-THI-001",
            "Premium stainless steel thali set with 6 pieces. Includes thali plate, 3 bowls, glass, and spoon. Mirror finish with laser-etched border.",
            "6-piece royal thali set", 1499, 1999, null, 35, 800, null, "Stainless Steel 304", "Mirror", null,
            true, true, false, catBySlug["steel-thali-sets"]);

        AddProduct("Steel Copper Bottom Thali Set", "steel-copper-bottom-thali-set", "STL-THI-002",
            "Stainless steel thali set with copper bottom for even heat distribution. Traditional design with modern utility. Set of 5 pieces.",
            "5-piece copper bottom thali", 1199, null, null, 28, 750, null, "Stainless Steel with Copper Bottom", "Satin", null,
            false, false, false, catBySlug["steel-thali-sets"]);

        AddProduct("Steel Serving Bowl Set - Hammered", "steel-serving-bowl-set-hammered", "STL-SRV-001",
            "Set of 3 hammered stainless steel serving bowls with lids. Elegant design for dinner parties and festive occasions. Double wall insulation.",
            "Set of 3 hammered serving bowls", 1899, 2499, null, 20, 1200, null, "Stainless Steel 304", "Hammered", null,
            false, true, false, catBySlug["steel-serving-ware"]);

        // Silver Plated products
        AddProduct("Silver Plated Bowl & Spoon Gift Set", "silver-plated-bowl-spoon-gift-set", "SLV-GFT-001",
            "Elegant silver plated bowl and spoon set in premium gift box. Perfect for baby shower, wedding, or housewarming. Anti-tarnish coating.",
            "Silver plated bowl & spoon in gift box", 999, 1499, null, 40, 300, null, "Silver Plated Brass", "High Polish", null,
            true, true, false, catBySlug["silver-gift-sets"]);

        AddProduct("Silver Plated Wine Glass Set - 4 Pcs", "silver-plated-wine-glass-set-4", "SLV-GFT-002",
            "Set of 4 silver plated wine glasses. Ideal for celebrations and special occasions. Comes in velvet-lined presentation box.",
            "Set of 4 silver plated wine glasses", 2499, 3299, null, 15, 500, null, "Silver Plated Brass", "High Polish", "250 ml each",
            false, true, false, catBySlug["silver-gift-sets"]);

        AddProduct("Silver Plated Puja Thali Set", "silver-plated-puja-thali-set", "SLV-PUJ-001",
            "Complete silver plated puja thali set with 7 pieces. Includes thali, diya, bell, incense holder, kumkum box, spoon, and achamani. Gift boxed.",
            "7-piece silver plated puja set", 1999, 2799, null, 18, 600, null, "Silver Plated Brass", "Engraved", null,
            true, true, false, catBySlug["silver-puja-items"]);

        AddProduct("Silver Plated Kumkum Box - Peacock", "silver-plated-kumkum-box-peacock", "SLV-PUJ-002",
            "Decorative silver plated kumkum box with peacock design lid. Dual compartment for kumkum and haldi. Traditional yet elegant.",
            "Peacock kumkum box, silver plated", 499, 699, null, 55, 150, null, "Silver Plated Brass", "Engraved", null,
            false, false, false, catBySlug["silver-puja-items"]);

        // Home Decor products
        AddProduct("Metal Wall Art - Tree of Life", "metal-wall-art-tree-of-life", "DCR-WLL-001",
            "Stunning Tree of Life metal wall art. Hand-painted iron artwork with golden accents. Statement piece for living room or hallway.",
            "Tree of Life wall art, 24 inches", 2999, 3999, null, 12, 2000, "24 x 24 inches", "Wrought Iron", "Hand Painted", null,
            true, true, false, catBySlug["metal-wall-art"]);

        AddProduct("Brass Wall Hanging - Sun Face", "brass-wall-hanging-sun-face", "DCR-WLL-002",
            "Decorative brass sun face wall hanging. Radiating sun rays design symbolizing energy and positivity. Handcrafted with mounting hook.",
            "Brass sun face, 10 inches", 1499, 1899, null, 16, 800, "10 inches diameter", "Brass", "Antique Gold", null,
            false, true, false, catBySlug["wall-hangings"]);

        AddProduct("Brass Candle Holder - Lotus Design", "brass-candle-holder-lotus-design", "DCR-CDL-001",
            "Elegant lotus-shaped brass candle holder. Creates beautiful light patterns. Perfect for romantic dinners and meditation spaces.",
            "Lotus brass candle holder", 799, 1099, null, 30, 400, "5 x 5 x 4 inches", "Brass", "Antique", null,
            false, true, false, catBySlug["candle-holders"]);

        AddProduct("Copper Table Centerpiece - Floral Urli", "copper-table-centerpiece-floral-urli", "DCR-TBL-001",
            "Traditional copper Urli for floating flowers and candles. Perfect table centerpiece for festivals and occasions. Handcrafted with decorative rim.",
            "Copper Urli, 12 inches", 1699, 2199, null, 14, 900, "12 inches diameter", "Pure Copper", "Polished", null,
            true, true, false, catBySlug["table-decor"]);

        AddProduct("Metal Elephant Figurine - Pair", "metal-elephant-figurine-pair", "DCR-TBL-002",
            "Pair of decorative metal elephant figurines with upraised trunks. Symbol of good luck and prosperity. Meenakari work with vibrant colors.",
            "Pair of elephant figurines, 5 inches", 1299, 1799, null, 20, 700, "5 x 3 x 4 inches each", "Brass with Meenakari", "Multi-color Enamel", null,
            false, true, false, catBySlug["table-decor"]);

        // Additional copper products
        AddProduct("Copper Puja Kalash with Coconut", "copper-puja-kalash-with-coconut", "COP-PUJ-001",
            "Sacred copper Kalash with decorative coconut lid. Essential for Griha Pravesh, Satyanarayan Puja and Navratri. Pure copper construction.",
            "Copper Puja Kalash, medium", 799, 1099, null, 35, 500, null, "Pure Copper", "Polished", "500 ml",
            false, true, false, catBySlug["copper-puja-items"]);

        AddProduct("Copper Tamba Set - 4 Glasses", "copper-tamba-set-4-glasses", "COP-GLS-002",
            "Set of 4 pure copper tamba glasses for Ayurvedic water drinking. Smooth polished finish with rounded edges. Perfect family set.",
            "Set of 4 copper glasses, 300ml", 999, 1399, null, 25, 480, null, "Pure Copper", "Polished", "300 ml each",
            false, true, false, catBySlug["copper-glasses"]);

        AddProduct("Brass Flower Vase - Antique", "brass-flower-vase-antique", "BRS-UTN-002",
            "Classic antique finish brass flower vase. Traditional Indian design with floral motifs. Adds elegance to any living space.",
            "Antique brass vase, 10 inches", 1399, 1899, null, 15, 650, "10 x 4 inches", "Pure Brass", "Antique", null,
            false, true, false, catBySlug["brass-utensils"]);

        // Wire up Product -> Category navigation
        foreach (var product in Products)
        {
            var category = Categories.First(c => c.Id == product.CategoryId);
            product.Category = category;
            category.Products.Add(product);
        }
    }

    private static void AddProduct(
        string name, string slug, string sku,
        string description, string shortDescription,
        decimal price, decimal? compareAtPrice, decimal? costPrice,
        int stockQuantity, decimal weightGrams, string? dimensions,
        string? material, string? finish, string? capacity,
        bool isFeatured, bool isGiftable, bool isCustomizable,
        Category category)
    {
        var productId = NextProductId();

        var product = new Product
        {
            Id = productId,
            Name = name,
            Slug = slug,
            SKU = sku,
            Description = description,
            ShortDescription = shortDescription,
            Price = price,
            CompareAtPrice = compareAtPrice,
            CostPrice = costPrice,
            StockQuantity = stockQuantity,
            TrackInventory = true,
            WeightGrams = weightGrams,
            Dimensions = dimensions,
            Material = material,
            Finish = finish,
            Capacity = capacity,
            IsFeatured = isFeatured,
            IsGiftable = isGiftable,
            IsCustomizable = isCustomizable,
            IsActive = true,
            CategoryId = category.Id,
            CreatedAt = DateTime.UtcNow
        };

        Products.Add(product);

        // Primary image
        var image = new ProductImage
        {
            Id = NextProductImageId(),
            ProductId = productId,
            ImageUrl = $"https://picsum.photos/seed/{slug}/600/600",
            AltText = name,
            IsPrimary = true,
            SortOrder = 1
        };
        ProductImages.Add(image);
        product.Images.Add(image);

        // Default variant
        var variant = new ProductVariant
        {
            Id = NextProductVariantId(),
            ProductId = productId,
            Name = "Default",
            SKU = sku,
            PriceAdjustment = 0,
            StockQuantity = stockQuantity
        };
        ProductVariants.Add(variant);
        product.Variants.Add(variant);

        // Tags
        if (material != null)
        {
            var materialTag = new ProductTag { Id = NextProductTagId(), ProductId = productId, Tag = material };
            ProductTags.Add(materialTag);
            product.Tags.Add(materialTag);
        }

        if (isGiftable)
        {
            var giftTag = new ProductTag { Id = NextProductTagId(), ProductId = productId, Tag = "Gift" };
            ProductTags.Add(giftTag);
            product.Tags.Add(giftTag);
        }

        if (isFeatured)
        {
            var featuredTag = new ProductTag { Id = NextProductTagId(), ProductId = productId, Tag = "Featured" };
            ProductTags.Add(featuredTag);
            product.Tags.Add(featuredTag);
        }
    }

    // ── Coupons ────────────────────────────────────────────────────
    private static void SeedCoupons()
    {
        Coupons.Add(new Coupon
        {
            Id = NextCouponId(),
            Code = "WELCOME10",
            Type = CouponType.Percentage,
            Value = 10,
            MinOrderAmount = 499,
            MaxDiscountAmount = 200,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddMonths(6)
        });

        Coupons.Add(new Coupon
        {
            Id = NextCouponId(),
            Code = "FLAT200",
            Type = CouponType.FixedAmount,
            Value = 200,
            MinOrderAmount = 999,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddMonths(3)
        });

        Coupons.Add(new Coupon
        {
            Id = NextCouponId(),
            Code = "FREESHIP",
            Type = CouponType.FreeShipping,
            Value = 0,
            MinOrderAmount = 500,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddMonths(6)
        });

        Coupons.Add(new Coupon
        {
            Id = NextCouponId(),
            Code = "DIWALI25",
            Type = CouponType.Percentage,
            Value = 25,
            MinOrderAmount = 1499,
            MaxDiscountAmount = 500,
            UsageLimit = 100,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddMonths(1)
        });

        Coupons.Add(new Coupon
        {
            Id = NextCouponId(),
            Code = "GIFT15",
            Type = CouponType.Percentage,
            Value = 15,
            MinOrderAmount = 799,
            MaxDiscountAmount = 300,
            IsActive = true,
            ExpiresAt = DateTime.UtcNow.AddMonths(12)
        });
    }
}
