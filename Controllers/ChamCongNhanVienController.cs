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
    public class ChamCongNhanVienController : BaseController
    {
        public ChamCongNhanVienController(QuanLyKhachSanDbContext context) : base(context)
        {
        }


        // GET: ChamCongNhanVien
        public async Task<IActionResult> Index()
        {
            if (base.KiemTraPhanQuyen("QuanLy"))
            {
            ViewData["NgayChamCong"] = DateTime.Now;
            ViewData["ListNhanVien"] = _context.NhanViens.Include(nv => nv.NhanVienNghiViecs).Include(nv => nv.ChamCongNhanViens.Where(cc => cc.NgayGioDiLam.Value.Year == DateTime.Now.Year
            && cc.NgayGioDiLam.Value.Month == DateTime.Now.Month && cc.NgayGioDiLam.Value.Day == DateTime.Now.Day)).Where(nv => nv.NhanVienNghiViecs.Where(nvnv => nvnv.MaNv == nv.MaNv).FirstOrDefault() == null).ToList();
            return View();
            }
            return base.ChuyenHuong();
        }

        [HttpPost]
        public JsonResult CreateChamCong(int? maNV, DateTime? ngayGioDiLam)
        {
            if (maNV == null || ngayGioDiLam == null)
            {
                return new JsonResult(-1);
            }
            else
            {
                try{
                    ChamCongNhanVienModel cc =  new ChamCongNhanVienModel(){MaNv = maNV.Value, NgayGioDiLam = ngayGioDiLam.Value};
                    if (_context.ChamCongNhanViens.Where(c => c.MaNv == maNV.Value && c.NgayGioDiLam.Value.Year == ngayGioDiLam.Value.Year && c.NgayGioDiLam.Value.Month == ngayGioDiLam.Value.Month && c.NgayGioDiLam.Value.Day == ngayGioDiLam.Value.Day).FirstOrDefault() != null)
                    {
                        return new JsonResult(-1);
                    }
                    if(ngayGioDiLam.Value > new DateTime(ngayGioDiLam.Value.Year, ngayGioDiLam.Value.Month, ngayGioDiLam.Value.Day, 6,55,00))
                    {
                        cc.DiLamTre = true;
                        _context.Add(cc);
                        _context.SaveChanges();
                        return new JsonResult(1);
                    }
                    _context.Add(cc);
                    _context.SaveChanges();
                    return new JsonResult(0);
                }catch{
                    return new JsonResult(-1);
                }

            }
        }

        [HttpPost]
        public JsonResult UpdateChamCong(int? maNV, DateTime? ngayGioRaVe)
        {
            if (maNV == null || ngayGioRaVe == null)
            {
                return new JsonResult(-1);
            }
            else
            {
                // try{
                    var cc = _context.ChamCongNhanViens.Where(c => c.MaNv == maNV.Value && c.NgayGioDiLam.Value.Year == ngayGioRaVe.Value.Year && c.NgayGioDiLam.Value.Month == ngayGioRaVe.Value.Month && c.NgayGioDiLam.Value.Day == ngayGioRaVe.Value.Day).FirstOrDefault();
                    if (cc == null)
                    {
                        return new JsonResult(-1);
                    }
                    if(ngayGioRaVe.Value < new DateTime(ngayGioRaVe.Value.Year, ngayGioRaVe.Value.Month, ngayGioRaVe.Value.Day, 17,00,00))
                    {
                        cc.MaNv = maNV.Value;
                        cc.NgayGioRaVe = ngayGioRaVe.Value;
                        cc.RaVeSom = true;
                        _context.Update(cc);
                        _context.SaveChanges();
                        return new JsonResult(1);
                    }
                    cc.MaNv = maNV.Value;
                    cc.NgayGioRaVe = ngayGioRaVe.Value;
                    _context.Update(cc);
                    _context.SaveChanges();
                    return new JsonResult(0);
                // }catch{
                //     return new JsonResult(-1);
                // }

            }
        }


        [HttpPost]
        public JsonResult XinPhepNghi(int? MaNv, string lyDo)
        {
            if (MaNv != null && lyDo != null)
            {
                var chamCong = _context.ChamCongNhanViens.Where(cc => cc.MaNv == MaNv.Value && cc.NgayGioDiLam.Value.Year == DateTime.Now.Year
                        && cc.NgayGioDiLam.Value.Month == DateTime.Now.Month && cc.NgayGioDiLam.Value.Day == DateTime.Now.Day).FirstOrDefault();
                if(chamCong == null)
                {
                    ChamCongNhanVienModel cc = new ChamCongNhanVienModel(){MaNv = MaNv.Value, NgayGioDiLam = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0,0,0), LyDoNghi = lyDo, XinNghiPhep = true};
                    try{
                        _context.Add(cc);
                        _context.SaveChanges();
                        return new JsonResult(1);
                    }catch{
                        return new JsonResult(-1);
                    }
                }
                else
                {
                    chamCong.XinNghiPhep = true;
                    chamCong.LyDoNghi = lyDo;
                    try{
                        _context.Update(chamCong);
                        _context.SaveChanges();
                        return new JsonResult(0);
                    }catch{
                        return new JsonResult(-1);
                    }
                }

            }
            return new JsonResult(-1);
        }

        // GET: ChamCongNhanVien/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ChamCongNhanViens == null)
            {
                return NotFound();
            }

            var chamCongNhanVienModel = await _context.ChamCongNhanViens
                .Include(c => c.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaCcnv == id);
            if (chamCongNhanVienModel == null)
            {
                return NotFound();
            }

            return View(chamCongNhanVienModel);
        }

        
        // GET: ChamCongNhanVien/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ChamCongNhanViens == null)
            {
                return NotFound();
            }

            var chamCongNhanVienModel = await _context.ChamCongNhanViens.FindAsync(id);
            if (chamCongNhanVienModel == null)
            {
                return NotFound();
            }
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "Cccd", chamCongNhanVienModel.MaNv);
            return View(chamCongNhanVienModel);
        }

        // POST: ChamCongNhanVien/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaCcnv,NgayGioDiLam,NgayGioRaVe,DiLamTre,RaVeSom,XinNghiPhep,LyDoNghi,MaNv")] ChamCongNhanVienModel chamCongNhanVienModel)
        {
            if (id != chamCongNhanVienModel.MaCcnv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chamCongNhanVienModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChamCongNhanVienModelExists(chamCongNhanVienModel.MaCcnv))
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
            ViewData["MaNv"] = new SelectList(_context.NhanViens, "MaNv", "Cccd", chamCongNhanVienModel.MaNv);
            return View(chamCongNhanVienModel);
        }

        // GET: ChamCongNhanVien/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ChamCongNhanViens == null)
            {
                return NotFound();
            }

            var chamCongNhanVienModel = await _context.ChamCongNhanViens
                .Include(c => c.MaNvNavigation)
                .FirstOrDefaultAsync(m => m.MaCcnv == id);
            if (chamCongNhanVienModel == null)
            {
                return NotFound();
            }

            return View(chamCongNhanVienModel);
        }

        // POST: ChamCongNhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ChamCongNhanViens == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.ChamCongNhanViens'  is null.");
            }
            var chamCongNhanVienModel = await _context.ChamCongNhanViens.FindAsync(id);
            if (chamCongNhanVienModel != null)
            {
                _context.ChamCongNhanViens.Remove(chamCongNhanVienModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChamCongNhanVienModelExists(int id)
        {
          return (_context.ChamCongNhanViens?.Any(e => e.MaCcnv == id)).GetValueOrDefault();
        }
    }
}
