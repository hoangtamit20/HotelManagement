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
    public class ChucVuController : Controller
    {
        private readonly QuanLyKhachSanDbContext _context;

        public ChucVuController(QuanLyKhachSanDbContext context)
        {
            _context = context;
        }

        // GET: ChucVu
        public async Task<IActionResult> Index()
        {
              return _context.ChucVus != null ? 
                          View(await _context.ChucVus.ToListAsync()) :
                          Problem("Entity set 'QuanLyKhachSanDbContext.ChucVus'  is null.");
        }

        // GET: ChucVu/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChucVus == null)
            {
                return NotFound();
            }

            var chucVuModel = await _context.ChucVus
                .FirstOrDefaultAsync(m => m.MaCv == id);
            if (chucVuModel == null)
            {
                return NotFound();
            }

            return View(chucVuModel);
        }

        // GET: ChucVu/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChucVu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCv,TenCv,LuongCanBan,MoTa")] ChucVuModel chucVuModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chucVuModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chucVuModel);
        }

        // GET: ChucVu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChucVus == null)
            {
                return NotFound();
            }

            var chucVuModel = await _context.ChucVus.FindAsync(id);
            if (chucVuModel == null)
            {
                return NotFound();
            }
            return View(chucVuModel);
        }

        // POST: ChucVu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaCv,TenCv,LuongCanBan,MoTa")] ChucVuModel chucVuModel)
        {
            if (id != chucVuModel.MaCv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chucVuModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChucVuModelExists(chucVuModel.MaCv))
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
            return View(chucVuModel);
        }

        // GET: ChucVu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChucVus == null)
            {
                return NotFound();
            }

            var chucVuModel = await _context.ChucVus
                .FirstOrDefaultAsync(m => m.MaCv == id);
            if (chucVuModel == null)
            {
                return NotFound();
            }

            return View(chucVuModel);
        }

        // POST: ChucVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChucVus == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.ChucVus'  is null.");
            }
            var chucVuModel = await _context.ChucVus.FindAsync(id);
            if (chucVuModel != null)
            {
                _context.ChucVus.Remove(chucVuModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChucVuModelExists(int id)
        {
          return (_context.ChucVus?.Any(e => e.MaCv == id)).GetValueOrDefault();
        }
    }
}
