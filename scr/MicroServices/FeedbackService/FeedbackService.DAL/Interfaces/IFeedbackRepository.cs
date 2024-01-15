using FeedbackService.DAL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeedbackService.DAL.Interfaces
{
    public interface IFeedbackRepository
    {
        /// <summary>
        /// Создание отзыва
        /// </summary>
        /// <param name="feedbaclStrings">Отзыв</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> CreateFeedbackAsync(string feedbaclStrings, CancellationToken cancellationToken = default);
        /// <summary>
        /// Обновление отзыва
        /// </summary>
        /// <param name="feedbackId">Id отзыва</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> UpdateFeedbackAsync(int feedbackId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Удаление отзыва
        /// </summary>
        /// <param name="feedbackId">Id отзыва</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> DeleteFeedbackAsync(int feedbackId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Получение отзыва
        /// </summary>
        /// <param name="orderId">Id заказа</param>
        /// <param name=""></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<FeedbackDTODAL> GetFeedbackAsync(int orderId , CancellationToken cancellationToken = default);
    }
}
