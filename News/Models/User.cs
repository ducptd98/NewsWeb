using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace News.Models
{
    public class User:IdentityUser
    {
        [Required]
        [Display(Name = "Full name")]
        public string Name { get; set; }

        public bool isAdmin { get; set; }
        public string Address { get; set; }

        public string Picture { get; set; } 

        public ICollection<Post> Posts { get; set; } = new List<Post>();

    }
}
