using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace News.Models.VIewModels
{
    public class PostViewModel
    {
        public Post Post { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
