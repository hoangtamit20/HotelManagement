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
    public class TacVuPhongController : Controller
    {
        private readonly QuanLyKhachSanDbContext _context;

        public TacVuPhongController(QuanLyKhachSanDbContext context)
        {
            _context = context;
        }

        // GET: TacVuPhong
        public async Task<IActionResult> Index()
        {
              return _context.TacVuPhongs != null ? 
                          View(await _context.TacVuPhongs.ToListAsync()) :
                          Problem("Entity set 'QuanLyKhachSanDbContext.TacVuPhongs'  is null.");
        }

        // GET: TacVuPhong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TacVuPhongs == null)
            {
                return NotFound();
            }

            var tacVuPhongModel = await _context.TacVuPhongs
                .FirstOrDefaultAsync(m => m.MaTvp == id);
            if (tacVuPhongModel == null)
            {
                return NotFound();
            }

            return View(tacVuPhongModel);
        }

        // GET: TacVuPhong/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TacVuPhong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTvp,TenTvp,MoTa")] TacVuPhongModel tacVuPhongModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tacVuPhongModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tacVuPhongModel);
        }

        // GET: TacVuPhong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TacVuPhongs == null)
            {
                return NotFound();
            }

            var tacVuPhongModel = await _context.TacVuPhongs.FindAsync(id);
            if (tacVuPhongModel == null)
            {
                return NotFound();
            }
            return View(tacVuPhongModel);
        }

        // POST: TacVuPhong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTvp,TenTvp,MoTa")] TacVuPhongModel tacVuPhongModel)
        {
            if (id != tacVuPhongModel.MaTvp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tacVuPhongModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TacVuPhongModelExists(tacVuPhongModel.MaTvp))
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
            return View(tacVuPhongModel);
        }

        // GET: TacVuPhong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TacVuPhongs == null)
            {
                return NotFound();
            }

            var tacVuPhongModel = await _context.TacVuPhongs
                .FirstOrDefaultAsync(m => m.MaTvp == id);
            if (tacVuPhongModel == null)
            {
                return NotFound();
            }

            return View(tacVuPhongModel);
        }

        // POST: TacVuPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TacVuPhongs == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.TacVuPhongs'  is null.");
            }
            var tacVuPhongModel = await _context.TacVuPhongs.FindAsync(id);
            if (tacVuPhongModel != null)
            {
                _context.TacVuPhongs.Remove(tacVuPhongModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TacVuPhongModelExists(int id)
        {
          return (_context.TacVuPhongs?.Any(e => e.MaTvp == id)).GetValueOrDefault();
        }
    }
}
