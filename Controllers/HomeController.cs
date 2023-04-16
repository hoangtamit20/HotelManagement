using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLKSMVC.Base;
using QLKSMVC.Data;
using QLKSMVC.Models;

namespace QLKSMVC.Controllers;

public class HomeController : BaseController
{
    public HomeController(QuanLyKhachSanDbContext context) : base(context)
    {
    }

    public IActionResult Index()
    {
        // var a = base.ListNam();
        // int a = listKhachHangTieuBieu(2021, DateTime.Now.AddYears(-3), DateTime.Now);
        // Math.Ceiling((DateTime.Now.AddDays(2) - DateTime.Now.AddDays(-2)).TotalDays)
        // foreach(var item in _context.TongLuongs.ToList())
        // {
        //     if (item.NgayNhanLuong.Value.Year == 2019)
        //     {
        //         Random rd = new Random();
        //         item.UngLuong = (rd.Next(5,20))*100000;
        //         item.NgayUngLuong = item.NgayNhanLuong.Value.AddDays(rd.Next(-15,-5));
        //         _context.Update(item);
        //         _context.SaveChanges();
        //     }
        // }

        // foreach(var item in _context.TongLuongs.Include(tl => tl.MaNvNavigation.MaCvNavigation).Include(tl => tl.MaNvNavigation).ToList())
        // {
        //     if (item.NgayNhanLuong.Value.Year == 2019)
        //     {
        //         var luongCB = item.MaNvNavigation.MaCvNavigation.LuongCanBan;
        //         var luongUng = item.UngLuong.Value;
        //         var luongThuong = item.SoNgayLam.Value >= 26 ? 200000 : item.SoNgayLam.Value >= 24 ? 100000 : 0;
        //         item.TongTien = luongCB + luongThuong - luongUng;
        //         _context.Update(item);
        //         _context.SaveChanges();
        //     }
        // }
        // foreach(var item in base.listNhanVienTieuBieu(2021))
        // {
        //     System.Console.WriteLine($"{item.MaNv} - {item.HoTen} - {item.SoNgayLamViec} - {item.SLThanhToan} - {item.TongTien} - {item.TongLuong} - {item.TrangThai} - {item.ChucVu}");
        // }

        // foreach(var item in base.ThongKeLuongNhanVienTheoNam())
        // {
        //     System.Console.WriteLine($"{item.Nam} - {item.TongLuong}");
        // }


        // foreach(var item in base.ThongKeLuongTieuThuTheoLoaiDichVu(2019))
        // {
        //     System.Console.WriteLine($"{item.MaLdv} - {item.TenLdv} - {item.SoLuong}");
        // }
        return RedirectToAction("DangNhap", "Account");
    }

    public IActionResult Privacy()
    {
        var list = _context.HoaDons.Include(hd => hd.MaDpNavigation).ToList();
        foreach(var item in list)
        {
            // System.Console.WriteLine(item.MaDpNavigation.TienDatCoc.Value);
            // var a = _context.DatDichVus.Where(ddv => ddv.MaDp == item.MaDp).Sum(ddv => ddv.TongTien.Value);
            // item.TongTienDichVu = _context.DatDichVus.Where(ddv => ddv.MaDp == item.MaDp).Sum(ddv => ddv.TongTien.Value);
            item.TongTien = item.TongTienDichVu.Value + item.TongTienPhong.Value;
            item.TongTien = item.TongTien.Value - item.MaDpNavigation.TienDatCoc.Value;
            _context.Update(item);
            _context.SaveChanges();
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
