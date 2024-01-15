using AutoMapper;
using FeedbackService.BLL.DTO;
using FeedbackService.BLL.Interfaces;
using FeedbackService.DAL.DTO;
using FeedbackService.DAL.Interfaces;

namespace FeedbackService.BLL
{
    public class FeedbackBLL: IFeedbackBLL
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMapper _mapper;
        public FeedbackBLL(IFeedbackRepository feedbackRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _mapper = mapper;
        }


        public async Task<bool> CreateFeedbackAsync(string feedbaclStrings, CancellationToken cancellationToken = default)
        {
           return await _feedbackRepository.CreateFeedbackAsync(feedbaclStrings, cancellationToken);
        }

        public async Task<bool> UpdateFeedbackAsync(int feedbackId, CancellationToken cancellationToken = default)
        {
            return await _feedbackRepository.UpdateFeedbackAsync(feedbackId, cancellationToken);
        }

        public async Task<bool> DeleteFeedbackAsync(int feedbackId, CancellationToken cancellationToken = default)
        {
            return await _feedbackRepository.DeleteFeedbackAsync(feedbackId, cancellationToken);
        }

        public async Task<FeedBackDTOBLL> GetFeedbackAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var feedback = await _feedbackRepository.GetFeedbackAsync(orderId, cancellationToken);
            return _mapper.Map<FeedBackDTOBLL>(feedback);
        }

    }
}