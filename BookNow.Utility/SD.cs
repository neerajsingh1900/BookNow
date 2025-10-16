using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Utility
{
    public static class SD
    {
        public const string Role_Customer = "Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Producer = "Producer";
        public const string Role_Theatre_Owner = "TheatreOwner";

        public const string Status_PendingApproval = "PendingApproval";
        public const string Status_Active = "Active";
        public const string Status_Maintenance = "Maintenance";

        // ➡️ NEW: Seat State Constants ⬅️
        public const string State_Available = "Available";
        public const string State_Reserved = "Reserved"; // For pre-booking/cart holding
        public const string State_Booked = "Booked";
    }

        public class EmailSettings
        {
            public string SmtpHost { get; set; }
            public int SmtpPort { get; set; }
            public string SmtpUser { get; set; }
            public string SmtpPass { get; set; }
            public string FromEmail { get; set; }
            public string FromName { get; set; }
        }
    
}
