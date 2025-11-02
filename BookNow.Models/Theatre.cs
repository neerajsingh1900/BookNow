using System;
using System.Collections.Generic;


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