using System.ComponentModel.DataAnnotations;


namespace LifelineHospital.WEB.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [StringLength(20, MinimumLength = 4)]
        [Required]
        public string PatientName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(35)]
        [Phone]
        public string ContactNo { get; set; } = string.Empty;
    }
}
