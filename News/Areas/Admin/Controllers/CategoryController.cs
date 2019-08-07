using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ContructorNews.Data;
using ElectronicStore.Ultilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using News.Models;

namespace News.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;

        public CategoryController(ApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            var lstCategory = _db.Categories.ToList();
            return View(lstCategory);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Category category)
        {
            if (ModelState.IsValid)
            {
                //category.Description = Request.Form["description"];
                _db.Categories.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(category);
        }
        public IActionResult Edit(long id)
        {
            var category = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(long id,Category category)
        {
            if (category.Id != id) return NotFound();
            if (ModelState.IsValid)
            {
                var categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
                categoryFromDb.Description = category.Description;
                categoryFromDb.CreatedDate = category.CreatedDate;
                categoryFromDb.ModifiedDate = category.ModifiedDate;
                categoryFromDb.Name = category.Name;
                categoryFromDb.ShowOnHome = category.ShowOnHome;

                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(long id)
        {
            var categoryFromDb = _db.Categories.Where(c => c.Id == id).FirstOrDefault();
            if (categoryFromDb == null) return NotFound();

            _db.Categories.Remove(categoryFromDb);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}