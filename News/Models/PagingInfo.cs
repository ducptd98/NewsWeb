using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace News.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemPerPage { get; set; }
        public int Curpage { get; set; }

        public int totalPage => (int) Math.Ceiling((decimal) TotalItems / ItemPerPage);

        //This will be used to build URL
        public string urlParam { get; set; }
        public int FirstRowOnPage
        {
            get { return (Curpage - 1) * ItemPerPage + 1; }
        }

        public int LastRowOnPage
        {
            get { return Math.Min(Curpage * ItemPerPage, totalPage); }
        }
    }
}
