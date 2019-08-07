using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ContructorNews.Data;
using Microsoft.AspNetCore.Mvc;
using ContructorNews.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.EntityFrameworkCore;
using News.Models;
using News.Models.VIewModels;

namespace ContructorNews.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        [BindProperty]
        public HomePostViewModel HomePostViewModel { get; set; }

        private int pageSize = 5;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
            HomePostViewModel = new HomePostViewModel()
            {
                Categories = _db.Categories.Include(c=>c.Posts).Where(c=>c.ShowOnHome).ToList(),
                Posts = _db.Posts.Include(p=>p.Category).Where(p=>p.ShowOnHome).ToList(),
                Slides = _db.Slides.Where(s=>s.ShowOnHome).ToList(),
                CommentVM = new List<CommentViewModel>()
            };
        }
        public IActionResult Index()
        {
            return View(HomePostViewModel);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Archive(long id, int pageNumber = 1)
        {
            HomePostViewModel homePostVM = new HomePostViewModel()
            {
                Posts = _db.Posts.Include(p => p.Category).Where(p => p.CategoryId == id && p.ShowOnHome).ToList(),
                Categories = _db.Categories.Include(c => c.Posts).ToList()
            };
            StringBuilder param = new StringBuilder();

            param.Append("/Customer/Home/Archive/"+id+"?pageNumber=:");

            var count = homePostVM.Posts.Count;
            homePostVM.Posts = homePostVM.Posts.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            homePostVM.PagingInfo = new PagingInfo()
            {
                Curpage = pageNumber,
                ItemPerPage = pageSize,
                TotalItems = count,
                urlParam = param.ToString()
            };

            return View(homePostVM);
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult SinglePost(long id)
        {
            var postFromDb = _db.Posts.Include(c => c.Category).FirstOrDefault(p => p.Id == id);
            if (Request.Cookies["ViewedPost"] == null)
            {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddDays(1);
                
                Response.Cookies.Append("ViewedPost","1", option);
                postFromDb.CountView++;
                _db.SaveChanges();
            }

            var userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["User"] = _db.Users.Where(u => u.Id == userid).SingleOrDefault();




            var lstParentComment = _db.ParentComments.Include(pc=>pc.Post)
                .Include(pc=>pc.User).Where(pc => pc.PostId == id).OrderByDescending(d=>d.DateTime).ToList();
            foreach (var parentComment in lstParentComment)
            {
                CommentViewModel comment = new CommentViewModel
                {
                    ParentComment = parentComment,
                    ChildComments = _db.ChildComments.Include(cc=>cc.ParentComment)
                        .Include(cc=>cc.User).Where(cc=>cc.ParentCommentId==parentComment.Id).ToList()
                };
                HomePostViewModel.CommentVM.Add(comment);
            }

            HomePostViewModel.Posts = _db.Posts.Include(p => p.Category).Where(p => p.Id == id).ToList();

            return View(HomePostViewModel);
        }
        public IActionResult SubmitVideo()
        {
            return View();
        }
        public IActionResult VideoPost()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public string[] NewCommentDetails(User user)
        {
            string[] newCommentDetails = new string[3];
            newCommentDetails[0] = "td" + user.Name; //comment Text
            newCommentDetails[1] = "tdc" + user.Name; //comment Text div
            newCommentDetails[2] = "tb" + user.Name; //comment text btn
            return newCommentDetails;
        }

        public string[] CommentDetails(ParentComment comment)
        {
            string[] commentDetails = new string[16];
            commentDetails[0] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(comment.User.Name); //username
            commentDetails[1] = comment.User.Picture; //imgUrl
            commentDetails[2] = comment.DateTime.ToShortDateString(); // datetime
            commentDetails[3] = "gp" + comment.Id; //grandparentId
            commentDetails[4] = "mc" + comment.Id; //maincommentid
            commentDetails[5] = "cpr" + comment.Id; //childCommentId
            commentDetails[6] = "cex" + comment.Id; //commentExpid
            commentDetails[7] = "ctex" + comment.Id; //ctrlExpid
            commentDetails[8] = "ctflg" + comment.Id; //ctrlFlagId
            commentDetails[9] = "sp" + comment.Id; //shareParentId
            commentDetails[10] = "sc" + comment.Id; //shareChildId
            commentDetails[11] = "td" + comment.Id; //comText
            commentDetails[12] = "tdc" + comment.Id; //comTextdiv
            commentDetails[13] = "rpl" + comment.Id; //Reply
            commentDetails[14] = "cc1" + comment.Id; //commentControl
            commentDetails[15] = "cc2" + comment.Id; //commentMenu

            return commentDetails;
        }
        public string[] ReplyDetails(ChildComment comment)
        {
            string[] commentDetails = new string[16];
            commentDetails[0] = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(comment.User.Name); //username
            commentDetails[1] = comment.User.Picture; //imgUrl
            commentDetails[2] = comment.DateTime.ToShortDateString(); // datetime
            commentDetails[3] = "gp" + comment.Id; //grandparentId
            commentDetails[4] = "mc" + comment.Id; //maincommentid
            commentDetails[5] = "cpr" + comment.Id; //childCommentId
            commentDetails[6] = "cex" + comment.Id; //commentExpid
            commentDetails[7] = "ctex" + comment.Id; //ctrlExpid
            commentDetails[8] = "ctflg" + comment.Id; //ctrlFlagId
            commentDetails[9] = "sp" + comment.Id; //shareParentId
            commentDetails[10] = "sc" + comment.Id; //shareChildId
            commentDetails[11] = "td" + comment.Id; //comText
            commentDetails[12] = "tdc" + comment.Id; //comTextdiv
            commentDetails[13] = "rpl" + comment.Id; //Reply
            commentDetails[14] = "cc1" + comment.Id; //commentControl
            commentDetails[15] = "cc2" + comment.Id; //commentMenu

            return commentDetails;
        }


        public IActionResult AddComments(long postId)
        {

            return RedirectToAction("SinglePost", postId);
        }
    }
}
