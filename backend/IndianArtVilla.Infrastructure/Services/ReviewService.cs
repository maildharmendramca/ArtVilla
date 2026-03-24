using IndianArtVilla.Application.DTOs;
using IndianArtVilla.Application.Interfaces;
using IndianArtVilla.Core.Entities;
using IndianArtVilla.Core.Enums;
using IndianArtVilla.Core.Exceptions;
using IndianArtVilla.Core.Interfaces.Repositories;
using IndianArtVilla.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IndianArtVilla.Infrastructure.Services;

public class ReviewService : IReviewService
{
    private readonly IUnitOfWork _uow;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReviewService(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
    {
        _uow = uow;
        _userManager = userManager;
    }

    public async Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId)
    {
        var reviews = await _uow.Reviews.Query()
            .Where(r => r.ProductId == productId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var result = new List<ReviewDto>();
        foreach (var r in reviews)
        {
            var user = await _userManager.FindByIdAsync(r.UserId);
            result.Add(new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserName = user?.FullName ?? "Unknown",
                Rating = r.Rating,
                Title = r.Title,
                Body = r.Body,
                IsVerifiedPurchase = r.IsVerifiedPurchase,
                CreatedAt = r.CreatedAt
            });
        }

        return result;
    }

    public async Task<ReviewDto> AddReviewAsync(CreateReviewDto dto, string userId)
    {
        var product = await _uow.Products.GetByIdAsync(dto.ProductId)
            ?? throw new NotFoundException("Product", dto.ProductId);

        var existingReview = await _uow.Reviews
            .AnyAsync(r => r.ProductId == dto.ProductId && r.UserId == userId);

        if (existingReview)
            throw new BusinessRuleViolationException("You have already reviewed this product.");

        var isVerifiedPurchase = await _uow.OrderItems.Query()
            .AnyAsync(oi => oi.ProductId == dto.ProductId &&
                _uow.Orders.Query().Any(o =>
                    o.Id == oi.OrderId &&
                    o.UserId == userId &&
                    o.Status == OrderStatus.Delivered));

        var user = await _userManager.FindByIdAsync(userId)
            ?? throw new NotFoundException("User", userId);

        var review = new Review
        {
            ProductId = dto.ProductId,
            UserId = userId,
            Rating = dto.Rating,
            Title = dto.Title,
            Body = dto.Body,
            IsVerifiedPurchase = isVerifiedPurchase,
            IsApproved = false
        };

        _uow.Reviews.Add(review);
        await _uow.SaveChangesAsync();

        return new ReviewDto
        {
            Id = review.Id,
            ProductId = review.ProductId,
            UserName = user.FullName,
            Rating = review.Rating,
            Title = review.Title,
            Body = review.Body,
            IsVerifiedPurchase = review.IsVerifiedPurchase,
            CreatedAt = review.CreatedAt
        };
    }

    public async Task ApproveReviewAsync(int reviewId)
    {
        var review = await _uow.Reviews.GetByIdAsync(reviewId)
            ?? throw new NotFoundException("Review", reviewId);

        review.IsApproved = true;
        await _uow.SaveChangesAsync();
    }
}
