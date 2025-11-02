using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Application.DTOs.CustomerDTOs.BookingDTOs
{
    public class SeatStatusDTO
    {
        public int SeatInstanceId { get; set; }
        public int SeatId { get; set; }
        public string SeatNumber { get; set; }
        public string RowLabel { get; set; }
        public int SeatIndex { get; set; }
        public string State { get; set; }
        public decimal Price { get; set; }
        public byte[] RowVersion { get; set; }
    }
}
