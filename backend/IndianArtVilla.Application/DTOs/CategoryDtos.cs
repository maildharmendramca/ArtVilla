using System.Text.Json.Serialization;

namespace IndianArtVilla.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }

    [JsonPropertyName("image")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("parentId")]
    public int? ParentCategoryId { get; set; }

    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }

    [JsonPropertyName("children")]
    public List<CategoryDto> SubCategories { get; set; } = new();
}

public class CategorySummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public string? ImageUrl { get; set; }

    public int ProductCount { get; set; }
}

public class CreateCategoryDto
{
    public string Name { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class UpdateCategoryDto : CreateCategoryDto
{
    public bool IsActive { get; set; } = true;
}
