using IndianArtVilla.Application.DTOs;

namespace IndianArtVilla.Application.Interfaces;

public interface IReviewService
{
    Task<IEnumerable<ReviewDto>> GetProductReviewsAsync(int productId);
    Task<ReviewDto> AddReviewAsync(CreateReviewDto dto, string userId);
    Task ApproveReviewAsync(int reviewId);
}
