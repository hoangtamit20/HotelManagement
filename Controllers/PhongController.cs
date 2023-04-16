using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLKSMVC.Base;
using QLKSMVC.Data;
using QLKSMVC.Models;

namespace QLKSMVC.Controllers
{
    public class PhongController : BaseController
    {
        private readonly IWebHostEnvironment _env;

        public PhongController(QuanLyKhachSanDbContext context, IWebHostEnvironment env) : base(context)
        {
            _env = env;
        }


        // GET: Phong
        public async Task<IActionResult> Index()
        {
            if (base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("QuanLy"))
            {
                var quanLyKhachSanDbContext = _context.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTvpNavigation);
                return View(await quanLyKhachSanDbContext.ToListAsync());
            }
            return base.ChuyenHuong();
        }

        // GET: Phong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("QuanLy"))
            {
            if (id == null || _context.Phongs == null)
            {
                return NotFound();
            }

            var phongModel = await _context.Phongs
                .Include(p => p.MaLpNavigation)
                .Include(p => p.MaTvpNavigation)
                .FirstOrDefaultAsync(m => m.MaP == id);
            if (phongModel == null)
            {
                return NotFound();
            }

            return View(phongModel);
            }
            return base.ChuyenHuong();
        }

        // GET: Phong/Create
        public IActionResult Create()
        {
            if (base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("QuanLy"))
            {
            ViewData["MaLp"] = new SelectList(_context.LoaiPhongs, "MaLp", "TenLp");
            ViewData["MaTvp"] = new SelectList(_context.TacVuPhongs, "MaTvp", "TenTvp");
            return View();
            }
            return base.ChuyenHuong();
        }

        // POST: Phong/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhongModel phongModel)
        {
            if (ModelState.IsValid)
            {
                if (phongModel.fileUpload != null)
                {
                    var filePath = Path.Combine(
                        _env.WebRootPath,
                        "Images/Phong",
                        phongModel.fileUpload.FileName
                    );
                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    //lưu dữ liệu fileUpload và stream
                    phongModel.fileUpload.CopyTo(fileStream);
                    phongModel.HinhAnh = phongModel.fileUpload.FileName;
                    // System.Console.WriteLine(phongModel.HinhAnh + " - " + phongModel.TenNv);
                }
                try
                {
                    _context.Add(phongModel);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Số phòng {phongModel.SoPhong} đã tồn tại! {ex.Message}");
                    return View(phongModel);
                }
            }
            ViewData["MaLp"] = new SelectList(_context.LoaiPhongs, "MaLp", "TenLp", phongModel.MaLp);
            ViewData["MaTvp"] = new SelectList(_context.TacVuPhongs, "MaTvp", "TenTvp", phongModel.MaTvp);

            return View(phongModel);
        }

        // GET: Phong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("QuanLy"))
            {
            if (id == null || _context.Phongs == null)
            {
                return NotFound();
            }

            var phongModel = await _context.Phongs.FindAsync(id);
            if (phongModel == null)
            {
                return NotFound();
            }
            ViewData["MaLp"] = new SelectList(_context.LoaiPhongs, "MaLp", "TenLp", phongModel.MaLp);
            ViewData["MaTvp"] = new SelectList(_context.TacVuPhongs, "MaTvp", "TenTvp", phongModel.MaTvp);
            return View(phongModel);
            }
            return base.ChuyenHuong();
        }

        // POST: Phong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaP,SoPhong,HinhAnh,MoTa,MaLp,MaTvp")] PhongModel phongModel)
        {
            if (id != phongModel.MaP)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phongModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongModelExists(phongModel.MaP))
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
            ViewData["MaLp"] = new SelectList(_context.LoaiPhongs, "MaLp", "TenLp", phongModel.MaLp);
            ViewData["MaTvp"] = new SelectList(_context.TacVuPhongs, "MaTvp", "TenTvp", phongModel.MaTvp);
            return View(phongModel);
        }

        // GET: Phong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Phongs == null)
            {
                return NotFound();
            }

            var phongModel = await _context.Phongs
                .Include(p => p.MaLpNavigation)
                .Include(p => p.MaTvpNavigation)
                .FirstOrDefaultAsync(m => m.MaP == id);
            if (phongModel == null)
            {
                return NotFound();
            }

            return View(phongModel);
        }

        // POST: Phong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Phongs == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.Phongs'  is null.");
            }
            var phongModel = await _context.Phongs.FindAsync(id);
            if (phongModel != null)
            {
                _context.Phongs.Remove(phongModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhongModelExists(int id)
        {
            return (_context.Phongs?.Any(e => e.MaP == id)).GetValueOrDefault();
        }
    }
}
