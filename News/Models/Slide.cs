using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace News.Models
{
    public class Slide
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }

        public string Title { get; set; }

        public string Image { get; set; }

        public bool ShowOnHome { get; set; }

        public DateTime CreateOn { get; set; }
    }
}
