using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLKSMVC.Data;
using QLKSMVC.Models;

namespace QLKSMVC.Controllers
{
    public class LoaiDichVuController : Controller
    {
        private readonly QuanLyKhachSanDbContext _context;

        public LoaiDichVuController(QuanLyKhachSanDbContext context)
        {
            _context = context;
        }

        // GET: LoaiDichVu
        public async Task<IActionResult> Index()
        {
              return _context.LoaiDichVus != null ? 
                          View(await _context.LoaiDichVus.ToListAsync()) :
                          Problem("Entity set 'QuanLyKhachSanDbContext.LoaiDichVus'  is null.");
        }

        // GET: LoaiDichVu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.LoaiDichVus == null)
            {
                return NotFound();
            }

            var loaiDichVuModel = await _context.LoaiDichVus
                .FirstOrDefaultAsync(m => m.MaLdv == id);
            if (loaiDichVuModel == null)
            {
                return NotFound();
            }

            return View(loaiDichVuModel);
        }

        // GET: LoaiDichVu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LoaiDichVu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaLdv,TenLdv,MoTa")] LoaiDichVuModel loaiDichVuModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loaiDichVuModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(loaiDichVuModel);
        }

        // GET: LoaiDichVu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.LoaiDichVus == null)
            {
                return NotFound();
            }

            var loaiDichVuModel = await _context.LoaiDichVus.FindAsync(id);
            if (loaiDichVuModel == null)
            {
                return NotFound();
            }
            return View(loaiDichVuModel);
        }

        // POST: LoaiDichVu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaLdv,TenLdv,MoTa")] LoaiDichVuModel loaiDichVuModel)
        {
            if (id != loaiDichVuModel.MaLdv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loaiDichVuModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoaiDichVuModelExists(loaiDichVuModel.MaLdv))
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
            return View(loaiDichVuModel);
        }

        // GET: LoaiDichVu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.LoaiDichVus == null)
            {
                return NotFound();
            }

            var loaiDichVuModel = await _context.LoaiDichVus
                .FirstOrDefaultAsync(m => m.MaLdv == id);
            if (loaiDichVuModel == null)
            {
                return NotFound();
            }

            return View(loaiDichVuModel);
        }

        // POST: LoaiDichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.LoaiDichVus == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.LoaiDichVus'  is null.");
            }
            var loaiDichVuModel = await _context.LoaiDichVus.FindAsync(id);
            if (loaiDichVuModel != null)
            {
                _context.LoaiDichVus.Remove(loaiDichVuModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoaiDichVuModelExists(int id)
        {
          return (_context.LoaiDichVus?.Any(e => e.MaLdv == id)).GetValueOrDefault();
        }
    }
}
