using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace documentmgr.dto
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [StringLength(100)]
        [Display(Name = "Middle name")]
        public string MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public  List<FileUploadDto> Documents { get; set; } = new List<FileUploadDto>();
    }

    public class SearchUserDto
    {
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        [Display(Name = "Middle name")]
        public string MiddleName { get; set; }
        [Display(Name = "Email address")]
        public string Email { get; set; }

        public List<FileSearchDto> Documents { get; set; } = new List<FileSearchDto>();
        public int Id { get; set; }
        public string TransactionNumber { get; set; }
    }

}
