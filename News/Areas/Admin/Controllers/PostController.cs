using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContructorNews.Data;
using ElectronicStore.Ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using News.Models;
using News.Models.VIewModels;

namespace News.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty] public PostViewModel PostVM { get; set; }

        public PostController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            PostVM = new PostViewModel()
            {
                Post = new Post(),
                Categories = _db.Categories.ToList()
            };
        }
        public IActionResult Index()
        {
            var lstPost = _db.Posts.Include(p => p.Category).ToList();
            return View(lstPost);
        }

        public IActionResult Create()
        {
            return View(PostVM);
        }
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (!ModelState.IsValid)
            {
                return View(PostVM);
            }

            PostVM.Post.Content = Request.Form["content"];
            _db.Posts.Add(PostVM.Post);
            await _db.SaveChangesAsync();

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var postFromDb = _db.Posts.Find(PostVM.Post.Id);

            if (files.Count != 0)
            {
                var uploads = Path.Combine(webRootPath, SD.ImageFolderPost);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads,PostVM.Post.Id + extension),FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                postFromDb.Image = @"/" + SD.ImageFolderPost + @"/" + PostVM.Post.Id + extension;
            }
            else
            {
                
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(long? id)
        {
            if (id == null) return NotFound();
            PostVM.Post = _db.Posts.Include(p=>p.Category).SingleOrDefault(p=>p.Id==id);
            if (PostVM.Post == null)
            {
                return NotFound();
            }
            return View(PostVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id)
        {
            if (id != PostVM.Post.Id) return NotFound("No entity");
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var postFromDb = _db.Posts.FirstOrDefault(m => m.Id == PostVM.Post.Id);

                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image
                    var uploads = Path.Combine(webRootPath, SD.ImageFolderPost);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(postFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, PostVM.Post.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, PostVM.Post.Id + extension_old));
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, PostVM.Post.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    PostVM.Post.Image = @"/" + SD.ImageFolderPost + @"/" + PostVM.Post.Id + extension_new;
                    
                }
                if (PostVM.Post.Image != null)
                {
                    postFromDb.Image = PostVM.Post.Image;
                }
                PostVM.Post.Content = Request.Form["content"];
                postFromDb.Name = PostVM.Post.Name;
                postFromDb.CategoryId = PostVM.Post.CategoryId;
                postFromDb.Title = PostVM.Post.Title;
                postFromDb.ShortDescription = PostVM.Post.ShortDescription;
                postFromDb.Content = PostVM.Post.Content;
                postFromDb.CreateOn = PostVM.Post.CreateOn;
                postFromDb.ModifiedOn = PostVM.Post.ModifiedOn;
                postFromDb.ShowOnHome = PostVM.Post.ShowOnHome;
                postFromDb.UpComming = PostVM.Post.UpComming;

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(PostVM);
        }

        public async Task<IActionResult> Delete(long id)
        {
            var postFromDb = _db.Posts.Where(c => c.Id == id).FirstOrDefault();
            if (postFromDb == null) return NotFound();

            _db.Posts.Remove(postFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}