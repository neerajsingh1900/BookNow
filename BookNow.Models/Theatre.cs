//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace BookNow.Models
//{
//    public class Theatre
//    {
//        [Key]
//        public int TheatreId { get; set; } // pk, increment


//        public string OwnerId { get; set; } = null!;


//        [ForeignKey("OwnerId")]
//        public virtual ApplicationUser Owner { get; set; } = null!;

//        [Required]
//        public string TheatreName { get; set; } = null!; // varchar


//        public int CityId { get; set; }


//        [ForeignKey("CityId")]
//        public virtual City City { get; set; } = null!;

//        public string Address { get; set; } = null!; // varchar

//        [StringLength(15)]
//        public string? PhoneNumber { get; set; } // varchar(15)

//        [EmailAddress]
//        public string Email { get; set; } = null!; // varchar [unique]

//        [StringLength(20)]
//        public string Status { get; set; } = "Active"; // varchar(20)

//        // Navigation collections
//        public virtual ICollection<Screen> Screens { get; set; } = new List<Screen>();
//    }
//}

using System;
using System.Collections.Generic;
using BookNow.Utility; 

namespace BookNow.Models
{
    public class Theatre 
    {
       public int TheatreId { get; private set; }

        public string OwnerId { get; private set; } = null!;
        public string TheatreName { get; private set; } = null!;
        public int CityId { get; private set; }
        public string Address { get; private set; } = null!;
        public string? PhoneNumber { get; private set; }
        public string Email { get; private set; } = null!;
        public string Status { get; private set; } = null!; 

        public virtual ApplicationUser Owner { get; private set; } = null!;
        public virtual City City { get; private set; } = null!;
        public virtual ICollection<Screen> Screens { get; set; } = new List<Screen>();


       private Theatre() { }
        public static Theatre CreateNew(
   
           string name, string email, string phone, int cityId, string address,
            string ownerId, string status)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Theatre name is required.");
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Theatre email is required.");
            if (cityId <= 0)
                throw new DomainException("Theatre must be assigned to a valid city.");
            if (string.IsNullOrWhiteSpace(ownerId))
                throw new DomainException("Theatre must have an owner ID.");

         
            return new Theatre
            {
                TheatreName = name,
                Email = email,
                PhoneNumber = phone,
                CityId = cityId,
                Address = address,
                OwnerId = ownerId,
                Status = status, 
            };
        }


     
        public void Approve()
        {
            if (Status == SD.Status_PendingApproval)
            {
                Status = SD.Status_Active;
            }
            else
            {
                throw new DomainException($"Cannot approve a theatre with current status: {Status}.");
            }
        }

        public void UpdateDetails(string name, string address, int cityId, string? phone, string email)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Name required");
            if (cityId <= 0) throw new DomainException("City required");
            if (string.IsNullOrWhiteSpace(email)) throw new DomainException("Email required");

            TheatreName = name;
            Address = address;
            CityId = cityId;
            PhoneNumber = phone;
            Email = email;
        }

    }
}