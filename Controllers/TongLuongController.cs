using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLKSMVC.Data;
using QLKSMVC.Models;

namespace QLKSMVC.Controllers
{
    public class TongLuongController : Controller
    {
        private readonly QuanLyKhachSanDbContext _context;

        public TongLuongController(QuanLyKhachSanDbContext context)
        {
            _context = context;
        }

        // GET: TongLuong
        public async Task<IActionResult> Index()
        {
            var quanLyKhachSanDbContext = await _context.TongLuongs.Include(t => t.MaNvNavigation).Include(tl => tl.MaNvNavigation.MaCvNavigation).Include(tl => tl.MaNvNavigation.ChamCongNhanViens)
                                                                // .Where(tl => tl.NgayNhanLuong.Value.Year == 2022 && tl.NgayNhanLuong.Value.Month == 12);
                                                                .OrderBy(tl => tl.NgayNhanLuong.Value).Take(_context.NhanViens.Count()).ToListAsync();
            return View(quanLyKhachSanDbContext);
        }

        [HttpPost]
        public async Task<IActionResult> Index(DateOnly? datepicker)
        {
            if (datepicker != null)
            {
                return View(await _context.TongLuongs.Include(tl => tl.MaNvNavigation).Include(tl => tl.MaNvNavigation.ChamCongNhanViens).Include(tl => tl.MaNvNavigation.MaCvNavigation)
                                                        .Where(tl => tl.NgayNhanLuong.Value.Year == datepicker.Value.Year && tl.NgayNhanLuong.Value.Month == datepicker.Value.Month).ToListAsync());
            }
            return View();
        }

        // GET: TongLuong/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TongLuongs == null)
            {
                return NotFound();
            }
            var tongLuongModel = await _context.TongLuongs
                .Include(t => t.MaNvNavigation)
                .Include(tl => tl.MaNvNavigation.MaCvNavigation)
                .Include(tl => tl.MaNvNavigation.ChamCongNhanViens)
                .FirstOrDefaultAsync(m => m.MaTl == id);
            if (tongLuongModel == null)
            {
                return NotFound();
            }
            return View(tongLuongModel);
        }
    }
}
