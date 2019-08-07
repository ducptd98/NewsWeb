using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace News.Models.VIewModels
{
    public class CommentViewModel
    {
        public ParentComment ParentComment { get; set; }
        public List<ChildComment> ChildComments { get; set; }
    }
}
