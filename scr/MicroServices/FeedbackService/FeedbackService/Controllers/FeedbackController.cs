using AutoMapper;
using FeedbackService.Api.DTO;
using FeedbackService.BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FeedbackService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeedbackController : ControllerBase
    {

        private readonly ILogger<FeedbackController> _logger;
        private readonly IFeedbackBLL _feedbackBLL;
        private readonly IMapper _mapper;

        public FeedbackController(ILogger<FeedbackController> logger, IFeedbackBLL feedbackBLL, IMapper mapper)
        {
            _logger = logger;
            _feedbackBLL = feedbackBLL;
            _mapper = mapper;
        }

        [HttpGet(nameof(GetFeedbackAsync))]
        public async Task<FeedBackDTOController> GetFeedbackAsync([FromHeader] int orderId, CancellationToken cancellationToken = default)
        {
            var feedback = await _feedbackBLL.GetFeedbackAsync(orderId);
            return _mapper.Map<FeedBackDTOController>(feedback);
        }
        
        [HttpPost(nameof(CreateFeedbackAsync))]
        public async Task<bool> CreateFeedbackAsync([FromHeader] string feedbaclStrings, CancellationToken cancellationToken = default)
        {
            return await _feedbackBLL.CreateFeedbackAsync(feedbaclStrings, cancellationToken);
        }
        [HttpPut(nameof(UpdateFeedbackAsync))]
        public async Task<bool> UpdateFeedbackAsync([FromHeader] int feedbackId, CancellationToken cancellationToken = default)
        {
            return await _feedbackBLL.UpdateFeedbackAsync(feedbackId, cancellationToken);
        }

        [HttpDelete(nameof(DeleteFeedbackAsync))]
        public async Task<bool> DeleteFeedbackAsync([FromHeader]  int feedbackId, CancellationToken cancellationToken = default)
        {
            return await _feedbackBLL.DeleteFeedbackAsync(feedbackId, cancellationToken);
        }

    }
}