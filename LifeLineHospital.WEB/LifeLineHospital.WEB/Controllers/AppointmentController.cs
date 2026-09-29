using Azure.Core.Pipeline;
using LifelineHospital.DAL.Models;
using LifelineHospital.DAL.Repositories;
using LifeLineHospital.WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LifeLineHospital.WEB.Controllers
{
    public class AppointmentController : Controller
    {
        private ILHMRepo _repository;
        public AppointmentController()
        {
            _repository = new LHMRepo();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Appointment appointment)
        {
            bool result = _repository.AddAppointment(appointment);
            if (result)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Message = "Failed to add patient.";
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var appointments = _repository.GetAllAppointments();
            return View(appointments);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
