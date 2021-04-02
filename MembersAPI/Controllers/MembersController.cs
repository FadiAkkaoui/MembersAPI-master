using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MembersAPI.Models;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using MimeKit;
using MailKit.Net.Smtp;
using NLog;

namespace MembersAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly MembersDBContext _context;
        private readonly IWebHostEnvironment _enviroment;
        Logger log = LogManager.GetCurrentClassLogger();

        public MembersController(MembersDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _enviroment = environment;

        }

        // GET: api/Members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Member>>> GetMembers()
        {
            return await _context.Members.ToListAsync();
        }

        // GET: api/Members/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(int id)
        {
            try
            {
                var member = await _context.Members.FindAsync(id);

                if (member == null)
                {
                    return NotFound();
                }

                return member;
            }
            catch(Exception)
            {
                return NotFound();
            }

        }

        // PUT: api/Members/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMember(int id, Member member)
        {
            if (id != member.Id)
            {
                return BadRequest();
            }

            _context.Entry(member).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MemberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Members
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Member>> PostMember(Member newMember)
        {
            try
            {
                var validateMember = _context.Members.Where(m => m.Email == newMember.Email || m.SocialSecurityNumber == newMember.SocialSecurityNumber).FirstOrDefault();
                if (validateMember == null)
                {
                    _context.Members.Add(newMember);
                    await _context.SaveChangesAsync();
                    SendMail(newMember);
                    return NoContent();
                }
                return BadRequest();
            }
            catch(Exception)
            {
                return BadRequest();
            }



        }

        // DELETE: api/Members/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            try
            {
                var member = await _context.Members.FindAsync(id);
                if (member == null)
                {
                    return NotFound();
                }

                _context.Members.Remove(member);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception)
            {
                return BadRequest();
            }

        }

        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }

        [HttpPost]
        [Route("Login")]
        public Member Login(UserCheck userlogin)
        {
            try
            {
                Member validation = _context.Members.Where(m => m.Email.ToUpper() == userlogin.Email.ToUpper() && m.Password == userlogin.Password).FirstOrDefault();
                if (validation != null)
                {
                    log.Info("Lyckad inloggning " + validation.Email);
                    return validation;
                }
                else
                {
                    log.Error("Misslyckad inloggning " + userlogin.Email);
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        [HttpPost]
        [Route("img")]
        public ImgUpload PostImg([FromForm] ImgUpload img)
        {
            string WebbSiteUrl = "http://193.10.202.72/membersAPI/uploads/";
            try
            {
                string path = _enviroment.WebRootPath + "\\uploads\\";
                string fileName = img.Img.FileName.ToLower();
                //Kontrollera så filen inte är större än 5mb och att filen är en bild
                if (img.Img.Length > 0 && img.Img.Length < 5000000 && (fileName.Contains("jpg") || fileName.Contains("png")))
                {
                    Member setImg = _context.Members.Where(m => m.Id == img.Id).FirstOrDefault();
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);

                    }
                    using (FileStream fileStream = System.IO.File.Create(path + img.Img.FileName))
                    {

                        if (setImg != null)
                        {
                            var member = _context.Members.Find(img.Id);
                            member.ProfilePicture = WebbSiteUrl + img.Img.FileName;
                            _context.SaveChanges();
                        }
                        img.Img.CopyTo(fileStream);
                        fileStream.Flush();
                        return img;
                    }
                }
                else
                {
                    log.Error("Medlemmen " + img.Id + "misslyckade ladda upp bild");
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SendMail(Member member)
        {
            try
            {
                string gym = "React gym";
                string subject = "Verifiering av ditt medlemskap";
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("info.reactgym@gmail.com"));
                message.To.Add(new MailboxAddress(member.Email));
                message.Subject = subject;
                message.Body = new TextPart("html")
                {
                    Text = "Från: " + gym + "<br>" +
                    "Kontaktinfo: " + message.From +
                    "<br>" +
                    "<br>" +
                    "Hej " + member.FirstName + " " + member.LastName + "!" +
                    "<br>" +
                    "<br>" +
                    message.Body + " Välkommen till React. Du är nu registrerad hos oss!" +
                    "<br>" +
                    "<br>" +
                    "<br>" +
                    "Mvh React teamet!"
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("info.reactgym@gmail.com", "react1234");
                    client.Send(message);
                    client.Disconnect(true);
                }
                
            }
            catch(Exception e)
            {
                log.Error("Misslyckade skicka mail till " + member.Email + " " + e.Message);
                throw;
            }
        }
    }
}
