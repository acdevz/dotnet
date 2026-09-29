using Azure.Core.Pipeline;
using LifelineHospital.DAL.Models;
using LifelineHospital.DAL.Repositories;
using LifeLineHospital.WEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LifeLineHospital.WEB.Controllers
{
    public class PatientController : Controller
    {
        private ILHMRepo _repository;
        public PatientController()
        {
            _repository = new LHMRepo();
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Patient patient)
        {
            bool result = _repository.AddPatient(patient);
            if (result)
            {
                return RedirectToAction("Details", new { id = patient.PatientId });
            }
            ViewBag.Message = "Failed to add patient.";
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            var patients = _repository.GetAllPatients();
            return View(patients);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var patient = _repository.GetPatientById(id);
            if (patient == null)
            {
                ViewBag.Message = "Patient Id not found.";
                return View();
            }
            return View(patient);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
