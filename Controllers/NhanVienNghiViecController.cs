using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QLKSMVC.Data;
using QLKSMVC.Models;

namespace QLKSMVC.Controllers
{
    public class NhanVienNghiViecController : Controller
    {
        private readonly QuanLyKhachSanDbContext _context;

        public NhanVienNghiViecController(QuanLyKhachSanDbContext context)
        {
            _context = context;
        }

        // GET: NhanVienNghiViec
        public async Task<IActionResult> Index()
        {
            
            var quanLyKhachSanDbContext = _context.NhanVienNghiViecs.Include(n => n.MaNvNavigation);
            return View(await quanLyKhachSanDbContext.ToListAsync());
        }

        // GET: NhanVienNghiViec/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NhanVienNghiViecs == null)
            {
                return NotFound();
            }

            var nhanVienNghiViecModel = await _context.NhanVienNghiViecs
                .Include(n => n.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaNvnv == id);
            if (nhanVienNghiViecModel == null)
            {
                return NotFound();
            }

            return View(nhanVienNghiViecModel);
        }

        // GET: NhanVienNghiViec/Create
        public IActionResult Create()
        {
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "Cccd");
            return View();
        }

        // POST: NhanVienNghiViec/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNvnv,NgayNghiViec,LyDo,MaNv")] NhanVienNghiViecModel nhanVienNghiViecModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVienNghiViecModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "Cccd", nhanVienNghiViecModel.MaNv);
            return View(nhanVienNghiViecModel);
        }

        // GET: NhanVienNghiViec/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.NhanVienNghiViecs == null)
            {
                return NotFound();
            }

            var nhanVienNghiViecModel = await _context.NhanVienNghiViecs.FindAsync(id);
            if (nhanVienNghiViecModel == null)
            {
                return NotFound();
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "Cccd", nhanVienNghiViecModel.MaNv);
            return View(nhanVienNghiViecModel);
        }

        // POST: NhanVienNghiViec/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNvnv,NgayNghiViec,LyDo,MaNv")] NhanVienNghiViecModel nhanVienNghiViecModel)
        {
            if (id != nhanVienNghiViecModel.MaNvnv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVienNghiViecModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienNghiViecModelExists(nhanVienNghiViecModel.MaNvnv))
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
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "Cccd", nhanVienNghiViecModel.MaNv);
            return View(nhanVienNghiViecModel);
        }

        // GET: NhanVienNghiViec/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.NhanVienNghiViecs == null)
            {
                return NotFound();
            }

            var nhanVienNghiViecModel = await _context.NhanVienNghiViecs
                .Include(n => n.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaNvnv == id);
            if (nhanVienNghiViecModel == null)
            {
                return NotFound();
            }

            return View(nhanVienNghiViecModel);
        }

        // POST: NhanVienNghiViec/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.NhanVienNghiViecs == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.NhanVienNghiViecs'  is null.");
            }
            var nhanVienNghiViecModel = await _context.NhanVienNghiViecs.FindAsync(id);
            if (nhanVienNghiViecModel != null)
            {
                _context.NhanVienNghiViecs.Remove(nhanVienNghiViecModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienNghiViecModelExists(int id)
        {
          return (_context.NhanVienNghiViecs?.Any(e => e.MaNvnv == id)).GetValueOrDefault();
        }

        [HttpPost]
        public JsonResult ConfirmThoiViecNV(int? MaNv, string LyDo)
        {
            if (MaNv != null && LyDo != null)
            {
                NhanVienNghiViecModel nv = new NhanVienNghiViecModel(){MaNv = MaNv.Value, LyDo = LyDo, NgayNghiViec = DateTime.Now};
                _context.Add(nv);
                try{
                    _context.SaveChanges();
                    var taiKhoan = _context.Accounts.Where(tk => tk.UserName == ((_context.NhanViens.Where(nv => nv.MaNv == MaNv.Value)).FirstOrDefault()).UserName).FirstOrDefault();
                    if (taiKhoan != null)
                    {
                        taiKhoan.RoleId = -1;
                        _context.Update(taiKhoan);
                        try{
                            _context.SaveChanges();
                            return Json(true);
                        }catch{
                            return Json(false);
                        }
                    }
                    return Json(false);
                }catch
                {
                    System.Console.WriteLine("thất bại!");

                    return Json(false);
                }
            }
            System.Console.WriteLine("thất bại!");
            return Json(false);
        }
    }
}
