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
    public class NhanVienController : BaseController
    {
        public NhanVienController(QuanLyKhachSanDbContext context) : base(context)
        {
        }
        // GET: NhanVien
        public async Task<IActionResult> Index(int? phanQuyenErrorMessage)
        {
            if (phanQuyenErrorMessage != null)
            {
                ViewData["phanQuyenErrorMessage"] = "Bạn không đủ quyền truy cập!";
            }
            if (base.KiemTraPhanQuyen("QuanLy") || base.KiemTraPhanQuyen("Admin"))
            {                
                var quanLyKhachSanDbContext = _context.NhanViens.Include(n => n.MaCvNavigation).Include(n => n.UserNameNavigation).Include(n => n.NhanVienNghiViecs);
                return View(await quanLyKhachSanDbContext.ToListAsync());
            }
            return base.ChuyenHuong();
        }

        // GET: NhanVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVienModel = await _context.NhanViens
                .Include(n => n.MaCvNavigation)
                .Include(n => n.UserNameNavigation)
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVienModel == null)
            {
                return NotFound();
            }

            return View(nhanVienModel);
        }

        [HttpPost]
        public JsonResult GetDataProfile(string account)
        {
            if (account != null)
            {
                string strResult = account.Split(';')[0];
                var tk = (_context.NhanViens.Where(nv => nv.UserName == strResult).FirstOrDefault());
                return new JsonResult(tk == null ? "/Images/NhanVien/admin.jpg" : tk.HinhAnh == null ? "/Images/NhanVien/noimage.jpg" : "/Images/NhanVien/" + tk.HinhAnh);
            }
            return new JsonResult(-1);
        }

        // GET: NhanVien/Create
        public IActionResult Create()
        {
            ViewData["MaCv"] = new SelectList(_context.ChucVus, "MaCv", "MaCv");
            ViewData["UserName"] = new SelectList(_context.Accounts, "UserName", "UserName");
            return View();
        }

        // POST: NhanVien/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNv,HoTen,Cccd,SoDt,NgaySinh,GioiTinh,DiaChi,Email,HinhAnh,NgayVaoLam,MaCv,UserName")] NhanVienModel nhanVienModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhanVienModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaCv"] = new SelectList(_context.ChucVus, "MaCv", "MaCv", nhanVienModel.MaCv);
            ViewData["UserName"] = new SelectList(_context.Accounts, "UserName", "UserName", nhanVienModel.UserName);
            return View(nhanVienModel);
        }

        // GET: NhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVienModel = await _context.NhanViens.FindAsync(id);
            if (nhanVienModel == null)
            {
                return NotFound();
            }
            ViewData["MaCv"] = new SelectList(_context.ChucVus, "MaCv", "MaCv", nhanVienModel.MaCv);
            ViewData["UserName"] = new SelectList(_context.Accounts, "UserName", "UserName", nhanVienModel.UserName);
            return View(nhanVienModel);
        }

        // POST: NhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaNv,HoTen,Cccd,SoDt,NgaySinh,GioiTinh,DiaChi,Email,HinhAnh,NgayVaoLam,MaCv,UserName")] NhanVienModel nhanVienModel)
        {
            if (id != nhanVienModel.MaNv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhanVienModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhanVienModelExists(nhanVienModel.MaNv))
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
            ViewData["MaCv"] = new SelectList(_context.ChucVus, "MaCv", "MaCv", nhanVienModel.MaCv);
            ViewData["UserName"] = new SelectList(_context.Accounts, "UserName", "UserName", nhanVienModel.UserName);
            return View(nhanVienModel);
        }

        // GET: NhanVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.NhanViens == null)
            {
                return NotFound();
            }

            var nhanVienModel = await _context.NhanViens
                .Include(n => n.MaCvNavigation)
                .Include(n => n.UserNameNavigation)
                .FirstOrDefaultAsync(m => m.MaNv == id);
            if (nhanVienModel == null)
            {
                return NotFound();
            }

            return View(nhanVienModel);
        }

        // POST: NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.NhanViens == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.NhanViens'  is null.");
            }
            var nhanVienModel = await _context.NhanViens.FindAsync(id);
            if (nhanVienModel != null)
            {
                _context.NhanViens.Remove(nhanVienModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhanVienModelExists(int id)
        {
            return (_context.NhanViens?.Any(e => e.MaNv == id)).GetValueOrDefault();
        }
    }
}
