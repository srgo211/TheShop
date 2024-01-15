using AutoMapper;
using FeedbackService.Api.DTO;
using FeedbackService.DAL.DTO;
using FeedbackService.BLL.DTO;

namespace FeedbackService.Api
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<FeedBackDTOBLL, FeedBackDTOController>();
            CreateMap<FeedbackDTODAL, FeedBackDTOBLL>();
        }
    }
}
