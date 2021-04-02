using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MembersAPI.Models
{
    public class MembersDBContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DbSet<Membership> Memberships { get; set; }

        public MembersDBContext(DbContextOptions options) : base(options)
        {

        }
    }
}
