using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace News.Models
{
    public class ChildComment
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public long ParentCommentId { get; set; }
        [ForeignKey("ParentCommentId")]
        public ParentComment ParentComment { get; set; }
    }
}
