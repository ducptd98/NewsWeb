using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ContructorNews.Data;
using ElectronicStore.Ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using News.Models;

namespace News.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly HostingEnvironment _hostingEnvironment;

        public UserController(ApplicationDbContext db, HostingEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View(_db.Users.ToList());
        }


        public IActionResult Edit(string id)
        {
            if (id == null || id.Trim().Length == 0)
            {
                return NotFound();
            }

            var userFromDb =  _db.Users.Find(id);

            if (userFromDb == null) return NotFound();
            return View(userFromDb);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,User user)
        {
            if (id != user.Id) return NotFound();
            if (ModelState.IsValid)
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                User userFromDb = _db.Users.FirstOrDefault(u => u.Id == id);
                if (files.Count != 0)
                {
                    var uploads = Path.Combine(webRootPath, SD.ImageUserFolder);
                    var extension = Path.GetExtension(files[0].FileName);

                    if (System.IO.File.Exists(Path.Combine(uploads, userFromDb.Name + extension)))
                    {
                        System.IO.File.Delete(Path.Combine(uploads, userFromDb.Name + extension));
                    }

                    using (var filestream = new FileStream(Path.Combine(uploads, userFromDb.Name + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }

                    userFromDb.Picture = @"\" + SD.ImageUserFolder + @"\" + userFromDb.Name + extension;
                }
                else
                {
                    //when user does not upload image
                    var uploads = Path.Combine(webRootPath, SD.ImageUserFolder + @"\" + SD.DefaultUserImage);
                    System.IO.File.Copy(uploads, webRootPath + @"\" + SD.ImageUserFolder + @"\" + userFromDb.Name + ".png");
                    userFromDb.Picture = @"\" + SD.ImageUserFolder + @"\" + userFromDb.Name + ".png";
                }
               
                userFromDb.Name = user.Name;
                userFromDb.Address = user.Address;
                userFromDb.PhoneNumber = user.PhoneNumber;
                userFromDb.isAdmin = user.isAdmin;

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }
        public async Task<IActionResult> Delete(string id)
        {
            User userFromDb = _db.Users.Where(u => u.Id == id).FirstOrDefault();
            userFromDb.LockoutEnd = DateTime.Now.AddYears(1000);

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}