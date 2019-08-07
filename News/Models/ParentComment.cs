using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace News.Models
{
    public class ParentComment
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string Message { get; set; }
        public DateTime DateTime { get; set; }

        public long PostId { get; set; }
        [ForeignKey("PostId")]
        public Post Post { get; set; }

        public ICollection<ChildComment> ChildComments { get; set; } = new List<ChildComment>();
    }
}
