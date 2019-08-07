using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace News.Models.VIewModels
{
    public class SinglePostViewModel
    {
        public Post Post { get; set; }
        public List<Category> Categories { get; set; }
    }
}
