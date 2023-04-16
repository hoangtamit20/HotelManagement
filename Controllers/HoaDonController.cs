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

namespace QLKSMVC.Controllers
{
    public class HoaDonController : BaseController
    {
        public HoaDonController(QuanLyKhachSanDbContext context) : base(context)
        {
        }

        // GET: HoaDon
        public async Task<IActionResult> Index()
        {
            if (TempData["ThongBaoThanhCong"] != null)
            {
                ViewData["ThongBaoThanhCong"] = TempData["ThongBaoThanhCong"].ToString();
            }
            //sort hóa đơn theo ngày hóa đơn
            var quanLyKhachSanDbContext = _context.HoaDons.Include(h => h.MaDpNavigation)
                                                            .Include(h => h.MaDpNavigation.MaKhNavigation)
                                                            .Include(h => h.MaNvNavigation)
                                                            .Include(h => h.MaDpNavigation.MaPNavigation)
                                                            .Include(h => h.MaDpNavigation.MaPNavigation.MaLpNavigation)
                                                            .OrderByDescending(hd => hd.NgayHoaDon.Value);
            return View(await quanLyKhachSanDbContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult GetDataDatPhong(int? MaDp)
        {
            if (MaDp != null)
            {
                TempData["MaDp"] = MaDp.Value;
            }
            return RedirectToAction(nameof(ThanhToanHoaDon));
        }

        [HttpGet]
        public IActionResult ThanhToanHoaDon()
        {
            if (TempData["MaDp"] != null && _context.DatPhongs != null && _context.NhanViens != null && _context.HoaDons != null)
            {
                var MaDp = (int)TempData["MaDp"];
                TempData["MaDp"] = MaDp;
                var datPhong = _context.DatPhongs.Include(dp => dp.MaKhNavigation).Include(dp => dp.MaPNavigation).Include(dp => dp.MaPNavigation.MaLpNavigation)
                                                    .Where(dp => dp.MaDp == MaDp).FirstOrDefault();
                var tenDN = base.getTenDn();
                ViewData["NhanVien"] = _context.NhanViens.Include(nv => nv.MaCvNavigation).Include(nv => nv.UserNameNavigation)
                                                            .Where(nv => nv.UserName == tenDN).FirstOrDefault();
                ViewData["HoaDon"] = _context.HoaDons.Include(hd => hd.MaDpNavigation).Include(hd => hd.MaNvNavigation)
                                                        .Where(hd => hd.MaDp == MaDp).FirstOrDefault();
                ViewData["ListDatDichVu"] = _context.DatDichVus.Include(ddv => ddv.MaDvNavigation).Where(ddv => ddv.MaDp == MaDp).ToList();
                var list = _context.DatDichVus.Include(ddv => ddv.MaDvNavigation).Where(ddv => ddv.MaDp == MaDp).ToList();
                ViewData["TongTienDichVu"] = list.Where(ddv => ddv.MaDp == MaDp).Sum(ddv => ddv.TongTien.Value);
                var ngayThanhToan = DateTime.Now - datPhong.NgayBatDau.Value;
                ViewData["TongTienPhong"] = (ngayThanhToan.Days < 1 ? 1 : (ngayThanhToan.Days >= 1 && ngayThanhToan.Hours >= 12) ? (ngayThanhToan.Days + 1) : ngayThanhToan.Days) * datPhong.MaPNavigation.MaLpNavigation.DonGia.Value;
                ViewData["MaHD"] = _context.HoaDons.Where(hd => hd.MaDp == MaDp).FirstOrDefault() == null ? (_context.HoaDons.OrderByDescending(hd => hd.MaHddp).FirstOrDefault().MaHddp + 1) : _context.HoaDons.Where(hd => hd.MaDp == MaDp).FirstOrDefault().MaHddp;
                if (datPhong != null)
                {
                    ViewData["DatPhong"] = datPhong;
                    return View();
                }
            }
            ViewData["ThongBaoLoi"] = "Không tìm thấy dữ liệu phù hợp!";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ThanhToanHoaDon(HoaDonModel hdModel)
        {
            TempData["MaDp"] = hdModel.MaDp;
            if (ModelState.IsValid)
            {
                var datPhongModel = await _context.DatPhongs.Where(dp => dp.MaDp == hdModel.MaDp).FirstOrDefaultAsync();
                _context.Add(hdModel);
                try{
                    await _context.SaveChangesAsync();
                    ViewData["ThongBaoThanhCong"] = "Thanh toán thành công!";
                    TempData["ThongBaoThanhCong"] = "Thanh toán thành công!";
                    return RedirectToAction(nameof(Index));
                }catch(SqlException ex){
                    ViewData["ThongBaoLoi"] = $"Lỗi! {ex.Message}";
                    return View(hdModel);
                }
            }
            ViewData["ThongBaoLoi"] = "Dữ liệu không hợp lệ!";
            return View(hdModel);
        }
    }
}