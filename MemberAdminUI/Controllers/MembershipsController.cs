using MemberAdminUI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using NLog;

namespace MemberAdminUI.Controllers
{
    [Authorize]
    public class MembershipsController : Controller
    {
        Logger log = LogManager.GetCurrentClassLogger();
          // private readonly string httpLocal = "http://localhost:50408/api";
          private readonly string httpServer = "http://193.10.202.72/membersAPI/api";

        // GET: MembershipsController
        public async Task<IActionResult> Index()
        {
            List<Membership> memberships = new List<Membership>();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Memberships"))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        memberships = JsonConvert.DeserializeObject<List<Membership>>(apiResponse);
                    }
                }
                return View(memberships);
            }
            catch (Exception e)
            {
                log.Error("Det gick inte ansluta till API: " + e.Message);
                TempData["Result"] = "Det gick inte hämta medlemskapen";
                return View();
            }
        }

        // GET: MembershipsController/Details
        public async Task<IActionResult> GetMembership(int id)
        {
            Membership membership = new Membership();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Memberships/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        membership = JsonConvert.DeserializeObject<Membership>(apiResponse);
                    }
                }
                log.Info("Medlemskap returnerad");
                return View(membership);
            }
            catch (Exception)
            {
                log.Error("Misslyckade att hämta medlemsskapet: " + id);
                TempData["Result"] = "Det gick inte hämta medlemskapet";
                return RedirectToAction("Index");
            }
        }


        // GET: MembershipsController/Create
        public ViewResult CreateMembership() => View();

        // POST: MembershipsController/Create
        [HttpPost]
        public async Task<IActionResult> CreateMembership(Membership membership)
        {
            Membership newMembership = new Membership();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(membership), Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PostAsync(httpServer + "/Memberships", content))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        newMembership = JsonConvert.DeserializeObject<Membership>(apiResponse);
                        log.Info("Medlemskap skapat " + membership.Id + " Status: " + response.StatusCode);
                    }
                }
                TempData["Result"] = "Medlemskapet skapades";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["Result"] = "Det gick inte skapa medlemskapet";
                log.Info("Gick ej att skapa medlemskapsid: " + membership.Id);
                return RedirectToAction("Index");
            }
        }

        // GET: MembershipsController/Edit
        public async Task<IActionResult> EditMembership(int id)
        {
            Membership membership = new Membership();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Memberships/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        membership = JsonConvert.DeserializeObject<Membership>(apiResponse);
                    }
                }
                return View(membership);
            }
            catch (Exception)
            {
                log.Error("Det gick inte uppdatera medlemsskapet: " + id);
                TempData["Result"] = "Det gick hämta medlemskapet";
                return RedirectToAction("Index");
            }
        }

        // POST: MembershipsController/Edit/5
        [HttpPost]
        public async Task<IActionResult> EditMembership(int id, Membership membership)
        {
            Membership editedMembership = new Membership();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var request = new HttpRequestMessage(HttpMethod.Put, httpServer + $"/Memberships/{membership.Id}")
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(membership), Encoding.UTF8, "application/json")
                    };

                    using (var response = await httpClient.SendAsync(request))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        editedMembership = JsonConvert.DeserializeObject<Membership>(apiResponse);
                        log.Info("Medlemskap uppdaterad " + membership.Id + " Status: " + response.StatusCode);
                    }
                    TempData["Result"] = "Medlemskapet uppdaterades";
                    return RedirectToAction("Index");
                }

            }
            catch (Exception)
            {
                TempData["Result"] = "Det gick inte uppdatera medlemskapet";
                log.Info("Gick ej att uppgradera medlemskapsid: " + membership.Id);
                return RedirectToAction("Index");
            }

        }
        // GET: MembershipsController/Delete
        public async Task<IActionResult> DeleteMembershipConfirm(int id)
        {
            Membership membership = new Membership();
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.GetAsync(httpServer + "/Memberships/" + id))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        membership = JsonConvert.DeserializeObject<Membership>(apiResponse);
                    }
                }
                return View(membership);
            }
            catch (Exception)
            {
                TempData["Result"] = "Det gick inte hämta medlemskapet";
                return RedirectToAction("Index");
            }
        }

        // POST: MembershipsController/Delete
        [HttpPost]
        public async Task<IActionResult> DeleteMembership(int membershipId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    using (var response = await httpClient.DeleteAsync(httpServer + "/Memberships/" + membershipId))
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        log.Info("Medlemskap raderad: " + membershipId + " Status: " + response.StatusCode);
                    }
                }
                TempData["Result"] = "Medlemskapet raderades";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                log.Error("Det gick inte radera medlemsskapet: " + membershipId);
                TempData["Result"] = "Medlemskapet kunde inte raderades";
                return RedirectToAction("Index");
            }
        }
    }
}