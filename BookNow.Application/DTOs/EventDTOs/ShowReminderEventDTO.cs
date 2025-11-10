using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.EventDTOs
{
    public class ShowReminderEventDTO
    {
        public int BookingId { get; set; }
        public string UserId { get; set; }
        public DateTime TriggerAtUtc { get; set; } 
    }
}
