using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MemberAdminUI.Models
{
    public class Membership
    {
        public int Id { get; set; }

        public double Discount { get; set; }

        [Required, MaxLength(50)]
        public string MembershipType { get; set; }

        [Required]
        public string Description { get; set; }

        public double Price { get; set; }

        public List<Member> Members { get; set; }
    }
}
