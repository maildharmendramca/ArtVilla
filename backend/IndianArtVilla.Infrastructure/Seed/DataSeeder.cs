using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Enums;
using IndianArtVilla.Infrastructure.Data;
using IndianArtVilla.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IndianArtVilla.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        await db.Database.MigrateAsync();

        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
        await SeedCategoriesAsync(db);
        await SeedProductsAsync(db);
        await SeedCouponsAsync(db);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "Customer" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        // Admin user
        const string adminEmail = "admin@testmail.com";
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var admin = new ApplicationUser
            {
                FullName = "Admin",
                Email = adminEmail,
                UserName = adminEmail,
                Phone = "+919876543210",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(admin, "Admin");
        }

    }

    private static async Task SeedCategoriesAsync(AppDbContext db)
    {
        if (await db.Categories.AnyAsync()) return;

        var copperWare = new Category { Name = "Copper Ware", Slug = "copper-ware", Description = "Handcrafted copper utensils and decor", SortOrder = 1, ImageUrl = "https://picsum.photos/seed/copper-ware/400/400" };
        var brassWare = new Category { Name = "Brass Ware", Slug = "brass-ware", Description = "Traditional brass items and utensils", SortOrder = 2, ImageUrl = "https://picsum.photos/seed/brass-ware/400/400" };
        var bronzeWare = new Category { Name = "Bronze Ware", Slug = "bronze-ware", Description = "Elegant bronze artifacts and statues", SortOrder = 3, ImageUrl = "https://picsum.photos/seed/bronze-ware/400/400" };
        var steelware = new Category { Name = "Steelware", Slug = "steelware", Description = "Premium stainless steel kitchenware", SortOrder = 4, ImageUrl = "https://picsum.photos/seed/steelware/400/400" };
        var silverPlated = new Category { Name = "Silver Plated", Slug = "silver-plated", Description = "Exquisite silver plated gift items", SortOrder = 5, ImageUrl = "https://picsum.photos/seed/silver-plated/400/400" };
        var homeDecor = new Category { Name = "Home Decor", Slug = "home-decor", Description = "Artisan home decor and accessories", SortOrder = 6, ImageUrl = "https://picsum.photos/seed/home-decor/400/400" };

        db.Categories.AddRange(copperWare, brassWare, bronzeWare, steelware, silverPlated, homeDecor);
        await db.SaveChangesAsync();

        var copperBottles = new Category { Name = "Copper Bottles", Slug = "copper-bottles", ParentCategoryId = copperWare.Id, SortOrder = 1 };
        var copperGlasses = new Category { Name = "Copper Glasses", Slug = "copper-glasses", ParentCategoryId = copperWare.Id, SortOrder = 2 };
        var copperJugs = new Category { Name = "Copper Jugs & Pitchers", Slug = "copper-jugs-pitchers", ParentCategoryId = copperWare.Id, SortOrder = 3 };
        var copperPuja = new Category { Name = "Copper Puja Items", Slug = "copper-puja-items", ParentCategoryId = copperWare.Id, SortOrder = 4 };
        var brassDiya = new Category { Name = "Brass Diyas & Lamps", Slug = "brass-diyas-lamps", ParentCategoryId = brassWare.Id, SortOrder = 1 };
        var brassIdols = new Category { Name = "Brass Idols", Slug = "brass-idols", ParentCategoryId = brassWare.Id, SortOrder = 2 };
        var brassUtensils = new Category { Name = "Brass Utensils", Slug = "brass-utensils", ParentCategoryId = brassWare.Id, SortOrder = 3 };
        var bronzeStatues = new Category { Name = "Bronze Statues", Slug = "bronze-statues", ParentCategoryId = bronzeWare.Id, SortOrder = 1 };
        var bronzeBells = new Category { Name = "Bronze Bells", Slug = "bronze-bells", ParentCategoryId = bronzeWare.Id, SortOrder = 2 };
        var steelThali = new Category { Name = "Steel Thali Sets", Slug = "steel-thali-sets", ParentCategoryId = steelware.Id, SortOrder = 1 };
        var steelServing = new Category { Name = "Steel Serving Ware", Slug = "steel-serving-ware", ParentCategoryId = steelware.Id, SortOrder = 2 };
        var silverGifts = new Category { Name = "Silver Gift Sets", Slug = "silver-gift-sets", ParentCategoryId = silverPlated.Id, SortOrder = 1 };
        var silverPuja = new Category { Name = "Silver Puja Items", Slug = "silver-puja-items", ParentCategoryId = silverPlated.Id, SortOrder = 2 };
        var wallDecor = new Category { Name = "Wall Decor", Slug = "wall-decor", ParentCategoryId = homeDecor.Id, SortOrder = 1 };
        var tableDecor = new Category { Name = "Table Decor", Slug = "table-decor", ParentCategoryId = homeDecor.Id, SortOrder = 2 };
        var candleHolders = new Category { Name = "Candle Holders", Slug = "candle-holders", ParentCategoryId = homeDecor.Id, SortOrder = 3 };

        db.Categories.AddRange(copperBottles, copperGlasses, copperJugs, copperPuja, brassDiya, brassIdols, brassUtensils, bronzeStatues, bronzeBells, steelThali, steelServing, silverGifts, silverPuja, wallDecor, tableDecor, candleHolders);
        await db.SaveChangesAsync();

        var plainBottles = new Category { Name = "Plain Copper Bottles", Slug = "plain-copper-bottles", ParentCategoryId = copperBottles.Id, SortOrder = 1 };
        var printedBottles = new Category { Name = "Printed Copper Bottles", Slug = "printed-copper-bottles", ParentCategoryId = copperBottles.Id, SortOrder = 2 };
        var hammeredBottles = new Category { Name = "Hammered Copper Bottles", Slug = "hammered-copper-bottles", ParentCategoryId = copperBottles.Id, SortOrder = 3 };
        var traditionalDiya = new Category { Name = "Traditional Diyas", Slug = "traditional-diyas", ParentCategoryId = brassDiya.Id, SortOrder = 1 };
        var hangingLamps = new Category { Name = "Hanging Lamps", Slug = "hanging-lamps", ParentCategoryId = brassDiya.Id, SortOrder = 2 };
        var ganeshIdols = new Category { Name = "Ganesh Idols", Slug = "ganesh-idols", ParentCategoryId = brassIdols.Id, SortOrder = 1 };
        var laxmiIdols = new Category { Name = "Laxmi Idols", Slug = "laxmi-idols", ParentCategoryId = brassIdols.Id, SortOrder = 2 };
        var natarajaStatues = new Category { Name = "Nataraja Statues", Slug = "nataraja-statues", ParentCategoryId = bronzeStatues.Id, SortOrder = 1 };
        var buddhaStatues = new Category { Name = "Buddha Statues", Slug = "buddha-statues", ParentCategoryId = bronzeStatues.Id, SortOrder = 2 };
        var metalWallArt = new Category { Name = "Metal Wall Art", Slug = "metal-wall-art", ParentCategoryId = wallDecor.Id, SortOrder = 1 };
        var wallHangings = new Category { Name = "Wall Hangings", Slug = "wall-hangings", ParentCategoryId = wallDecor.Id, SortOrder = 2 };

        db.Categories.AddRange(plainBottles, printedBottles, hammeredBottles, traditionalDiya, hangingLamps, ganeshIdols, laxmiIdols, natarajaStatues, buddhaStatues, metalWallArt, wallHangings);
        await db.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(AppDbContext db)
    {
        if (await db.Products.AnyAsync()) return;

        var categories = await db.Categories.ToDictionaryAsync(c => c.Slug, c => c.Id);

        var products = new List<Product>
        {
            new() { Name = "Pure Copper Water Bottle - Hammered", Slug = "pure-copper-water-bottle-hammered", SKU = "COP-BTL-001", Description = "Handcrafted pure copper water bottle with elegant hammered finish. Ayurvedic benefits of drinking water stored in copper vessel. Leak-proof cap and 1 litre capacity.", ShortDescription = "Hammered copper bottle, 1L capacity", Price = 899, CompareAtPrice = 1299, StockQuantity = 50, WeightGrams = 350, Material = "Pure Copper", Finish = "Hammered", Capacity = "1 Litre", CategoryId = categories["hammered-copper-bottles"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Copper Water Bottle - Printed Paisley", Slug = "copper-water-bottle-printed-paisley", SKU = "COP-BTL-002", Description = "Beautiful paisley printed copper water bottle. Traditional Indian design meets modern functionality.", ShortDescription = "Paisley print copper bottle, 1L", Price = 999, CompareAtPrice = 1499, StockQuantity = 40, WeightGrams = 340, Material = "Pure Copper", Finish = "Printed", Capacity = "1 Litre", CategoryId = categories["printed-copper-bottles"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Plain Copper Water Bottle - Matte", Slug = "plain-copper-water-bottle-matte", SKU = "COP-BTL-003", Description = "Sleek matte finish plain copper water bottle for everyday use.", ShortDescription = "Matte plain copper bottle, 950ml", Price = 699, StockQuantity = 60, WeightGrams = 320, Material = "Pure Copper", Finish = "Matte", Capacity = "950 ml", CategoryId = categories["plain-copper-bottles"], IsGiftable = true },
            new() { Name = "Copper Moscow Mule Mugs - Set of 2", Slug = "copper-moscow-mule-mugs-set-2", SKU = "COP-GLS-001", Description = "Set of 2 solid copper Moscow Mule mugs with brass handles.", ShortDescription = "Set of 2 copper mugs, 500ml each", Price = 1299, CompareAtPrice = 1799, StockQuantity = 30, WeightGrams = 600, Material = "Copper with Brass Handle", Finish = "Polished", Capacity = "500 ml each", CategoryId = categories["copper-glasses"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Copper Jug with Lid - Traditional", Slug = "copper-jug-with-lid-traditional", SKU = "COP-JUG-001", Description = "Pure copper water jug with fitted lid.", ShortDescription = "Traditional copper jug, 1.5L", Price = 1499, CompareAtPrice = 1999, StockQuantity = 25, WeightGrams = 550, Material = "Pure Copper", Finish = "Polished", Capacity = "1.5 Litres", CategoryId = categories["copper-jugs-pitchers"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Brass Akhand Diya - Large", Slug = "brass-akhand-diya-large", SKU = "BRS-DYA-001", Description = "Traditional brass Akhand Diya for continuous lighting.", ShortDescription = "Large brass Akhand Diya for puja", Price = 599, CompareAtPrice = 899, StockQuantity = 45, WeightGrams = 450, Material = "Pure Brass", Finish = "Antique", CategoryId = categories["traditional-diyas"], IsFeatured = true },
            new() { Name = "Brass Hanging Diya - Peacock Design", Slug = "brass-hanging-diya-peacock-design", SKU = "BRS-DYA-002", Description = "Ornate brass hanging diya with peacock motif.", ShortDescription = "Peacock design brass hanging lamp", Price = 1899, CompareAtPrice = 2499, StockQuantity = 15, WeightGrams = 800, Material = "Pure Brass", Finish = "Antique Gold", CategoryId = categories["hanging-lamps"], IsGiftable = true },
            new() { Name = "Brass Ganesh Idol - Sitting Pose", Slug = "brass-ganesh-idol-sitting-pose", SKU = "BRS-IDL-001", Description = "Beautifully crafted brass Ganesh idol in sitting pose.", ShortDescription = "Brass Ganesh idol, 6 inches", Price = 2499, CompareAtPrice = 3499, StockQuantity = 20, WeightGrams = 900, Material = "Pure Brass", Finish = "Polished Gold", Dimensions = "6 x 4 x 3 inches", CategoryId = categories["ganesh-idols"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Brass Laxmi Idol - Standing", Slug = "brass-laxmi-idol-standing", SKU = "BRS-IDL-002", Description = "Exquisite standing Laxmi idol in pure brass.", ShortDescription = "Standing Laxmi idol, 8 inches", Price = 3299, CompareAtPrice = 4499, StockQuantity = 12, WeightGrams = 1200, Material = "Pure Brass", Finish = "Polished Gold", Dimensions = "8 x 3 x 2.5 inches", CategoryId = categories["laxmi-idols"], IsGiftable = true },
            new() { Name = "Brass Handi with Lid - Cooking", Slug = "brass-handi-with-lid-cooking", SKU = "BRS-UTN-001", Description = "Traditional brass Handi for authentic Indian cooking.", ShortDescription = "Brass cooking Handi, 2L capacity", Price = 1799, StockQuantity = 18, WeightGrams = 1100, Material = "Brass with Tin Lining", Finish = "Hammered", Capacity = "2 Litres", CategoryId = categories["brass-utensils"] },
            new() { Name = "Bronze Nataraja - Dancing Shiva", Slug = "bronze-nataraja-dancing-shiva", SKU = "BRZ-STT-001", Description = "Magnificent bronze Nataraja statue.", ShortDescription = "Bronze Nataraja, 12 inches", Price = 4999, StockQuantity = 8, WeightGrams = 2500, Material = "Bronze", Finish = "Antique Patina", Dimensions = "12 x 10 x 4 inches", CategoryId = categories["nataraja-statues"], IsFeatured = true, IsGiftable = true, IsCustomizable = true },
            new() { Name = "Bronze Buddha - Meditating", Slug = "bronze-buddha-meditating", SKU = "BRZ-STT-002", Description = "Serene meditating Buddha statue in bronze.", ShortDescription = "Meditating Buddha, 8 inches", Price = 3499, CompareAtPrice = 4299, StockQuantity = 10, WeightGrams = 1800, Material = "Bronze", Finish = "Oxidized", Dimensions = "8 x 5 x 4 inches", CategoryId = categories["buddha-statues"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Bronze Temple Bell - Hanging", Slug = "bronze-temple-bell-hanging", SKU = "BRZ-BEL-001", Description = "Resonant bronze temple bell with chain.", ShortDescription = "Bronze temple bell, 5 inches", Price = 1299, CompareAtPrice = 1699, StockQuantity = 22, WeightGrams = 700, Material = "Bronze", Finish = "Antique", Dimensions = "5 x 3 inches", CategoryId = categories["bronze-bells"], IsGiftable = true },
            new() { Name = "Bronze Ganesha - Tribal Style", Slug = "bronze-ganesha-tribal-style", SKU = "BRZ-STT-003", Description = "Unique tribal style Ganesha in bronze.", ShortDescription = "Tribal bronze Ganesha, 7 inches", Price = 2799, StockQuantity = 6, WeightGrams = 1400, Material = "Bronze", Finish = "Rustic", Dimensions = "7 x 4 x 3 inches", CategoryId = categories["bronze-statues"], IsGiftable = true, IsCustomizable = true },
            new() { Name = "Stainless Steel Thali Set - Royal", Slug = "stainless-steel-thali-set-royal", SKU = "STL-THI-001", Description = "Premium stainless steel thali set with 6 pieces.", ShortDescription = "6-piece royal thali set", Price = 1499, CompareAtPrice = 1999, StockQuantity = 35, WeightGrams = 800, Material = "Stainless Steel 304", Finish = "Mirror", CategoryId = categories["steel-thali-sets"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Steel Copper Bottom Thali Set", Slug = "steel-copper-bottom-thali-set", SKU = "STL-THI-002", Description = "Stainless steel thali set with copper bottom.", ShortDescription = "5-piece copper bottom thali", Price = 1199, StockQuantity = 28, WeightGrams = 750, Material = "Stainless Steel with Copper Bottom", Finish = "Satin", CategoryId = categories["steel-thali-sets"] },
            new() { Name = "Steel Serving Bowl Set - Hammered", Slug = "steel-serving-bowl-set-hammered", SKU = "STL-SRV-001", Description = "Set of 3 hammered stainless steel serving bowls.", ShortDescription = "Set of 3 hammered serving bowls", Price = 1899, CompareAtPrice = 2499, StockQuantity = 20, WeightGrams = 1200, Material = "Stainless Steel 304", Finish = "Hammered", CategoryId = categories["steel-serving-ware"], IsGiftable = true },
            new() { Name = "Silver Plated Bowl & Spoon Gift Set", Slug = "silver-plated-bowl-spoon-gift-set", SKU = "SLV-GFT-001", Description = "Elegant silver plated bowl and spoon set.", ShortDescription = "Silver plated bowl & spoon in gift box", Price = 999, CompareAtPrice = 1499, StockQuantity = 40, WeightGrams = 300, Material = "Silver Plated Brass", Finish = "High Polish", CategoryId = categories["silver-gift-sets"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Silver Plated Wine Glass Set - 4 Pcs", Slug = "silver-plated-wine-glass-set-4", SKU = "SLV-GFT-002", Description = "Set of 4 silver plated wine glasses.", ShortDescription = "Set of 4 silver plated wine glasses", Price = 2499, CompareAtPrice = 3299, StockQuantity = 15, WeightGrams = 500, Material = "Silver Plated Brass", Finish = "High Polish", Capacity = "250 ml each", CategoryId = categories["silver-gift-sets"], IsGiftable = true },
            new() { Name = "Silver Plated Puja Thali Set", Slug = "silver-plated-puja-thali-set", SKU = "SLV-PUJ-001", Description = "Complete silver plated puja thali set.", ShortDescription = "7-piece silver plated puja set", Price = 1999, CompareAtPrice = 2799, StockQuantity = 18, WeightGrams = 600, Material = "Silver Plated Brass", Finish = "Engraved", CategoryId = categories["silver-puja-items"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Silver Plated Kumkum Box - Peacock", Slug = "silver-plated-kumkum-box-peacock", SKU = "SLV-PUJ-002", Description = "Decorative silver plated kumkum box.", ShortDescription = "Peacock kumkum box, silver plated", Price = 499, CompareAtPrice = 699, StockQuantity = 55, WeightGrams = 150, Material = "Silver Plated Brass", Finish = "Engraved", CategoryId = categories["silver-puja-items"] },
            new() { Name = "Metal Wall Art - Tree of Life", Slug = "metal-wall-art-tree-of-life", SKU = "DCR-WLL-001", Description = "Stunning Tree of Life metal wall art.", ShortDescription = "Tree of Life wall art, 24 inches", Price = 2999, CompareAtPrice = 3999, StockQuantity = 12, WeightGrams = 2000, Material = "Wrought Iron", Finish = "Hand Painted", Dimensions = "24 x 24 inches", CategoryId = categories["metal-wall-art"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Brass Wall Hanging - Sun Face", Slug = "brass-wall-hanging-sun-face", SKU = "DCR-WLL-002", Description = "Decorative brass sun face wall hanging.", ShortDescription = "Brass sun face, 10 inches", Price = 1499, CompareAtPrice = 1899, StockQuantity = 16, WeightGrams = 800, Material = "Brass", Finish = "Antique Gold", Dimensions = "10 inches diameter", CategoryId = categories["wall-hangings"], IsGiftable = true },
            new() { Name = "Brass Candle Holder - Lotus Design", Slug = "brass-candle-holder-lotus-design", SKU = "DCR-CDL-001", Description = "Elegant lotus-shaped brass candle holder.", ShortDescription = "Lotus brass candle holder", Price = 799, CompareAtPrice = 1099, StockQuantity = 30, WeightGrams = 400, Material = "Brass", Finish = "Antique", Dimensions = "5 x 5 x 4 inches", CategoryId = categories["candle-holders"], IsGiftable = true },
            new() { Name = "Copper Table Centerpiece - Floral Urli", Slug = "copper-table-centerpiece-floral-urli", SKU = "DCR-TBL-001", Description = "Traditional copper Urli for floating flowers.", ShortDescription = "Copper Urli, 12 inches", Price = 1699, CompareAtPrice = 2199, StockQuantity = 14, WeightGrams = 900, Material = "Pure Copper", Finish = "Polished", Dimensions = "12 inches diameter", CategoryId = categories["table-decor"], IsFeatured = true, IsGiftable = true },
            new() { Name = "Metal Elephant Figurine - Pair", Slug = "metal-elephant-figurine-pair", SKU = "DCR-TBL-002", Description = "Pair of decorative metal elephant figurines.", ShortDescription = "Pair of elephant figurines, 5 inches", Price = 1299, CompareAtPrice = 1799, StockQuantity = 20, WeightGrams = 700, Material = "Brass with Meenakari", Finish = "Multi-color Enamel", Dimensions = "5 x 3 x 4 inches each", CategoryId = categories["table-decor"], IsGiftable = true },
            new() { Name = "Copper Puja Kalash with Coconut", Slug = "copper-puja-kalash-with-coconut", SKU = "COP-PUJ-001", Description = "Sacred copper Kalash with decorative coconut lid.", ShortDescription = "Copper Puja Kalash, medium", Price = 799, CompareAtPrice = 1099, StockQuantity = 35, WeightGrams = 500, Material = "Pure Copper", Finish = "Polished", Capacity = "500 ml", CategoryId = categories["copper-puja-items"], IsGiftable = true },
            new() { Name = "Copper Tamba Set - 4 Glasses", Slug = "copper-tamba-set-4-glasses", SKU = "COP-GLS-002", Description = "Set of 4 pure copper tamba glasses.", ShortDescription = "Set of 4 copper glasses, 300ml", Price = 999, CompareAtPrice = 1399, StockQuantity = 25, WeightGrams = 480, Material = "Pure Copper", Finish = "Polished", Capacity = "300 ml each", CategoryId = categories["copper-glasses"], IsGiftable = true },
            new() { Name = "Brass Flower Vase - Antique", Slug = "brass-flower-vase-antique", SKU = "BRS-UTN-002", Description = "Classic antique finish brass flower vase.", ShortDescription = "Antique brass vase, 10 inches", Price = 1399, CompareAtPrice = 1899, StockQuantity = 15, WeightGrams = 650, Material = "Pure Brass", Finish = "Antique", Dimensions = "10 x 4 inches", CategoryId = categories["brass-utensils"], IsGiftable = true },
        };

        db.Products.AddRange(products);
        await db.SaveChangesAsync();

        var savedProducts = await db.Products.ToListAsync();
        foreach (var product in savedProducts)
        {
            db.ProductImages.Add(new ProductImage { ProductId = product.Id, ImageUrl = $"https://picsum.photos/seed/{product.Slug}/600/600", AltText = product.Name, IsPrimary = true, SortOrder = 1 });
            db.ProductVariants.Add(new ProductVariant { ProductId = product.Id, Name = "Default", SKU = product.SKU, PriceAdjustment = 0, StockQuantity = product.StockQuantity });
            if (product.Material != null) db.ProductTags.Add(new ProductTag { ProductId = product.Id, Tag = product.Material });
            if (product.IsGiftable) db.ProductTags.Add(new ProductTag { ProductId = product.Id, Tag = "Gift" });
            if (product.IsFeatured) db.ProductTags.Add(new ProductTag { ProductId = product.Id, Tag = "Featured" });
        }

        await db.SaveChangesAsync();
    }

    private static async Task SeedCouponsAsync(AppDbContext db)
    {
        if (await db.Coupons.AnyAsync()) return;

        var coupons = new List<Coupon>
        {
            new() { Code = "WELCOME10", Type = CouponType.Percentage, Value = 10, MinOrderAmount = 499, MaxDiscountAmount = 200, IsActive = true, ExpiresAt = DateTime.UtcNow.AddMonths(6) },
            new() { Code = "FLAT200", Type = CouponType.FixedAmount, Value = 200, MinOrderAmount = 999, IsActive = true, ExpiresAt = DateTime.UtcNow.AddMonths(3) },
            new() { Code = "FREESHIP", Type = CouponType.FreeShipping, Value = 0, MinOrderAmount = 500, IsActive = true, ExpiresAt = DateTime.UtcNow.AddMonths(6) },
            new() { Code = "DIWALI25", Type = CouponType.Percentage, Value = 25, MinOrderAmount = 1499, MaxDiscountAmount = 500, UsageLimit = 100, IsActive = true, ExpiresAt = DateTime.UtcNow.AddMonths(1) },
            new() { Code = "GIFT15", Type = CouponType.Percentage, Value = 15, MinOrderAmount = 799, MaxDiscountAmount = 300, IsActive = true, ExpiresAt = DateTime.UtcNow.AddMonths(12) },
        };

        db.Coupons.AddRange(coupons);
        await db.SaveChangesAsync();
    }
}
