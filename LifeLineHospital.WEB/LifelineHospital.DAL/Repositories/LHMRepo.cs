using System;
using System.Collections.Generic;
using System.Text;
using LifelineHospital.DAL.Data;
using LifelineHospital.DAL.Models;

namespace LifelineHospital.DAL.Repositories
{
    public class LHMRepo : ILHMRepo
    {
        public bool AddPatient(Patient patient)
        {
            try
            {
                using var db = new LHMDbContext();
                db.Patients.Add(patient);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Patient? GetPatientById(int id)
        {
            try
            {
                using var db = new LHMDbContext();
                var patient = db.Patients.FirstOrDefault(p => p.PatientId == id);
                return patient;
            }
            catch
            {
                return null;
            }
        }
        public IEnumerable<Patient> GetAllPatients()
        {
            try
            {
                using var db = new LHMDbContext();
                var patients = db.Patients.ToList();
                return patients;
            }
            catch
            {
                return Enumerable.Empty<Patient>();
            }
        }
        public bool AddAppointment(Appointment appointment)
        {
            try
            {
                using var db = new LHMDbContext();
                db.Appointments.Add(appointment);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public IEnumerable<Appointment> GetAllAppointments()
        {
            try
            {
                using var db = new LHMDbContext();
                var appointments = db.Appointments.ToList();
                return appointments;
            }
            catch
            {
                return Enumerable.Empty<Appointment>();
            }
        }
    }
}
