using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MemberAdminUI.Models
{
    public class ImgUpload
    {
        public int Id { get; set; }
        public IFormFile Img { get; set; }
    }
}
