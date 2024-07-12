using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsCustom
{
    public class CusMod_Password
    {
        [Required(ErrorMessage = "Old password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        [Display(Name = "Old Password")]
        public string oldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        [Display(Name = "New Password")]
        public string newPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100)]
        [Display(Name = "Confirm Password")]
        public string confirmPassword { get; set; }
    }
}