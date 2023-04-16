using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QLKSMVC.Controllers;
using QLKSMVC.Data;
using QLKSMVC.Models;

namespace QLKSMVC.Base
{
    public class BaseController : Controller
    {
        protected readonly QuanLyKhachSanDbContext _context;

        public BaseController(QuanLyKhachSanDbContext context)
        {
            _context = context;
        }

        public bool KiemTraPhanQuyen(string roleName)
        {
            // bool result = false;
            int roleId = -99;
            string? strCookie =
                HttpContext.Request.Cookies["DangNhap"] == null
                    ? ""
                    : HttpContext.Request.Cookies["DangNhap"];
            if (!string.IsNullOrEmpty(strCookie) && _context.Accounts != null)
            {
                BaseClass bs = new BaseClass();
                strCookie = bs.deCodeHash(strCookie);
                string tenDN = strCookie.Split(";")[0];
                Int32.TryParse(strCookie.Split(";")[1], out roleId);
                if (_context.Accounts.Where(tk => tk.UserName == tenDN).FirstOrDefault() != null) // đã đăng nhập
                {
                    if (!string.IsNullOrEmpty(roleName)) // trường sử dụng phân quyền để truy cập
                    {
                        if (roleName.Equals("Admin") && roleId == 99)
                        {
                            return true;
                        }
                        else if (roleName.Equals("QuanLy") && roleId == 1)
                        {
                            return true;
                        }
                        else if (roleName.Equals("NhanVien") && roleId == 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else // trường hợp xác nhận đăng nhập
                    {
                        return true;
                    }
                }
                else
                {
                    // cookie vẫn được lưu nhưng tài khoản trong database đã bị xóa -> tiến hành xóa cookie
                    xoaCookieDangNhap();
                    ModelState.AddModelError(
                        "",
                        "Tài khoản đã không còn tồn tại trên hệ thống! Cookie đã được xóa bỏ!"
                    );
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void xoaCookieDangNhap()
        {
            foreach (var cookie in HttpContext.Request.Cookies)
            {
                if (cookie.Key.Equals("DangNhap"))
                {
                    Response.Cookies.Delete(cookie.Key);
                }
            }
        }

        public IActionResult ChuyenHuong()
        {
            if (HttpContext.Request.Cookies["DangNhap"] != null)
            {
                BaseClass bs = new BaseClass();
                int roleId = Int32.Parse(
                    bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[1]
                );
                if (roleId == 99)
                {
                    return RedirectToAction("Index", "Account", new {phanQuyenErrorMessage = 1});
                }
                else if (roleId == 1)
                {
                    return RedirectToAction("Index", "NhanVien", new {phanQuyenErrorMessage = 1});
                }
                else if (roleId == 0)
                {
                    return RedirectToAction("DanhSachDatPhong", "DatPhong", new {phanQuyenErrorMessage = 1});
                }
                return NotFound();
            }
            else
            {
                return RedirectToAction("DangNhap", "Account", new {dangNhapErrorMessage = 1});
            }
        }

        public void addModelError()
        {
            var errors = ModelState.Select(x => x.Value.Errors).Where(y => y.Count > 0).ToList();
            ViewBag.Errors = errors;
        }

        public void taoMaDp()
        {
            var datP = _context.DatPhongs.OrderByDescending(dp => dp.MaDp).FirstOrDefault();
            if (datP != null)
            {
                ViewData["MaDp"] = "MDP" + (datP.MaDp + 1).ToString();
            }
            else
            {
                ViewData["MaDp"] = "MDP1";
            }
        }

        public void taoMaKH()
        {
            var kh = _context.KhachHangs.OrderByDescending(dp => dp.MaKh).FirstOrDefault();
            if (kh != null)
            {
                ViewData["MaKh"] = "MaKH" + (kh.MaKh + 1).ToString();
            }
            else
            {
                ViewData["MaKh"] = "MaKH1";
            }
        }

        public bool KiemTraThoiGianDatPhong(DatPhongModel datPhong)
        {
            bool check = true;
            var timeCheckIn = datPhong.NgayBatDau;
            var timeCheckOut = datPhong.NgayKetThuc;
            var datPhongs =
                from dp in _context.DatPhongs
                join hddp in _context.HoaDons on dp.MaDp equals hddp.MaDp into g
                from x in g.DefaultIfEmpty()
                select new { dp = dp, maHD = x.MaHddp };
            foreach (var item in datPhongs)
            {
                if (item.maHD == null && item.dp.MaP == datPhong.MaP)
                {
                    TimeSpan timeSpan1 = (datPhong.NgayBatDau.Value - item.dp.NgayKetThuc.Value);
                    TimeSpan timeSpan2 = (item.dp.NgayBatDau.Value - datPhong.NgayKetThuc.Value);
                    if (!(timeSpan1.TotalMinutes > 0 || timeSpan2.TotalMinutes > 0))
                    {
                        check = false;
                    }
                }
            }
            return check;
        }

        public bool KiemTraThoiGianDatDichVu(DatDichVuModel datDV)
        {
            bool check = true;
            var ngayDatDichVu = datDV.NgayDatDichVu;
            var datPhongs = _context.DatPhongs.FirstOrDefault(d => d.MaDp == datDV.MaDp);

            TimeSpan timeSpan1 = (ngayDatDichVu.Value - datPhongs.NgayBatDau.Value);
            TimeSpan timeSpan2 = (datPhongs.NgayKetThuc.Value - ngayDatDichVu.Value);
            if (timeSpan1.TotalMinutes <= 0 || timeSpan2.TotalMinutes <= 0)
            {
                check = false;
            }
            return check;
        }

        public string getTenDn()
        {
            BaseClass bs = new BaseClass();
            string tenDN = "";
            string? chuoiCookie = HttpContext.Request.Cookies["DangNhap"];
            if (chuoiCookie != null)
            {
                chuoiCookie = bs.deCodeHash(chuoiCookie);
            }
            if (chuoiCookie != null && chuoiCookie.Contains(";"))
            {
                tenDN = chuoiCookie.Split(";")[0];
            }
            return tenDN;
        }

        // (a >= c && a <= d) || (b >= c && b <= d) || (a <= c && b >= d)
        public bool checkNgayDatPhong(DatPhongModel DatPhong, DateTime NgayBatDau, DateTime NgayKetThuc)
        {
            if ((DatPhong.NgayBatDau.Value <= NgayBatDau && DatPhong.NgayKetThuc.Value >= NgayBatDau)
                    || DatPhong.NgayBatDau.Value <= NgayKetThuc && DatPhong.NgayKetThuc.Value >= NgayKetThuc
                    || DatPhong.NgayBatDau.Value >= NgayBatDau && DatPhong.NgayKetThuc.Value <= NgayKetThuc)
                return true;
            return false;
        }

        public List<DatPhongModel> DanhSachPhongDangSuDung()
        {
            // var danhSachPhongDaDuocDat = _context.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTvpNavigation)
            //                                                 .Join(_context.DatPhongs, p => p.MaP, dp => dp.MaP, (p, dp) => new { Phongs = p, DatPhongs = dp })
            //                                                 .AsEnumerable()
            //                                                 .Where(group => group.DatPhongs.NgayBatDau.Value <= DateTime.Now && group.DatPhongs.NgayKetThuc.Value >= DateTime.Now)
            //                                                 .Select(group => new PhongModel()
            //                                                 {
            //                                                     MaP = group.Phongs.MaP,
            //                                                     SoPhong = group.Phongs.SoPhong,
            //                                                     HinhAnh = group.Phongs.HinhAnh,
            //                                                     MaLp = group.Phongs.MaLp,
            //                                                     MaTvp = group.Phongs.MaTvp,
            //                                                     MoTa = group.Phongs.MoTa,
            //                                                     MaLpNavigation = group.Phongs.MaLpNavigation,
            //                                                     MaTvpNavigation = group.Phongs.MaTvpNavigation
            //                                                 }).ToList();

            var danhSachPhongDaDuocDat = _context.DatPhongs.Include(dp => dp.MaKhNavigation)
                                                            .Include(dp => dp.MaPNavigation)
                                                            .Include(dp => dp.MaPNavigation.MaLpNavigation)
                                                            .Include(dp => dp.MaPNavigation.MaTvpNavigation)
                                                            .Where(dp => dp.NgayBatDau.Value <= DateTime.Now 
                                                                        && dp.NgayKetThuc.Value >= DateTime.Now 
                                                                        && _context.HoaDons.Where(hd => hd.MaDp == dp.MaDp).FirstOrDefault() == null).ToList();
            return danhSachPhongDaDuocDat;
        }


        public List<PhongModel> DanhSachDatPhongTrong(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            if (_context.Phongs != null && _context.DatPhongs != null)
            {
                // Lấy ra danh sách tất cả các phòng
                var danhSachPhongTrong = _context.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTvpNavigation).ToList();
                // Lấy ra danh sách phòng đang được đặt trong khoản thời gian từ ngày đến -> ngày đi
                var danhSachPhongDaDuocDat = _context.Phongs.Include(p => p.MaLpNavigation).Include(p => p.MaTvpNavigation)
                                                            .Join(_context.DatPhongs, p => p.MaP, dp => dp.MaP, (p, dp) => new { Phongs = p, DatPhongs = dp })
                                                            .AsEnumerable()
                                                            .Where(group => checkNgayDatPhong(group.DatPhongs, ngayBatDau, ngayKetThuc))
                                                            .Select(group => new PhongModel()
                                                            {
                                                                MaP = group.Phongs.MaP,
                                                                SoPhong = group.Phongs.SoPhong,
                                                                HinhAnh = group.Phongs.HinhAnh,
                                                                MaLp = group.Phongs.MaLp,
                                                                MaTvp = group.Phongs.MaTvp,
                                                                MoTa = group.Phongs.MoTa,
                                                                MaLpNavigation = group.Phongs.MaLpNavigation,
                                                                MaTvpNavigation = group.Phongs.MaTvpNavigation
                                                            }).ToList();
                foreach (var item in danhSachPhongTrong.ToArray())
                {
                    foreach (var item1 in danhSachPhongDaDuocDat)
                    {
                        if (item1.MaP == item.MaP)
                        {
                            danhSachPhongTrong.Remove(item);
                        }
                    }
                }
                return danhSachPhongTrong;
            }
            return null;
        }

        public bool checkDateValid(DateTime d1, DateTime d2, DateTime dateCheck)
        {
            if (dateCheck.Ticks < DateTime.Now.Ticks)
                return false;
            if (dateCheck.Ticks >= d1.Ticks && dateCheck.Ticks <= d2.Ticks)
            {
                return false;
            }
            return true;
        }


        //Tâm thêm
        // public List<ThongKeDoanhThuTheoThangModel> listDoanhThuTheoThang(int year)
        // {
        //     var listResult = new List<ThongKeDoanhThuTheoThangModel>();

        //     if (_context.HoaDonDatPhongs != null)
        //     {
        //         listResult = _context.HoaDonDatPhongs
        //             .Include(hd => hd.MaDpNavigation)
        //             .Where(hd => hd.NgayHd.Year == year)
        //             .GroupBy(hd => hd.NgayHd.Month)
        //             .AsEnumerable()
        //             .Select(
        //                 group =>
        //                     new ThongKeDoanhThuTheoThangModel
        //                     {
        //                         month = group.Key,
        //                         total = group.Sum(
        //                             g =>
        //                                 (decimal)(
        //                                     Math.Ceiling(
        //                                         (g.NgayHd - g.MaDpNavigation.NgayBatDau).TotalDays
        //                                     )
        //                                 ) * g.MaDpNavigation.TongTien
        //                         )
        //                     }
        //             )
        //             .ToList();
        //     }
        //     return listResult;
        // }

        // public List<NhanVienModel> listTopNhanVienTieuBieu(int year)
        // {
        //     var listResult = new List<NhanVienModel>();
        //     if (_context.NhanViens != null && _context.HoaDonDatPhongs != null)
        //     {
        //         var list = _context.HoaDonDatPhongs
        //             .Include(hd => hd.MaDpNavigation)
        //             .Include(hd => hd.MaDpNavigation.MaPNavigation)
        //             .Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
        //             .Where(hd => hd.NgayHd.Year == year)
        //             .GroupBy(hd => hd.MaNv)
        //             .Select(group => new { MaNV = group.Key, Count = group.Count() })
        //             .ToList();
        //         list = list.OrderByDescending(l => l.Count).Take(10).ToList();
        //         int dem = 0;
        //         foreach (var item in list)
        //         {
        //             dem++;
        //             listResult.Add(
        //                 _context.NhanViens
        //                     .Include(nv => nv.UserNameNavigation)
        //                     .Include(nv => nv.UserNameNavigation.IdRoleNavigation)
        //                     .Where(nv => nv.MaNv == item.MaNV)
        //                     .FirstOrDefault()
        //             );
        //             if (dem == 10)
        //                 break;
        //         }
        //     }
        //     return listResult;
        // }

        // public List<KhachHangModel> listTopKhachHangTieuBieu(int year)
        // {
        //     var listResult = new List<KhachHangModel>();
        //     if (_context.DatPhongs != null && _context.KhachHangs != null)
        //     {
        //         var list = _context.DatPhongs
        //             .Where(dp => dp.NgayBatDau.Year == year)
        //             .GroupBy(dp => dp.MaKh)
        //             .Select(group => new { MaKH = group.Key, Count = group.Count() })
        //             .ToList();
        //         list = list.OrderByDescending(group => group.Count).ToList();
        //         int dem = 0;
        //         foreach (var item in list)
        //         {
        //             dem++;
        //             listResult.Add(
        //                 _context.KhachHangs.Where(kh => kh.MaKh == item.MaKH).FirstOrDefault()
        //             );
        //             if (dem == 3)
        //                 break;
        //         }
        //     }
        //     return listResult;
        // }

        // public List<ThongKeDoanhThuLoaiPhongModel> listThongKeDoanhThuTheoLoaiPhong(int year)
        // {
        //     var listDoanhThuLoaiPhong = new List<ThongKeDoanhThuLoaiPhongModel>();
        //     if (_context.HoaDonDatPhongs != null && _context.LoaiPhongs != null)
        //     {
        //         listDoanhThuLoaiPhong = _context.HoaDonDatPhongs
        //             .Include(hd => hd.MaDpNavigation)
        //             .Include(hd => hd.MaDpNavigation.MaPNavigation)
        //             .Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
        //             .Where(hd => hd.NgayHd.Year == year)
        //             .GroupBy(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation.TenLp)
        //             .AsEnumerable()
        //             .Select(
        //                 group =>
        //                     new ThongKeDoanhThuLoaiPhongModel
        //                     {
        //                         tenLP = group.Key,
        //                         total = group.Sum(
        //                             g =>
        //                                 (decimal)(
        //                                     Math.Ceiling(
        //                                         (g.NgayHd - g.MaDpNavigation.NgayBatDau).TotalDays
        //                                     )
        //                                 ) * g.MaDpNavigation.TongTien
        //                         )
        //                     }
        //             )
        //             .ToList();
        //     }
        //     if (listDoanhThuLoaiPhong.Find(t => t.tenLP.Contains("Phòng Đơn")) == null)
        //     {
        //         listDoanhThuLoaiPhong.Add(
        //             new ThongKeDoanhThuLoaiPhongModel() { tenLP = "Phòng Đơn", total = 0 }
        //         );
        //     }
        //     if (listDoanhThuLoaiPhong.Find(t => t.tenLP.Contains("Phòng Đôi")) == null)
        //     {
        //         listDoanhThuLoaiPhong.Add(
        //             new ThongKeDoanhThuLoaiPhongModel() { tenLP = "Phòng Đôi", total = 0 }
        //         );
        //     }
        //     if (listDoanhThuLoaiPhong.Find(t => t.tenLP.Contains("Phòng Gia Đình")) == null)
        //     {
        //         listDoanhThuLoaiPhong.Add(
        //             new ThongKeDoanhThuLoaiPhongModel() { tenLP = "Phòng Gia Đình", total = 0 }
        //         );
        //     }
        //     if (listDoanhThuLoaiPhong.Find(t => t.tenLP.Contains("Phòng Vip")) == null)
        //     {
        //         listDoanhThuLoaiPhong.Add(
        //             new ThongKeDoanhThuLoaiPhongModel() { tenLP = "Phòng Vip", total = 0 }
        //         );
        //     }
        //     return listDoanhThuLoaiPhong;
        // }

        // public List<ThongKeSoNgayDatDoanhThuNhanVienModel> listTongSoNgayDatDoanhThuTrongNam(
        //     int year
        // )
        // {
        //     var listResult = new List<ThongKeSoNgayDatDoanhThuNhanVienModel>();
        //     if (_context.HoaDonDatPhongs != null && _context.NhanViens != null)
        //     {
        //         listResult = _context.HoaDonDatPhongs
        //             .Where(hd => hd.NgayHd.Year == year)
        //             .GroupBy(
        //                 hd =>
        //                     new
        //                     {
        //                         hd.MaNv,
        //                         hd.NgayHd.Day,
        //                         hd.NgayHd.Month
        //                     }
        //             )
        //             .AsEnumerable()
        //             .Select(
        //                 group =>
        //                     new
        //                     {
        //                         MaNV = group.Key.MaNv,
        //                         Thang = group.Key.Month,
        //                         Ngay = group.Key.Day,
        //                         total = group.Sum(
        //                             g =>
        //                                 (decimal)(
        //                                     Math.Ceiling(
        //                                         (g.NgayHd - g.MaDpNavigation.NgayBatDau).TotalDays
        //                                     )
        //                                 ) * g.MaDpNavigation.TongTien
        //                         )
        //                     }
        //             )
        //             .GroupBy(group => new { group.MaNV, group.Thang })
        //             .Select(
        //                 group =>
        //                     new
        //                     {
        //                         MaNV = group.Key.MaNV,
        //                         Thang = group.Key.Thang,
        //                         Ngay = group.Count(),
        //                         Total = group.Sum(l => l.total)
        //                     }
        //             )
        //             .GroupBy(group => group.MaNV)
        //             .Select(
        //                 group =>
        //                     new ThongKeSoNgayDatDoanhThuNhanVienModel
        //                     {
        //                         MaNV = group.Key,
        //                         TongNgay = group.Sum(l => l.Ngay),
        //                         Total = group.Sum(l => l.Total)
        //                     }
        //             )
        //             .ToList();
        //     }
        //     return listResult;
        // }

        // public int TongSoKHTrongNam(int year)
        // {
        //     int result = 0;
        //     if (_context.HoaDonDatPhongs != null && _context.KhachHangs != null)
        //     {
        //         result = _context.HoaDonDatPhongs
        //             .Include(hd => hd.MaDpNavigation)
        //             .Include(hd => hd.MaDpNavigation.MaKhNavigation)
        //             .Where(hd => hd.NgayHd.Year == year)
        //             .GroupBy(hd => hd.MaDpNavigation.MaKh)
        //             .Select(group => new { MaKH = group.Key, Count = group.Count() })
        //             .Count();
        //     }
        //     return result;
        // }

        // public decimal TongDoanhThuTrongNam(int year)
        // {
        //     decimal? result = 0;
        //     if (_context.HoaDonDatPhongs != null)
        //     {
        //         var i = _context.HoaDonDatPhongs
        //             .Include(hd => hd.MaDpNavigation)
        //             .Where(hd => hd.NgayHd.Year == year)
        //             .AsEnumerable()
        //             .Sum(
        //                 g =>
        //                     (decimal)(
        //                         Math.Ceiling((g.NgayHd - g.MaDpNavigation.NgayBatDau).TotalDays)
        //                     ) * g.MaDpNavigation.TongTien
        //                     ?? 0
        //             );
        //         var i2 = _context.DatDichVus
        //             .Include(ddv => ddv.MaDvNavigation)
        //             .Where(ddv => ddv.NgayDatDichVu.Year == year)
        //             .Sum(ddv => ddv.SoLuong * ddv.MaDvNavigation.DonGia);
        //         result = i + i2;
        //     }

        //     return result.Value;
        // }

        // public int TongSoNgayDatDoanhThu(int year)
        // {
        //     int result = 0;
        //     if (_context.HoaDonDatPhongs != null)
        //     {
        //         result = _context.HoaDonDatPhongs
        //             .Where(hd => hd.NgayHd.Year == year)
        //             .GroupBy(hd => new { hd.NgayHd.Month, hd.NgayHd.Day })
        //             .Select(
        //                 group =>
        //                     new
        //                     {
        //                         Thang = group.Key.Month,
        //                         Ngay = group.Key.Day,
        //                         Count = group.Count()
        //                     }
        //             )
        //             .GroupBy(group => group.Thang)
        //             .Select(group => new { Thang = group.Key, Count = group.Count() })
        //             .Sum(group => group.Count);
        //     }
        //     return result;
        // }

        // public int TongSoPhongDuocSuDungTrongNam(int year)
        // {
        //     int result = 0;
        //     if (_context.HoaDonDatPhongs != null && _context.Phongs != null)
        //     {
        //         result = _context.HoaDonDatPhongs
        //             .Include(hd => hd.MaDpNavigation)
        //             .Include(hd => hd.MaDpNavigation.MaPNavigation)
        //             .Where(hd => hd.NgayHd.Year == year)
        //             .GroupBy(hd => hd.MaDpNavigation.MaPNavigation.SoPhong)
        //             .Select(group => new { Phong = group.Key, Count = group.Count() })
        //             .Count();
        //     }
        //     return result;
        // }



        // public List<object> ThongKeLoaiPhong(int year, int? quy, int? thang, int? tuan, DateTime? ngayBatDau, DateTime? ngayKetThuc)
        // {
        //     List<object> listResult = new List<object>();
        //     // -> Thống kê theo năm
        //     if (quy == null && thang == null && ngayBatDau == null && ngayKetThuc == null)
        //     {
        //         var list = _context.HoaDons.Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
        //                                 .Where(hd => (hd.NgayHoaDon.Value).Year == year)
        //                                 .Select(hd => new {TenLp = hd.MaDpNavigation.MaPNavigation.MaLpNavigation.TenLp, TongTien = hd.TongTien.Value})
        //                                 .GroupBy(hd => hd.TenLp)
        //                                 .Select(group => new {TenLp = group.Key, SoLuongDatPhong = group.Count(), TongTien = group.Sum(hd => hd.TongTien)})
        //                                 .ToList();
        //     }
        //     // Thống kê theo quý của năm
        //     else if (quy != null && thang == null)
        //     {
        //         var list = _context.HoaDons.Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
        //                                     .Where(hd => (hd.NgayHoaDon.Value).Year == year && 
        //                                                                                     (quy.Value == 1 ? ((hd.NgayHoaDon.Value).Month >= 1 && (hd.NgayHoaDon.Value).Month <= 3)
        //                                                                                     : quy.Value == 2 ? ((hd.NgayHoaDon.Value).Month >= 4 && (hd.NgayHoaDon.Value).Month <= 6)
        //                                                                                     : quy.Value == 3 ? ((hd.NgayHoaDon.Value).Month >= 7 && (hd.NgayHoaDon.Value).Month <= 9)
        //                                                                                     : ((hd.NgayHoaDon.Value).Month >= 10 && (hd.NgayHoaDon.Value).Month <= 12)))
        //                                     .Select(hd => new {TenLp = hd.MaDpNavigation.MaPNavigation.MaLpNavigation.TenLp, TongTien = hd.TongTien.Value})
        //                                     .GroupBy(hd => hd.TenLp)
        //                                     .Select(group => new {TenLp = group.Key, SoLuongDatPhong = group.Count(), TongTien = group.Sum(hd => hd.TongTien)})
        //                                     .ToList();
        //     }
        //     // Thống kê theo tháng của năm
        //     else if (quy == null && thang != null && tuan == null)
        //     {
        //         var list = _context.HoaDons.Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
        //                                     .Where(hd => (hd.NgayHoaDon.Value).Year == year && (hd.NgayHoaDon.Value).Month == thang.Value)
        //                                     .Select(hd => new {TenLp = hd.MaDpNavigation.MaPNavigation.MaLpNavigation.TenLp, TongTien = hd.TongTien.Value})
        //                                     .GroupBy(hd => hd.TenLp)
        //                                     .Select(group => new {TenLp = group.Key, SoLuongDatPhong = group.Count(), TongTien = group.Sum(hd => hd.TongTien)})
        //                                     .ToList();
        //     }





        //     return listResult;
        // }

        public List<ChooseYearModel> ListNam()
        {
            List<ChooseYearModel> listResult = new List<ChooseYearModel>();
            listResult.Add(new ChooseYearModel(){value = -1, text = "Chọn năm thống kê"});
            var list = _context.HoaDons.GroupBy(hd => hd.NgayHoaDon.Value.Year).Select(hd => (hd.Key)).OrderByDescending(p => p).ToList();
            foreach(var item in list)
            {
                listResult.Add(new ChooseYearModel(){value = item, text = item.ToString()});
            }
            return listResult;
        }

        public List<ThongKeLoaiPhongModel> ThongKeLoaiPhong(int year, DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            var list = _context.HoaDons.Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
                            .Where(hd => (ngayBatDau == null || ngayKetThuc == null) ? (hd.NgayHoaDon.Value.Year == year) : (hd.NgayHoaDon.Value >= ngayBatDau.Value && hd.NgayHoaDon.Value <= ngayKetThuc.Value))
                            .Select(hd => new { TenLp = hd.MaDpNavigation.MaPNavigation.MaLpNavigation.TenLp, TongTien = hd.TongTien.Value , MaLp = hd.MaDpNavigation.MaPNavigation.MaLp})
                            .GroupBy(hd => hd.TenLp)
                            .Select(group => new ThongKeLoaiPhongModel { MaLp = group.Select(hd => hd.MaLp).FirstOrDefault() ,TenLp = group.Key, SoLuongDatPhong = group.Count(), TongTien = group.Sum(hd => hd.TongTien) })
                            .ToList();
            if (list.Where(l => l.TenLp.Contains("Phòng đơn")).FirstOrDefault() == null)
            {
                list.Add(new ThongKeLoaiPhongModel(){TenLp = "Phòng đơn", SoLuongDatPhong = 0, TongTien = 0});
            }
            if (list.Where(l => l.TenLp.Contains("Phòng đôi")).FirstOrDefault() == null)
            {
                list.Add(new ThongKeLoaiPhongModel(){TenLp = "Phòng đôi", SoLuongDatPhong = 0, TongTien = 0});
            }
            if (list.Where(l => l.TenLp.Contains("Phòng gia đình")).FirstOrDefault() == null)
            {
                list.Add(new ThongKeLoaiPhongModel(){TenLp = "Phòng gia đình", SoLuongDatPhong = 0, TongTien = 0});
            }
            if (list.Where(l => l.TenLp.Contains("Phòng Vip")).FirstOrDefault() == null)
            {
                list.Add(new ThongKeLoaiPhongModel(){TenLp = "Phòng Vip", SoLuongDatPhong = 0, TongTien = 0});
            }
            list = list.OrderBy(l => l.MaLp).ToList();
            return list;
        }
                                // :
                                // _context.HoaDons.Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
                                //                 .Where(hd => hd.NgayHoaDon.Value.Year == year)
                                //                 .Select(hd => new { TenLp = hd.MaDpNavigation.MaPNavigation.MaLpNavigation.TenLp, TongTien = hd.TongTien.Value })
                                //                 .GroupBy(hd => hd.TenLp)
                                //                 .Select(group => new ThongKeLoaiPhongModel { TenLp = group.Key, SoLuongDatPhong = group.Count(), TongTien = group.Sum(hd => hd.TongTien) })
                                //                 .ToList();

        public int ThongKeKhachHang(int year, DateTime? ngayBatDau, DateTime? ngayKetThuc) => 
                                _context.HoaDons.Include(hd => hd.MaDpNavigation.MaKhNavigation)
                                                .Where(hd => (ngayBatDau == null || ngayKetThuc == null) ? (hd.NgayHoaDon.Value.Year == year) : (hd.NgayHoaDon.Value >= ngayBatDau.Value && hd.NgayHoaDon.Value <= ngayKetThuc.Value))
                                                .Sum(hd => hd.MaDpNavigation.SoNguoi.Value);
                                // :
                                // _context.HoaDons.Include(hd => hd.MaDpNavigation.MaKhNavigation)
                                //                 .Where(hd => hd.NgayHoaDon.Value.Year == year)
                                //                 .Sum(hd => hd.MaDpNavigation.SoNguoi.Value);

        public decimal ThongKeTongDoanhThu(int year, DateTime? ngayBatDau, DateTime? ngayKetThuc) => 
                                _context.HoaDons.Where(hd => (ngayBatDau == null || ngayKetThuc == null) ? (hd.NgayHoaDon.Value.Year == year) : (hd.NgayHoaDon.Value >= ngayBatDau.Value && hd.NgayHoaDon.Value <= ngayKetThuc.Value))
                                                .Sum(hd => hd.TongTien.Value);
                                // :
                                // _context.HoaDons.Where(hd => hd.NgayHoaDon.Value.Year == year)
                                //                 .Sum(hd => hd.TongTien.Value);

        public int ThongKeTongPhongSuDung(int year, DateTime? ngayBatDau, DateTime? ngayKetThuc) =>
                                _context.HoaDons.Include(hd => hd.MaDpNavigation)
                                                .Where(hd => (ngayBatDau == null || ngayKetThuc == null) ? (hd.NgayHoaDon.Value.Year == year) : (hd.NgayHoaDon.Value >= ngayBatDau.Value && hd.NgayHoaDon.Value <= ngayKetThuc.Value))
                                                .Select(hd => new { Phong = hd.MaDpNavigation.MaP })
                                                .GroupBy(h => h.Phong)
                                                .Count();
                                // :
                                // _context.HoaDons.Include(hd => hd.MaDpNavigation)
                                //                 .Where(hd => hd.NgayHoaDon.Value.Year == year)
                                //                 .Select(hd => new { Phong = hd.MaDpNavigation.MaP })
                                //                 .GroupBy(h => h.Phong)
                                //                 .Count();


        public int ThongKeTongNgayDatDoanhThu(int year, DateTime? ngayBatDau, DateTime? ngayKetThuc) =>  
                                _context.HoaDons.Where(hd => (ngayBatDau == null || ngayKetThuc == null) ? (hd.NgayHoaDon.Value.Year == year) : (hd.NgayHoaDon.Value >= ngayBatDau.Value && hd.NgayHoaDon.Value <= ngayKetThuc.Value))
                                                .GroupBy(hd => new { hd.NgayHoaDon.Value.Year, hd.NgayHoaDon.Value.Month, hd.NgayHoaDon.Value.Day })
                                                .AsEnumerable()
                                                .Count();
                                // :
                                // _context.HoaDons.Where(hd => hd.NgayHoaDon.Value.Year == year)
                                //                 .GroupBy(hd => new { hd.NgayHoaDon.Value.Year, hd.NgayHoaDon.Value.Month, hd.NgayHoaDon.Value.Day })
                                //                 .AsEnumerable()
                                //                 .Count();




        public List<KhachHangTieuBieuModel> listKhachHangTieuBieu(int year, DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            return _context.HoaDons.Include(hd => hd.MaDpNavigation).Include(hd => hd.MaDpNavigation.MaPNavigation).Include(hd => hd.MaDpNavigation.MaPNavigation.MaLpNavigation)
                                            .Include(hd => hd.MaDpNavigation.MaKhNavigation)
                                            .Where(hd => (ngayBatDau == null || ngayKetThuc == null) ? (hd.NgayHoaDon.Value.Year == year) : (hd.NgayHoaDon.Value >= ngayBatDau.Value && hd.NgayHoaDon.Value <= ngayKetThuc.Value))
                                            .GroupBy(hd => hd.MaDpNavigation.MaKh)
                                            .Select(hd => new KhachHangTieuBieuModel(){
                                                MaKh = hd.Key,
                                                HoTen = hd.Select(h => h.MaDpNavigation.MaKhNavigation.HoTen).FirstOrDefault(),
                                                HinhAnh = hd.Select(h => h.MaDpNavigation.MaKhNavigation.HinhAnh).FirstOrDefault(),
                                                SoDt = hd.Select(h => h.MaDpNavigation.MaKhNavigation.SoDt).FirstOrDefault(),
                                                SLDatPhong = hd.Count(),
                                                TongTien = hd.Sum(h => h.TongTien.Value),
                                            })
                                            .OrderByDescending(group => group.SLDatPhong)
                                            .ThenByDescending(group => group.TongTien)
                                            .Take(10)
                                            .ToList();
        }

        public List<NhanVienTieuBieuModel> listNhanVienTieuBieu(int year)
        {
            var result = _context.HoaDons.Include(hd => hd.MaDpNavigation).Include(hd => hd.MaNvNavigation)
                                            .Include(hd => hd.MaNvNavigation.MaCvNavigation)
                                            .Where(hd => hd.NgayHoaDon.Value.Year == year)
                                            .GroupBy(hd => hd.MaNv)
                                            .Select(group => new NhanVienTieuBieuModel{
                                                MaNv = group.Key,
                                                HoTen = group.Select(hd => hd.MaNvNavigation.HoTen).FirstOrDefault(),
                                                ChucVu = group.Select(hd => hd.MaNvNavigation.MaCvNavigation.TenCv).FirstOrDefault(),
                                                Email = group.Select(hd => hd.MaNvNavigation.Email).FirstOrDefault(),
                                                HinhAnh = group.Select(hd => hd.MaNvNavigation.HinhAnh).FirstOrDefault(),
                                                SLThanhToan = group.Count(),
                                                SoNgayLamViec = group.Select(hd => _context.TongLuongs.Where(tl => tl.NgayNhanLuong.Value.Year == year && tl.MaNv == group.Key).Sum(tl => tl.SoNgayLam.Value)).FirstOrDefault(),
                                                TongLuong = group.Select(hd => _context.TongLuongs.Where(tl => tl.NgayNhanLuong.Value.Year == year && tl.MaNv == group.Key).Sum(tl => tl.TongTien.Value)).FirstOrDefault(),
                                                TongTien = group.Sum(hd => hd.TongTien.Value),
                                                TrangThai = _context.NhanVienNghiViecs.Where(nvnv => nvnv.MaNv == group.Key).FirstOrDefault() == null ? true : false
                                            })
                                            .OrderByDescending(tb => tb.SLThanhToan).ThenByDescending(tb => tb.SoNgayLamViec).ThenByDescending(tb => tb.TongTien)
                                            .Take(10)
                                            .ToList();
            
            return result;
        }


        public List<DoanhThuTheoNamModel> ThongKeDoanhThuTheoNam() =>
             _context.HoaDons.GroupBy(hd => hd.NgayHoaDon.Value.Year)
                                                .Select(group => new DoanhThuTheoNamModel{Nam = group.Key, TongDoanhThu = group.Sum(hd => hd.TongTien.Value)})
                                                .OrderBy(group => group.Nam)
                                                .ToList();
        public List<DoanhThuTheoThangCuaNamModel> ThongKeDoanhThuTheoThang(int year)
        {
            var listResult = _context.HoaDons.Where(hd => hd.NgayHoaDon.Value.Year == year)
                                                .GroupBy(hd => hd.NgayHoaDon.Value.Month)
                                                .Select(group => new DoanhThuTheoThangCuaNamModel(){Thang = group.Key, TongDoanhThu = group.Sum(hd => hd.TongTien.Value)})
                                                .OrderBy(group => group.Thang)
                                                .ToList();
            for (int i = 1 ; i < 13 ; i++)
            {
                if (listResult.Where(l => l.Thang == i).FirstOrDefault() == null)
                {
                    listResult.Add(new DoanhThuTheoThangCuaNamModel(){Thang = i, TongDoanhThu = 0});
                }
            }
            listResult = listResult.OrderBy(l => l.Thang).ToList();
            return listResult;
        }

        public List<LuongNhanVienTheoNamModel> ThongKeLuongNhanVienTheoNam()
        {
            var listResult = _context.TongLuongs.GroupBy(tl => tl.NgayNhanLuong.Value.Year)
                                                .Select(group => new LuongNhanVienTheoNamModel(){Nam = group.Key, TongLuong = group.Sum(tl => tl.TongTien.Value)})
                                                .OrderBy(group => group.Nam)
                                                .ToList();

            var min = listResult.Min(lb => lb.Nam);
            for (int i = listResult.Min(lb => lb.Nam); i <= DateTime.Now.Year ; i++)
            {
                if (listResult.Where(l => l.Nam == i).FirstOrDefault() == null)
                {
                    listResult.Add(new LuongNhanVienTheoNamModel(){Nam = i, TongLuong = 0});
                }
            }
            listResult = listResult.OrderBy(l => l.Nam).ToList();
            return listResult;
        }

        public List<LuongNhanVienTheoThangModel> ThongKeLuongNhanVienTheoThang(int year)
        {
            var listResult = _context.TongLuongs.Where(tl => tl.NgayNhanLuong.Value.Year == year)
                                                .GroupBy(tl => tl.NgayNhanLuong.Value.Month)
                                                .Select(group => new LuongNhanVienTheoThangModel(){Thang = group.Key, TongTien = group.Sum(tl => tl.TongTien.Value)})
                                                .OrderBy(group => group.Thang)
                                                .ToList();
            for (int i = 1 ; i < 13 ; i++)
            {
                if (listResult.Where(l => l.Thang == i).FirstOrDefault() == null)
                {
                    listResult.Add(new LuongNhanVienTheoThangModel(){Thang = i, TongTien = 0});
                }
            }
            listResult = listResult.OrderBy(l => l.Thang).ToList();
            return listResult;
        }

        public List<TieuThuLoaiDichVuModel> ThongKeLuongTieuThuTheoLoaiDichVu(int year)
        {
            var listResult = _context.DatDichVus.Include(ddv => ddv.MaDvNavigation).Include(ddv => ddv.MaDvNavigation.MaLdvNavigation)
                                                .Where(ddv => ddv.NgayDatDichVu.Value.Year == year)
                                                .GroupBy(ddv => ddv.MaDvNavigation.MaLdv)
                                                .Select(group => new TieuThuLoaiDichVuModel(){MaLdv = group.Key, TenLdv = group.Select(ddv => ddv.MaDvNavigation.MaLdvNavigation.TenLdv).FirstOrDefault(), SoLuong = group.Sum(ddv => ddv.SoLuong.Value)})
                                                .OrderBy(group => group.MaLdv)
                                                .ToList();
            foreach(var item in _context.LoaiDichVus.ToList())
            {
                if (listResult.Where(l => l.MaLdv == item.MaLdv).FirstOrDefault() == null)
                {
                    listResult.Add(new TieuThuLoaiDichVuModel(){MaLdv = item.MaLdv, TenLdv = item.TenLdv, SoLuong = 0});
                }
            }
            return listResult.OrderBy(l => l.MaLdv).ToList();
        }
    }
}