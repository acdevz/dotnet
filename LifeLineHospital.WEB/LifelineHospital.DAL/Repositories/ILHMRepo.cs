using LifelineHospital.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LifelineHospital.DAL.Repositories
{
    public interface ILHMRepo
    {
        bool AddPatient(Patient patient);
        Patient? GetPatientById(int id);
        IEnumerable<Patient> GetAllPatients();
        bool AddAppointment(Appointment appointment);
        IEnumerable<Appointment> GetAllAppointments();
    }
}
