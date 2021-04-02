using MailKit.Net.Smtp;
using MemberAdminUI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MemberAdminUI.Controllers
{
    [Authorize]
    public class MembersController : Controller
    {
        Logger log = LogManager.GetCurrentClassLogger();
        //private readonly string httpLocal = "http://localhost:50408/api";
         private readonly string httpServer = "http://193.10.202.72/membersAPI/api";
        
        // GET: MembersController
        public async Task<IActionResult> Index()
        {
            try
            {
                List<Member> members = new List<Member>();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Members"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        members = JsonConvert.DeserializeObject<List<Member>>(apiResponse);

                        //För att lista medlemskapsnamnet istället för id
                        List<Membership> memberships = await GetMembershipsAsync();
                        ViewData["MembershipType"] = memberships;
                    }
                }
                return View(members);
            }
            catch (Exception e)
            {
                log.Error("Det gick inte hämta medlemmarna " + e.Message);
                TempData["Result"] = "Ett fel uppstod";
                return RedirectToAction("Index");
            }

        }
        // GET: MembersController/Details/5
        public async Task<IActionResult> GetMember(int id)
        {
            try
            {
                Member member = new Member();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Members/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        member = JsonConvert.DeserializeObject<Member>(apiResponse);
                        log.Info("Medlemmar hämtade: Status: " + response.StatusCode);
                    }
                }
                //För att lista medlemskapsnamnet istället för id
                List<Membership> memberships = await GetMembershipsAsync();
                foreach (var membershipType in memberships)
                {
                    if (member.MemberShipId == membershipType.Id)
                    {
                        ViewBag.MembershipType = membershipType.MembershipType;
                    }
                }

                return View(member);
            }
            catch (Exception){
                TempData["Result"] = "Ett fel uppstod";
                return RedirectToAction("Index");
            } 
        }

        // GET: MembersController/Create
        public async Task<IActionResult> CreateMember()
        {
            List<Membership> memberships = await GetMembershipsAsync();
            ViewData["MembershipsId"] = new SelectList(memberships, "Id", "MembershipType");
            return View();
        }

        // POST: MembersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMember(Member member)
        { 
            //Sätta deafult-bild
            member.ProfilePicture = "http://193.10.202.72/membersAPI/images/React-ProfilePicture.png";

            try
            {
                string apiResponse = "";
                Member createMember = new Member();
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(member), Encoding.UTF8, "application/json");

                    using (var response = await httpClient.PostAsync(httpServer + "/Members", content))
                    {
                        apiResponse = await response.Content.ReadAsStringAsync();
                        createMember = JsonConvert.DeserializeObject<Member>(apiResponse);
                        log.Info("Medlem skapad: " + member.Id + " Status: " + response.StatusCode);
                    }
                }
                if(apiResponse.Contains("Bad Request"))
                {
                    TempData["Result"] = "Det gick inte registrera medlemmen, vänligen kontrollera Email och Personnummer.";
                    return View(createMember);
                }
                TempData["Result"] = "Medlemmen har lagts till!";
                return RedirectToAction("Index");
            }
            catch(Exception)
            {
                TempData["Result"] = "Ett fel uppstod";
                return RedirectToAction("Index");
            }
        }

        // GET: MembersController/Edit/5
        public async Task<IActionResult> EditMember(int id)
        {
            Member member = new Member();
            List<Membership> memberships = await GetMembershipsAsync();
            ViewData["MembershipsId"] = new SelectList(memberships, "Id", "MembershipType");
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Members/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        member = JsonConvert.DeserializeObject<Member>(apiResponse);
                    }
                }
                return View(member);
            }
            catch (Exception)
            {
                TempData["Result"] = "Ett fel uppstod, det gick inte hämta medlemmen";
                return RedirectToAction("Index");
            }
            

        }
        // POST: MembersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(Member member)
        {
            Member editMember = new Member();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Put, httpServer + $"/Members/{member.Id}")
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(member), Encoding.UTF8, "application/json")
                    };

                    using (var response = await httpClient.SendAsync(request))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        editMember = JsonConvert.DeserializeObject<Member>(apiResponse);
                        log.Info("Medlem uppdaterad " + member.Id + " Status: " + response.StatusCode);
                    }
                    TempData["Result"] = "Medlem uppdaterad!";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception)
            {
                TempData["Result"] = "Det gick inte uppdatera medlemmen";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> EditProfilePic(int id)
        {
            try
            {
                Member member = new Member();
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Members/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        member = JsonConvert.DeserializeObject<Member>(apiResponse);
                    }
                }
                ViewBag.Name = member.FirstName;
                ImgUpload imgUpload = new ImgUpload();

                imgUpload.Id = id;
                return View(imgUpload);
            }
            catch
            {
                TempData["Result"] = "Det gick inte uppdatera profilbilden";
                return RedirectToAction("Index");
            }

        }
        [HttpPost]
        public async Task<IActionResult> EditProfilePic(ImgUpload newImg)
        {
            try 
            {
                string apiResponse = "";
                using (var httpClient = new HttpClient())
                {
                    var formData = new MultipartFormDataContent();
                    using (var filestream = newImg.Img.OpenReadStream())
                    {
                        formData.Add(new StringContent(newImg.Id.ToString()), "Id");
                        formData.Add(new StreamContent(filestream), "Img", newImg.Img.FileName);
                        using (var response = await httpClient.PostAsync(httpServer + "/Members/img", formData))
                        {
                            apiResponse = await response.Content.ReadAsStringAsync();
                            log.Info("Medlems profilbild uppdaterad Status: " + response.StatusCode);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            
            catch (Exception)
            {
                TempData["Result"] = "Det gick inte uppdatera profilbilden";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> DeleteMemberConfirm(int id)
        {
            Member member = new Member();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Members/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        member = JsonConvert.DeserializeObject<Member>(apiResponse);
                    }
                }
                //För att lista medlemskapsnamnet istället för id
                List<Membership> memberships = await GetMembershipsAsync();
                foreach (var membershipType in memberships)
                {
                    if (member.MemberShipId == membershipType.Id)
                    {
                        ViewBag.MembershipType = membershipType.MembershipType;
                    }
                }
                return View(member);
            }
            catch (Exception)
            {
                TempData["Result"] = "Det gick inte hämta medlemmen";
                return RedirectToAction("Index");
            }
        }

        // POST: MembershipsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMember(int memberId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.DeleteAsync(httpServer + "/Members/" + memberId))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        log.Info("Medlem raderad: " + memberId + " Status: " + response.StatusCode);
                    }
                }
                TempData["Result"] = "Medlemmen raderad";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Result"] = "Det gick inte radera medlemmen";
                return RedirectToAction("Index");
            }
        }
        private async Task<List<Membership>> GetMembershipsAsync()
        {
            List<Membership> memberships = new List<Membership>();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(httpServer + "/Memberships"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    memberships = JsonConvert.DeserializeObject<List<Membership>>(apiResponse);
                }
            }
            return memberships;
        }
    }
}
