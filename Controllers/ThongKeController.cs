using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using QLKSMVC.Base;
using QLKSMVC.Data;

namespace QLKSMVC.Controllers
{
    public class ThongKeController : BaseController
    {
        public ThongKeController(QuanLyKhachSanDbContext context) : base(context)
        {
        }

        public IActionResult Index()
        {
            // if (base.KiemTraPhanQuyen("Admin") || base.KiemTraPhanQuyen("QuanLy"))
            // {
                ViewData["ChooseYear"] = new SelectList(base.ListNam(), "value", "text", DateTime.Now.Year);
                ViewData["KhachHang"] = base.ThongKeKhachHang(DateTime.Now.Year, null, null);
                ViewData["LoaiPhong"] = base.ThongKeLoaiPhong(DateTime.Now.Year, null, null);
                ViewData["TongDoanhThu"] = base.ThongKeTongDoanhThu(DateTime.Now.Year, null, null);
                ViewData["TongPhongSuDung"] = base.ThongKeTongPhongSuDung(DateTime.Now.Year, null, null);
                ViewData["TongNgayDatDoanhThu"] = base.ThongKeTongNgayDatDoanhThu(DateTime.Now.Year, null, null);
                ViewData["ListTopKHTieuBieu"] = base.listKhachHangTieuBieu(DateTime.Now.Year, null, null);
                ViewData["ListNVTieuBieu"] = base.listNhanVienTieuBieu(DateTime.Now.Year);
                ViewData["DoanhThuTheoNam"] = base.ThongKeDoanhThuTheoNam();
                ViewData["LuongNVTheoNam"] = base.ThongKeLuongNhanVienTheoNam();
                ViewData["LuongNVTheoThang"] = base.ThongKeLuongNhanVienTheoThang(DateTime.Now.Year);
                ViewData["DoanhThuTheoThangCuaNam"] = base.ThongKeDoanhThuTheoThang(DateTime.Now.Year);
                ViewData["ThongKeDichVu"] = base.ThongKeLuongTieuThuTheoLoaiDichVu(DateTime.Now.Year);
                ViewData["GetYear"] = TempData["ChonNam"] != null ? (TempData["ChonNam"]) : DateTime.Now.Year;
                TempData["ChonNam"] = TempData["ChonNam"] != null ? (int)TempData["ChonNam"] : DateTime.Now.Year;
                return View();
            // }
            // return base.ChuyenHuong();
        }

        [HttpPost]
        public IActionResult Index(int chooseYear, DateTime? ngayBatDau, DateTime? ngayKetThuc)
        {
            ViewData["ChooseYear"] = new SelectList(base.ListNam(), "value", "text", chooseYear);
            ViewData["KhachHang"] = base.ThongKeKhachHang(chooseYear, ngayBatDau == null ? null : ngayBatDau.Value, ngayKetThuc == null ? null : ngayKetThuc.Value);
            ViewData["LoaiPhong"] = base.ThongKeLoaiPhong(chooseYear, ngayBatDau == null ? null : ngayBatDau.Value, ngayKetThuc == null ? null : ngayKetThuc.Value);
            ViewData["TongDoanhThu"] = base.ThongKeTongDoanhThu(chooseYear, ngayBatDau == null ? null : ngayBatDau.Value, ngayKetThuc == null ? null : ngayKetThuc.Value);
            ViewData["TongPhongSuDung"] = base.ThongKeTongPhongSuDung(chooseYear, ngayBatDau == null ? null : ngayBatDau.Value, ngayKetThuc == null ? null : ngayKetThuc.Value);
            ViewData["TongNgayDatDoanhThu"] = base.ThongKeTongNgayDatDoanhThu(chooseYear, ngayBatDau == null ? null : ngayBatDau.Value, ngayKetThuc == null ? null : ngayKetThuc.Value);
            ViewData["ListTopKHTieuBieu"] = base.listKhachHangTieuBieu(chooseYear, ngayBatDau == null ? null : ngayBatDau.Value, ngayKetThuc == null ? null : ngayKetThuc.Value);
            ViewData["ListNVTieuBieu"] = base.listNhanVienTieuBieu(chooseYear != -1 ? chooseYear : TempData["ChonNam"] != null ? (int)TempData["ChonNam"] : DateTime.Now.Year);
            ViewData["DoanhThuTheoNam"] = base.ThongKeDoanhThuTheoNam();
            ViewData["LuongNVTheoNam"] = base.ThongKeLuongNhanVienTheoNam();
            ViewData["LuongNVTheoThang"] = base.ThongKeLuongNhanVienTheoThang(chooseYear != -1 ? chooseYear : (int)TempData["ChonNam"]);
            ViewData["DoanhThuTheoThangCuaNam"] = base.ThongKeDoanhThuTheoThang(chooseYear != -1 ? chooseYear : (int)TempData["ChonNam"]);
            ViewData["ThongKeDichVu"] = base.ThongKeLuongTieuThuTheoLoaiDichVu(chooseYear != -1 ? chooseYear : (int)TempData["ChonNam"]);

            ViewData["ngayBatDauData"] = ngayBatDau == null ? null : ngayBatDau.Value;
            ViewData["ngayKetThucData"] = ngayKetThuc == null ? null : ngayKetThuc.Value;
            var a = (int)TempData["ChonNam"];
            ViewData["GetYear"] = chooseYear != -1 ? chooseYear : a;
            TempData["ChonNam"] = chooseYear != -1 ? chooseYear : a;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}