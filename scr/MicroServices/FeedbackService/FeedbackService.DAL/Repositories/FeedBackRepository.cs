using FeedbackService.DAL.Interfaces;
using AutoMapper;
using FeedbackService.DAL.DTO;

namespace FeedbackService.DAL.Repositories
{
    public class FeedBackRepository : IFeedbackRepository
    {
        private readonly IMapper _mapper;
        public FeedBackRepository(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<bool> CreateFeedbackAsync(string feedbaclStrings, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteFeedbackAsync(int feedbackId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateFeedbackAsync(int feedbackId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<FeedbackDTODAL> GetFeedbackAsync(int orderId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
