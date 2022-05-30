﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FlyBuy.Data;
using FlyBuy.Models;
using Microsoft.AspNetCore.Authorization;

namespace FlyBuy.Controllers
{
    public class AgeCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgeCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        public async Task<IActionResult> Index()
        {
              return _context.AgeCategories != null ? 
                          View(await _context.AgeCategories.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.AgeCategories'  is null.");
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] AgeCategory ageCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ageCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ageCategory);
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.AgeCategories == null)
            {
                return NotFound();
            }

            var ageCategory = await _context.AgeCategories.FindAsync(id);
            if (ageCategory == null)
            {
                return NotFound();
            }
            return View(ageCategory);
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] AgeCategory ageCategory)
        {
            if (id != ageCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ageCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgeCategoryExists(ageCategory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ageCategory);
        }

        [Authorize(Roles = "Admin,Manager,Worker")]
        [HttpPost]
        public JsonResult Delete(int? id)
        {
            var AgeCategories = _context.AgeCategories.Find(id);      
            _context.AgeCategories.Remove(AgeCategories);
            _context.SaveChanges();
            return new JsonResult(Ok());

        }

        private bool AgeCategoryExists(int id)
        {
          return (_context.AgeCategories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
