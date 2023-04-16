using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLKSMVC.Base;
using QLKSMVC.Data;
using QLKSMVC.Models;
using SelectPdf;

namespace QLKSMVC.Controllers
{
    public class DatPhongController : BaseController
    {
        public DatPhongController(QuanLyKhachSanDbContext context) : base(context)
        {
        }

        // GET: DatPhong
        public async Task<IActionResult> Index()
        {
            var quanLyKhachSanDbContext = _context.DatPhongs.Include(d => d.MaKhNavigation).Include(d => d.MaPNavigation).Include(dp => dp.MaPNavigation.MaLpNavigation).OrderByDescending(dp => dp.NgayBatDau.Value);
            return View(await quanLyKhachSanDbContext.ToListAsync());
        }

        // GET: DatPhong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DatPhongs == null)
            {
                return NotFound();
            }

            var datPhongModel = await _context.DatPhongs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaPNavigation)
                .FirstOrDefaultAsync(m => m.MaDp == id);
            if (datPhongModel == null)
            {
                return NotFound();
            }

            return View(datPhongModel);
        }


        [HttpPost]
        public JsonResult ConfirmDeleteDatPhong(int? MaDp)
        {
            System.Console.WriteLine(MaDp.Value);
            if (MaDp != null)
            {
                var datPhong = _context.DatPhongs.Find(MaDp.Value);
                if (datPhong != null)
                {
                    try{
                        _context.DatPhongs.Remove(datPhong);
                        _context.SaveChanges();
                        return new JsonResult(true);
                    }catch{
                        return new JsonResult(false);
                    }
                }
                return new JsonResult(false);
            }
            return new JsonResult(false);
        }

        public IActionResult ThayDoiDatPhong()
        {
            return View(_context.DatPhongs.Include(dp => dp.MaKhNavigation).Include(dp => dp.MaPNavigation).Where(dp => dp.NgayBatDau.Value > DateTime.Now).ToList());
        }

        // GET: DatPhong/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DatPhongs == null)
            {
                return NotFound();
            }

            var datPhongModel = await _context.DatPhongs.FindAsync(id);
            if (datPhongModel == null)
            {
                return NotFound();
            }
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "HoTen", datPhongModel.MaKh);
            ViewData["MaP"] = new SelectList(_context.Phongs, "MaP", "SoPhong", datPhongModel.MaP);
            return View(datPhongModel);
        }

        // POST: DatPhong/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DatPhongModel datPhongModel)
        {
            if (id != datPhongModel.MaDp)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (_context.HoaDons.Where(hd => hd.MaDp == datPhongModel.MaDp).FirstOrDefault() != null)
                {
                    ModelState.AddModelError("","Bạn không thể thay đổi thông tin đặt phòng đã thanh toán!");
                    ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "MaKh", datPhongModel.MaKh);
                    ViewData["MaP"] = new SelectList(_context.Phongs, "MaP", "SoPhong", datPhongModel.MaP);
                    return View(datPhongModel);
                }
                if (datPhongModel.NgayBatDau.Value > datPhongModel.NgayKetThuc.Value)
                {
                    ModelState.AddModelError("", $"Ngày đặt phòng phải nhỏ hơn ngày kết thúc");
                    ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "HoTen", datPhongModel.MaKh);
                    ViewData["MaP"] = new SelectList(_context.Phongs, "MaP", "SoPhong", datPhongModel.MaP);
                    return View(datPhongModel);
                }
                var listDatPhong = _context.DatPhongs.Where(
                    dp => 
                        (
                            dp.MaDp != datPhongModel.MaDp
                            &&
                            dp.MaP == datPhongModel.MaP
                            &&
                            ((datPhongModel.NgayBatDau.Value >= dp.NgayBatDau.Value && datPhongModel.NgayBatDau.Value <= dp.NgayKetThuc.Value) 
                            || 
                            (datPhongModel.NgayKetThuc.Value >= dp.NgayBatDau.Value && datPhongModel.NgayKetThuc.Value <= dp.NgayKetThuc.Value))
                        )
                ).FirstOrDefault();
                if (listDatPhong != null)
                {
                    ModelState.AddModelError("", $"Phòng này đã được đặt trong thời gian này vui lòng chọn thời gian khác phù hợp!");
                    ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "HoTen", datPhongModel.MaKh);
                    ViewData["MaP"] = new SelectList(_context.Phongs, "MaP", "MaP", datPhongModel.MaP);
                    return View(datPhongModel);
                }
                try
                {
                    _context.Update(datPhongModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatPhongModelExists(datPhongModel.MaDp))
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
            ViewData["MaKh"] = new SelectList(_context.KhachHangs, "MaKh", "HoTen", datPhongModel.MaKh);
            ViewData["MaP"] = new SelectList(_context.Phongs, "MaP", "SoPhong", datPhongModel.MaP);
            return View(datPhongModel);
        }

        // GET: DatPhong/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DatPhongs == null)
            {
                return NotFound();
            }

            var datPhongModel = await _context.DatPhongs
                .Include(d => d.MaKhNavigation)
                .Include(d => d.MaPNavigation)
                .FirstOrDefaultAsync(m => m.MaDp == id);
            if (datPhongModel == null)
            {
                return NotFound();
            }

            return View(datPhongModel);
        }

        // POST: DatPhong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.DatPhongs == null)
            {
                return Problem("Entity set 'QuanLyKhachSanDbContext.DatPhongs'  is null.");
            }
            var datPhongModel = await _context.DatPhongs.FindAsync(id);
            if (datPhongModel != null)
            {
                _context.DatPhongs.Remove(datPhongModel);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatPhongModelExists(int id)
        {
          return (_context.DatPhongs?.Any(e => e.MaDp == id)).GetValueOrDefault();
        }


        public IActionResult DanhSachDatPhong(int? phanQuyenErrorMessage)
        {
            if(phanQuyenErrorMessage != null)
            {
                ViewData["phanQuyenErrorMessage"] = "Bạn không đủ quyền truy cập!";
            }
            if (_context.LoaiPhongs != null && _context.KhachHangs != null)
            {
                var listLoaiPhong = _context.LoaiPhongs.ToList();
                listLoaiPhong.Insert(0, new LoaiPhongModel(){MaLp = -1, TenLp = "Chọn loại phòng"});
                ViewData["MaLp"] = new SelectList(listLoaiPhong, "MaLp", "TenLp");
                TempData["NgayDatDK"] = DateTime.Now;
                TempData["NgayTraDK"] = DateTime.Now.AddDays(1);
            }
            return base.DanhSachDatPhongTrong(DateTime.Now, DateTime.Now.AddDays(1)) != null ? View(base.DanhSachDatPhongTrong(DateTime.Now, DateTime.Now.AddDays(1))) : View();
        }

        [HttpPost]
        public async Task<IActionResult> DanhSachDatPhong(DateTime ngayDatPhong, DateTime ngayTraPhong, int? MaLp)
        {
            if (ngayDatPhong == DateTime.MinValue || ngayTraPhong == DateTime.MinValue)
            {
                if (ngayDatPhong == DateTime.MinValue)
                {
                    ViewData["NgayDatPhongError"] = "Ngày đặt phòng không được để trống!";
                }
                if (ngayTraPhong == DateTime.MinValue)
                {
                    ViewData["NgayTraPhongError"] = "Ngày trả phòng không được để trống!";
                }
                ViewBag.NgayDatPhongData = ngayDatPhong;
                ViewBag.NgayKetThucData = ngayTraPhong;
                if (_context.LoaiPhongs != null)
                {
                    var listLoaiPhong = _context.LoaiPhongs.ToList();
                    listLoaiPhong.Insert(0, new LoaiPhongModel(){MaLp = -1, TenLp = "Chọn loại phòng"});
                    ViewData["MaLp"] = new SelectList(listLoaiPhong, "MaLp", "TenLp", MaLp.Value);
                }
                return View();
            }
            else
            {
                if (ngayTraPhong < ngayDatPhong)
                {
                    ViewBag.NgayDatPhongData = ngayDatPhong;
                    ViewBag.NgayKetThucData = ngayTraPhong;
                    if (_context.LoaiPhongs != null)
                    {
                        var listLoaiPhong = _context.LoaiPhongs.ToList();
                        listLoaiPhong.Insert(0, new LoaiPhongModel(){MaLp = -1, TenLp = "Chọn loại phòng"});
                        ViewData["MaLp"] = new SelectList(listLoaiPhong, "MaLp", "TenLp", MaLp.Value);
                    }
                    ViewBag.ShowErrorMessage = "Ngày trả phòng phải lớn hơn ngày đặt phòng!";
                    return View();
                }
                else
                {
                    ViewBag.NgayDatPhongData = ngayDatPhong;
                    ViewBag.NgayKetThucData = ngayTraPhong;
                    if (_context.LoaiPhongs != null)
                    {
                        var listLoaiPhong = _context.LoaiPhongs.ToList();
                        listLoaiPhong.Insert(0, new LoaiPhongModel(){MaLp = -1, TenLp = "Chọn loại phòng"});
                        ViewData["MaLp"] = new SelectList(listLoaiPhong, "MaLp", "TenLp", MaLp.Value);
                    }
                    TempData["NgayDatDK"] = ngayDatPhong;
                    TempData["NgayTraDK"] = ngayTraPhong;
                    if(MaLp != -1 && MaLp != null)
                    {   
                        return View(base.DanhSachDatPhongTrong(ngayDatPhong, ngayTraPhong).Where(p => p.MaLp == MaLp.Value).ToList());
                    }
                    return View(base.DanhSachDatPhongTrong(ngayDatPhong, ngayTraPhong));
                }
            }
        }

        public IActionResult GetDataPhong(int? MaP)
        {
            TempData["MaP"] = MaP.Value;
            return RedirectToAction(nameof(DangKyDatPhong));
        }

        public IActionResult DangKyDatPhong()
        {
            if (_context.Phongs != null && TempData["MaP"] != null && _context.KhachHangs != null)
            {
                var MaP = (int)TempData["MaP"];
                ViewData["Phong"] = _context.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTvpNavigation)
                                        .Where(p => p.MaP == MaP).FirstOrDefault();
                ViewData["MaKH"] = TempData["MaKH"] == null ? (_context.KhachHangs.OrderByDescending(k => k.MaKh).FirstOrDefault().MaKh + 1) : TempData["MaKH"];
                ViewData["KhachHang"] = null;
                var ngayDat = TempData["NgayDatDK"];
                var ngayTra = TempData["NgayTraDK"];
                ViewData["NgayDatPDK"] = ngayDat;
                ViewData["NgayTraPDK"] = ngayTra;
                return View();
            }
            ViewData["ErrorThongBao"] = "Dữ liệu không hợp lệ";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DangKyDatPhong(DatPhongModel datPhongModel, KhachHangModel khachHangModel)
        {
            TempData["MaP"] = datPhongModel.MaP;
            ViewData["Phong"] = _context.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTvpNavigation)
                                        .Where(p => p.MaP == datPhongModel.MaP).FirstOrDefault();
            ViewData["KHModel"] = khachHangModel;
            if (datPhongModel != null)
            {
                ViewData["NgayDatPDK"] = datPhongModel.NgayBatDau != null ? datPhongModel.NgayBatDau.Value : null;
                ViewData["NgayTraPDK"] = datPhongModel.NgayKetThuc != null ? datPhongModel.NgayKetThuc.Value : null;
            }
            if(!validateKhachHangModel(khachHangModel))
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                var khachHang = _context.KhachHangs.Where(kh => kh.Cccd.Equals(khachHangModel.Cccd)).FirstOrDefault();
                if (khachHang != null && khachHangModel != null)
                {
                    khachHangModel.MaKh = khachHang.MaKh;
                }
                else
                {
                    _context.Add(khachHangModel);
                    try{
                        await _context.SaveChangesAsync();
                    }catch(SqlException ex)
                    {
                        ModelState.AddModelError("", $"Cập nhật khách hàng bị lỗi {ex.Message}");
                    }
                }
                datPhongModel.MaKh = khachHangModel.MaKh;
                _context.Add(datPhongModel);
                try{
                    await _context.SaveChangesAsync();
                    TempData["DatPhongSuccess"] = "Đặt phòng thành công!";
                    return RedirectToAction(nameof(ChiTietDangKyDatPhong), new{MaDp = datPhongModel.MaDp});
                }catch(SqlException ex)
                {
                    ModelState.AddModelError("", $"Cập nhật khách hàng bị lỗi {ex.Message}");
                }
                return View(datPhongModel);
            }
            ModelState.AddModelError("", "Dữ liệu không hợp lệ");
            return View(datPhongModel);
        }

        public IActionResult ChiTietDangKyDatPhong(int? MaDp)
        {
            if (TempData["DatPhongSuccess"] != null)
            {
                ViewData["DatPhongThanhCong"] = TempData["DatPhongSuccess"].ToString();
            }
            if (MaDp != null)
            {
                var datPhongModel = _context.DatPhongs.Include(dp => dp.MaKhNavigation)
                                                        .Include(dp => dp.MaPNavigation)
                                                        .Include(dp => dp.MaPNavigation.MaLpNavigation)
                                                        .Where(dp => dp.MaDp == MaDp.Value).FirstOrDefault();
                if (datPhongModel != null)
                {
                    var listDatDichVu = _context.DatDichVus.Include(ddv => ddv.MaDvNavigation)
                                                            .Include(ddv => ddv.MaDvNavigation.MaLdvNavigation)
                                                            .Include(ddv => ddv.MaKhNavigation)
                                                            .Where(ddv => ddv.MaDp == MaDp.Value).ToList();
                    ViewData["ListDatDichVu"] = listDatDichVu;
                    var hoaDon = _context.HoaDons.Where(hd => hd.MaDp == MaDp.Value).FirstOrDefault();
                    ViewData["HoaDon"] = hoaDon;
                    System.Console.WriteLine(datPhongModel.MaKhNavigation.HinhAnh);
                    return View(datPhongModel);
                }
                ViewData["ThongBaoLoi"] = "Không tìm thấy dữ liệu hợp lệ!";
                return View();
            }
            ViewData["ThongBaoLoi"] = "Không tìm thấy dữ liệu hợp lệ";
            return View();
        }


        [HttpPost]
        public IActionResult GeneratePDF(string html)
        {
            // System.Console.WriteLine(html);
            html = html.Replace("StrTag", "<").Replace("EndTag", ">");
            var htmlToPdf = new HtmlToPdf();
            var pdfDocumnet = htmlToPdf.ConvertHtmlString(html);
            byte[] pdfFile = pdfDocumnet.Save();
            pdfDocumnet.Close();
            return File(pdfFile, "application/pdf", $"hoadondichvu{DateTime.Now.ToString()}.pdf");
        }

        public bool validateKhachHangModel(KhachHangModel khModel)
        {
            int dem = 0;
            if(string.IsNullOrEmpty(khModel.HoTen))
            {
                dem++;
                ViewData["HoTenError"] = "Họ tên không được để trống!";
            }
            else if(khModel.HoTen.Length < 3 || khModel.HoTen.Length > 50)
            {
                dem++;
                ViewData["HoTenError"] = "Họ tên phải có ít nhất 3 ký tự và nhiều nhất 50 ký tự!";
            }
            if(string.IsNullOrEmpty(khModel.Cccd))
            {
                dem++;
                ViewData["CCCDError"] = "CCCD/CMND không được để trống!";
            }
            else if (khModel.Cccd.Length != 12 && khModel.Cccd.Length != 9)
            {
                dem++;
                ViewData["CCCDError"] = "CMND phải có 9 chữ số. CCCD phải có 12 chữ số.";
            }
            else if (!decimal.TryParse(khModel.Cccd, out decimal result))
            {
                dem++;
                ViewData["CCCDError"] = "CMND/CCD phải là các chữ số";
            }
            if (khModel.NgaySinh == DateTime.MinValue || khModel.NgaySinh == null)
            {
                dem++;
                ViewData["NgaySinhError"] = "Ngày sinh không được để trống!";
            }
            if(string.IsNullOrEmpty(khModel.SoDt))
            {
                dem++;
                ViewData["SoDTError"] = "Số điện thoại không được để trống!";
            }
            else if (khModel.SoDt.Length != 11 && khModel.Cccd.Length != 10)
            {
                dem++;
                ViewData["SoDTError"] = "Số điện thoại phải có 10 - 11 chữ số";
            }
            else if (!decimal.TryParse(khModel.SoDt, out decimal result))
            {
                dem++;
                ViewData["SoDTError"] = "Số điện thoại phải là các chữ số";
            }
            if(khModel.GioiTinh == null)
            {
                dem++;
                ViewData["GioiTinhError"] = "Giới tính không được để trống!";
            }
            if(dem > 0)
                return false;
            return true;
        }

        [HttpGet]
        public IActionResult DanhSachPhongDangSuDung()
        {
            // var a = base.DanhSachPhongDangSuDung();
            return View(base.DanhSachPhongDangSuDung());
        }
    }
}