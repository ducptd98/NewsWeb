using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace News.Models
{
    public class Post
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

        public string Image { get; set; }

        public string ShortDescription { get; set; }

        public string Content { get; set; }
        public DateTime CreateOn { get; set; }
        public DateTime ModifiedOn { get; set; }

        public int ViewsCount { get; set; }
        public bool UpComming { get; set; }

        public bool ShowOnHome { get; set; }

        public long CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        //UserId
        public string UserId { get; set; }
        [ForeignKey("UserId")] public virtual User CreateBy { get; set; }

        public int CountView { get; set; }
        public int CountLike { get; set; }
        public int CountShare { get; set; }

        public ICollection<ParentComment> ParentComments { get; set; } = new List<ParentComment>();
    }
}
