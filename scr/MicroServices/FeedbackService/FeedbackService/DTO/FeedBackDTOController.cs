using FeedbackService.DAL.Enums;

namespace FeedbackService.Api.DTO
{
    public class FeedBackDTOController
    {
        /// <summary>
        /// ID отзыва
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// ID заказа, к которому принадлежит отзыв
        /// </summary>
        public int orderId { get; set; }
        /// <summary>
        /// Сообщение отзыва
        /// </summary>
        public string feedbackStrings { get; set; }
        /// <summary>
        /// Оценка
        /// </summary>
        public Score Score { get; set; }

    }
}
