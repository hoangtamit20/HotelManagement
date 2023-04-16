using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLKSMVC.Data;
using QLKSMVC.Models;

namespace QLKSMVC.Controllers
{
    public class DatDichVuController : Controller
    {
        private readonly QuanLyKhachSanDbContext _context;

        public DatDichVuController(QuanLyKhachSanDbContext context)
        {
            _context = context;
        }

        // GET: DatDichVu
        public async Task<IActionResult> Index()
        {
            if (TempData["MaDpDDV"] != null)
            {
                var MaDp = (int)TempData["MaDpDDV"];
                ViewData["MaKh"] = (await _context.DatPhongs.Where(dp => dp.MaDp == MaDp).FirstOrDefaultAsync()).MaKh;
                ViewData["MaDp"] = MaDp;
                TempData["MaDp"] = MaDp;
                var listDV = await _context.DichVus.Include(dv => dv.MaLdvNavigation).ToListAsync();
                return View(listDV);
            }
            ViewData["ThongBaoLoi"] = "Dữ liệu không hợp lệ! Vui lòng thử lại!";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string dataListDatDichVu)
        {
            List<string> listError = new List<string>();
            if (TempData["MaDpDDV"] != null)
            {
                var MaDp = (int)TempData["MaDpDDV"];
                ViewData["MaKh"] = (await _context.DatPhongs.Where(dp => dp.MaDp == MaDp).FirstOrDefaultAsync()).MaKh;
                ViewData["MaDp"] = MaDp;
                TempData["MaDpDDV"] = MaDp;
                if (dataListDatDichVu != null)
                {
                    var danhSachDatPhong = dataListDatDichVu.Split(";");
                    var listDV = await _context.DichVus.Include(dv => dv.MaLdvNavigation).ToListAsync();
                    var listDatDichVu = new List<DatDichVuModel>();
                    for (int i = 0; i < danhSachDatPhong.Length; i++)
                    {
                        var kq = danhSachDatPhong[i].Split(",");
                        var datDV = new DatDichVuModel();
                        datDV.SoLuong = Int32.Parse(kq[0]);
                        datDV.NgayDatDichVu = DateTime.Parse(kq[1]);
                        datDV.MaDv = Int32.Parse(kq[2]);
                        datDV.MaKh = Int32.Parse(kq[3]);
                        datDV.MaDp = Int32.Parse(kq[4]);
                        datDV.TongTien = decimal.Parse(kq[5]);
                        listDatDichVu.Add(datDV);
                    }

                    var danhSachDatDichVu = await _context.DatDichVus.Include(ddv => ddv.MaDvNavigation).Where(ddv => ddv.MaDp == MaDp).ToListAsync();
                    foreach (var item in listDatDichVu)
                    {
                        if (item.SoLuong.Value > (await _context.DichVus.Where(dv => dv.MaDv == item.MaDv).FirstOrDefaultAsync()).SoLuong.Value)
                        {
                            listError.Add($"Số lượng dịch vụ {(await _context.DichVus.Where(dv => dv.MaDv == item.MaDv).FirstOrDefaultAsync()).TenDv} đang đặt vượt quá số lượng thực tế.");
                        }
                        else
                        {
                            if (listError.Count < 1)
                            {
                                if (danhSachDatDichVu.Where(ds => ds.MaDv == item.MaDv).FirstOrDefault() != null)
                                {
                                    var ddVu = danhSachDatDichVu.Where(ds => ds.MaDv == item.MaDv).FirstOrDefault();
                                    if (ddVu != null)
                                    {
                                        ddVu.SoLuong += item.SoLuong.Value;
                                        ddVu.TongTien = ddVu.SoLuong.Value * (_context.DichVus.Where(dv => dv.MaDv == item.MaDv).FirstOrDefault()).DonGia.Value;
                                        _context.Update(ddVu);
                                        try
                                        {
                                            await _context.SaveChangesAsync();
                                        }
                                        catch (SqlException ex)
                                        {
                                            listError.Add($"Lỗi! Không thể cập nhật dịch vụ. {ex.Message}");
                                            ViewData["ListError"] = listError;
                                            return View(_context.DichVus.Include(dv => dv.MaLdvNavigation).ToList());
                                        }
                                        var dichVu = _context.DichVus.Where(dv => dv.MaDv == item.MaDv).FirstOrDefault();
                                        if (dichVu != null)
                                        {
                                            dichVu.SoLuong -= item.SoLuong.Value;
                                            _context.Update(dichVu);
                                            try
                                            {
                                                _context.SaveChanges();
                                            }
                                            catch (SqlException ex)
                                            {
                                                listError.Add($"Lỗi! Không thể cập nhật dịch vụ. {ex.Message}");
                                                ViewData["ListError"] = listError;
                                                return View(await _context.DichVus.Include(dv => dv.MaLdvNavigation).ToListAsync());
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    _context.Add(item);
                                    try
                                    {
                                        await _context.SaveChangesAsync();
                                    }
                                    catch (SqlException ex)
                                    {
                                        listError.Add($"Lỗi! Không thể thêm dịch vụ. {ex.Message}");
                                        ViewData["ListError"] = listError;
                                        return View(await _context.DichVus.Include(dv => dv.MaLdvNavigation).ToListAsync());
                                    }
                                    var dichVu = _context.DichVus.Where(dv => dv.MaDv == item.MaDv).FirstOrDefault();
                                    if (dichVu != null)
                                    {
                                        dichVu.SoLuong -= item.SoLuong.Value;
                                        _context.Update(dichVu);
                                        try
                                        {
                                            _context.SaveChanges();
                                        }
                                        catch (SqlException ex)
                                        {
                                            listError.Add($"Lỗi! Không thể cập nhật dịch vụ. {ex.Message}");
                                            ViewData["ListError"] = listError;
                                            return View(await _context.DichVus.Include(dv => dv.MaLdvNavigation).ToListAsync());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ViewData["ListError"] = listError;
                                return View(await _context.DichVus.Include(dv => dv.MaLdvNavigation).ToListAsync());
                            }
                        }
                    }
                    if (listError.Count < 1)
                    {
                        ViewData["ThongBaoThanhCong"] = "Đặt dịch vụ thành công!";
                        return View(_context.DichVus.Include(dv => dv.MaLdvNavigation).ToList());
                    }
                }
                listError.Add("Vui lòng chọn dịch vụ muốn đặt!");
                ViewData["ListError"] = listError;
                return View(await _context.DichVus.Include(dv => dv.MaLdvNavigation).ToListAsync());
            }
            ViewData["ThongBaoLoi"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!";
            return View(await _context.DichVus.Include(dv => dv.MaLdvNavigation).ToListAsync());
        }

        // GET: DatDichVu/Details/5

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DatDichVus == null)
            {
                return NotFound();
            }

            var datDichVuModel = await _context.DatDichVus
                .Include(d => d.MaDpNavigation)
                .Include(d => d.MaDvNavigation)
                .Include(d => d.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaDdv == id);
            if (datDichVuModel == null)
            {
                return NotFound();
            }

            return View(datDichVuModel);
        }

        // GET: DatDichVu/Create
        public IActionResult Create()
        {
            ViewData["MaDp"] = new SelectList(_context.DatPhongs, "MaDp", "MaDp");
            ViewData["MaDv"] = new SelectList(_context.DichVus, "MaDv", "MaDv");
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "Cccd");
            return View();
        }

        // POST: DatDichVu/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDdv,SoLuong,NgayDatDichVu,MaDv,MaKh,MaDp,TongTien")] DatDichVuModel datDichVuModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(datDichVuModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaDp"] = new SelectList(_context.DatPhongs, "MaDp", "MaDp", datDichVuModel.MaDp);
            ViewData["MaDv"] = new SelectList(_context.DichVus, "MaDv", "MaDv", datDichVuModel.MaDv);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "Cccd", datDichVuModel.MaKh);
            return View(datDichVuModel);
        }

        // GET: DatDichVu/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DatDichVus == null)
            {
                return NotFound();
            }

            var datDichVuModel = await _context.DatDichVus.FindAsync(id);
            if (datDichVuModel == null)
            {
                return NotFound();
            }
            ViewData["MaDp"] = new SelectList(_context.DatPhongs, "MaDp", "MaDp", datDichVuModel.MaDp);
            ViewData["MaDv"] = new SelectList(_context.DichVus, "MaDv", "MaDv", datDichVuModel.MaDv);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "Cccd", datDichVuModel.MaKh);
            return View(datDichVuModel);
        }

        // POST: DatDichVu/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDdv,SoLuong,NgayDatDichVu,MaDv,MaKh,MaDp,TongTien")] DatDichVuModel datDichVuModel)
        {
            if (id != datDichVuModel.MaDdv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(datDichVuModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatDichVuModelExists(datDichVuModel.MaDdv))
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
            ViewData["MaDp"] = new SelectList(_context.DatPhongs, "MaDp", "MaDp", datDichVuModel.MaDp);
            ViewData["MaDv"] = new SelectList(_context.DichVus, "MaDv", "MaDv", datDichVuModel.MaDv);
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "Cccd", datDichVuModel.MaKh);
            return View(datDichVuModel);
        }

        // GET: DatDichVu/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DatDichVus == null)
            {
                return NotFound();
            }

            var datDichVuModel = await _context.DatDichVus
                .Include(d => d.MaDpNavigation)
                .Include(d => d.MaDvNavigation)
                .Include(d => d.MaKhNavigation)
                .FirstOrDefaultAsync(m => m.MaDdv == id);
            if (datDichVuModel == null)
            {
                return NotFound();
            }

            return View(datDichVuModel);
        }

        // POST: DatDichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DatDichVus == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.DatDichVus'  is null.");
            }
            var datDichVuModel = await _context.DatDichVus.FindAsync(id);
            if (datDichVuModel != null)
            {
                _context.DatDichVus.Remove(datDichVuModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatDichVuModelExists(int id)
        {
            return (_context.DatDichVus?.Any(e => e.MaDdv == id)).GetValueOrDefault();
        }

        public IActionResult GetDataDatDichVu(int? MaDp)
        {
            if (MaDp != null)
            {
                TempData["MaDpDDV"] = MaDp.Value;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
    }
}
