using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContructorNews.Data;
using ElectronicStore.Ultilities;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Mvc;
using News.Models;

namespace News.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        public SlideController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {

            return View(_db.Slides.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Slide slide)
        {
            if (!ModelState.IsValid) return View(slide);
            _db.Slides.Add(slide);
            await _db.SaveChangesAsync();

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var slideFromDb = _db.Slides.Find(slide.Id);

            if (files.Count != 0)
            {
                var uploads = Path.Combine(webRootPath, SD.ImageSlideFolderPost);
                var extension = Path.GetExtension(files[0].FileName);

                using (var filestream = new FileStream(Path.Combine(uploads, slide.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filestream);
                }

                slideFromDb.Image = @"/" + SD.ImageSlideFolderPost + @"/" + slide.Id + extension;
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
            var slideFromDb = _db.Slides.SingleOrDefault(s => s.Id == id);
            if (slideFromDb == null) return NotFound();

            return View(slideFromDb);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(long id,Slide slide)
        {
            if (id != slide.Id) return NotFound();
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                var slideFromDb = _db.Slides.FirstOrDefault(m => m.Id == slide.Id);

                if (files.Count > 0 && files[0] != null)
                {
                    //if user uploads a new image
                    var uploads = Path.Combine(webRootPath, SD.ImageSlideFolderPost);
                    var extension_new = Path.GetExtension(files[0].FileName);
                    var extension_old = Path.GetExtension(slideFromDb.Image);

                    if (System.IO.File.Exists(Path.Combine(uploads, slide.Id + extension_old)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, slide.Id + extension_old));
                    }
                    using (var filestream = new FileStream(Path.Combine(uploads, slide.Id + extension_new), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    slideFromDb.Image = @"/" + SD.ImageSlideFolderPost + @"/" + slide.Id + extension_new;

                }

                slideFromDb.Name = slide.Name;
                slideFromDb.CreateOn = slide.CreateOn;
                slideFromDb.Title = slide.Title;
                slideFromDb.ShowOnHome = slideFromDb.ShowOnHome;

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(slide);
        }
        public async Task<IActionResult> Delete(long id)
        {
            var slideFromDb = _db.Slides.Where(c => c.Id == id).FirstOrDefault();
            if (slideFromDb == null) return NotFound();

            _db.Slides.Remove(slideFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}