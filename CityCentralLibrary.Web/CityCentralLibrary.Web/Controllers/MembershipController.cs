using CityCentralLibrary.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;
using CityCentralLibrary.DAL.Models;

namespace CityCentralLibrary.Web.Controllers
{
    public class MembershipController : Controller
    {
        private readonly IRepository _repository;

        public MembershipController()
        {
            _repository = new Repository();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_repository.GetAllMembers());
        }

        [HttpGet]
        public IActionResult RegisterForMembership()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterForMembership(MemberInfo memberInfo)
        {
            if (!ModelState.IsValid)
            {
                return View(memberInfo);
            }
            try
            {
                bool result = _repository.RegisterForMembership(memberInfo);
                if (result)
                {
                    return View("RegisterSuccess", memberInfo);
                }
                ModelState.AddModelError("", "Unable to register membership.");
                return View(memberInfo);
            }
            catch(Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred.");
                return View(memberInfo);
            }
        }

        [HttpGet]
        public IActionResult CancelMembership()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CancelMembership(int registrationId)
        {
            try
            {
                bool result = _repository.CancelMembership(registrationId);
                if (result)
                {
                    ViewBag.Message = "Membership cancelled successfully.";
                    return View();
                }
                ViewBag.Message = "Registration ID not found.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "An unexpected error occurred";
                return View();
            }
        }

        [HttpGet]
        public IActionResult UpdateEmail()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UpdateEmail(int registrationId, string email)
        {
            try
            {
                bool result = _repository.UpdateEmail(registrationId, email);
                if (result)
                {
                    ViewBag.Message = "Email updated successfully.";
                    return View();
                }
                ViewBag.Message = "Registration ID not found.";
                return View();
            }
            catch (Exception)
            {
                ViewBag.Message = "An unexpected error occurred";
                return View();
            }
        }

        [HttpGet]
        public IActionResult SearchMember()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SearchMember(string searchTerm)
        {
            try
            {
                IEnumerable<MemberInfo> members = _repository.SearchMember(searchTerm);
                return View(members);
            }
            catch
            {
                ViewBag.Message = "An unexpected error occurred";
                return View();
            }
        }
    }
}
