using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using News.Models;

namespace News.Models.VIewModels
{
    public class HomePostViewModel
    {
        public List<Post> Posts { get; set; }
        public List<Category> Categories { get; set; }

        public List<Slide> Slides { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public List<CommentViewModel> CommentVM { get; set; }
    }
}
