﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberAdminUI.Models
{
    public class AdminLogin
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleId { get; set; }
    }
}