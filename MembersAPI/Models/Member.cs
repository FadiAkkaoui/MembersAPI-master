using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MembersAPI.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required, MaxLength(11), MinLength(11)]
        public string SocialSecurityNumber { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        [Required, MaxLength(50)]
        public string Email { get; set; }

        [Required, MaxLength(11)]
        public string PhoneNumber { get; set; }

        [Required, MaxLength(50), MinLength(6)]
        public string Password { get; set; }

        public DateTime StartDate { get; set; }

        public System.Nullable<DateTime> EndDate { get; set; }

        public int MemberShipId { get; set; }

        public Membership Membership { get; set; }

        public string ProfilePicture { get; set; }

        public bool Verified { get; set; }

    }
}
