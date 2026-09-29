using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LifelineHospital.DAL.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [StringLength(35)]
        [Required]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(35)]
        [Phone]
        public string ContactNo { get; set; } = string.Empty;

        public List<Appointment> Appointments { get; set; }
    }
}
