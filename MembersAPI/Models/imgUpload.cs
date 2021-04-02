using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MembersAPI.Models
{
    public class ImgUpload
    {
        public IFormFile Img { get; set; }
        public int Id{ get; set; }
    }
}
