using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace News.Models.VIewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }

        public string givenName { get; set; }

        public string Picture { get; set; }

        public string Gender { get; set; }
        public string DOB { get; set; }
    }
}
