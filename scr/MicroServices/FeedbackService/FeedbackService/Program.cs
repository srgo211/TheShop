
using AutoMapper;
using FeedbackService.Api;
using FeedbackService.Api.DTO;
using FeedbackService.BLL;
using FeedbackService.BLL.DTO;
using FeedbackService.BLL.Interfaces;
using FeedbackService.DAL.DTO;
using FeedbackService.DAL.Interfaces;
using FeedbackService.DAL.Repositories;

namespace FeedbackService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddAutoMapper(typeof(MappingProfile));
            builder.Services.AddSingleton<IFeedbackBLL, FeedbackBLL>();
            builder.Services.AddSingleton<IFeedbackRepository, FeedBackRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}