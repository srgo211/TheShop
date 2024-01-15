using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeedbackService.DAL.Enums;

namespace FeedbackService.DAL.DTO
{
    public class FeedbackDTODAL
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
        public  Score Score { get; set; }
    }
}
