using System.ComponentModel.DataAnnotations;

namespace CityCentralLibrary.Web.ViewModels
{
    public class MemberInfo
    {
        [Key]
        public int RegistrationID { get; set; }

        [Required]
        [StringLength(25)]
        public string MemberName { get; set; }

        [Required]
        [Range(10, 100)]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [StringLength(15)]
        public string MembershipType { get; set; }

        [Required]
        [StringLength(10)]
        [Phone]
        public string ContactNumber { get; set; }

        [Required]
        [StringLength(30)]
        [EmailAddress]
        public string EmailID { get; set; }
    }
}
