using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLKSMVC.Base;
using QLKSMVC.Data;
using QLKSMVC.Models;

namespace QLKSMVC.Controllers
{
    public class AccountController : BaseController
    {

        private IWebHostEnvironment _env;
        public AccountController(QuanLyKhachSanDbContext context, IWebHostEnvironment env) : base(context)
        {
            _env = env;
        }


        // GET: Account
        public async Task<IActionResult> Index(int? phanQuyenErrorMessage)
        {
            if (base.KiemTraPhanQuyen("Admin"))
            {
                if (phanQuyenErrorMessage != null)
                {
                    ViewData["phanQuyenErrorMessage"] = "Yêu cầu truy cập bị từ chối!";
                }
                if (_context.Accounts != null)
                {
                    ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
                    var listAccount = await _context.Accounts.Include(a => a.RoleIdNavigation).ToListAsync();
                    ViewData["ListAccount"] = listAccount;
                    return View();
                }
                return View();
            }
            return base.ChuyenHuong();
        }


        [HttpPost]
        public JsonResult Create(AccountModel accModel)
        {
            List<string> listError = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(accModel);
                    _context.SaveChanges();
                    return Json(listError);
                }
                catch (SqlException ex)
                {
                    listError.Add(ex.Message);
                }
            }
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                    listError.Add(error.ErrorMessage);
                }
            }
            return Json(listError);
        }


        [HttpPost]
        public JsonResult GetDataEdit(int? id)
        {
            if (id != null)
            {
                var accountModel = _context.Accounts.Where(acc => acc.Id == id.Value).FirstOrDefault();
                if (accountModel != null)
                {
                    return new JsonResult(accountModel);
                }
                return new JsonResult(-1);
            }
            return new JsonResult(-1);
        }

        [HttpPost]
        public JsonResult Update(AccountModel accModel)
        {
            List<string> listError = new List<string>();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(accModel);
                    _context.SaveChanges();
                }
                catch (SqlException ex)
                {
                    listError.Add(ex.Message);
                }
            }
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                    listError.Add(error.ErrorMessage);
                }
            }
            return Json(listError);
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var accountModel = await _context.Accounts
                .Include(a => a.RoleIdNavigation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (accountModel == null)
            {
                return NotFound();
            }

            return View(accountModel);
        }

        [HttpGet]
        public IActionResult DangNhap(int? dangNhapErrorMessage)
        {
            if (HttpContext.Request.Cookies["DangNhap"] != null)
            {
                return base.ChuyenHuong();
            }
            if (dangNhapErrorMessage != null)
            {
                ViewData["dangNhapErrorMessage"] = "Bạn cần phải đăng nhập trước!";
            }
            if (TempData["DoiMatKhauSuccess"] != null)
            {
                ViewData["DoiMatKhauSuccess"] = TempData["DoiMatKhauSuccess"].ToString();
            }
            ViewData["ThongBaoLoi"] = null;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> DangNhap(AccountModel accModel)
        {
            if (ModelState.IsValid && _context.Accounts != null && _context.NhanViens != null && _context != null)
            {
                List<string> listError = new List<string>();
                listError.Clear();
                var check = await _context.Accounts.Where(tk => tk.UserName == accModel.UserName && tk.UserPassword == accModel.UserPassword)
                                                    .FirstOrDefaultAsync();
                if (check != null)
                {
                    if (check.TimeLockOut != null && DateTime.Now.CompareTo(check.TimeLockOut) <= 0)
                    {
                        listError.Add($"Tài khoản của bạn sẽ được mở lại sau ({check.TimeLockOut})");
                        ViewData["ThongBaoLoi"] = listError;
                        return View(accModel);
                    }
                    else
                    {
                        if (check.RoleId == -1) // TH: tài khoản bị khóa
                        {
                            listError.Add("Tài khoản của bạn đã bị khóa vui lòng liên hệ admin để được hỗ trợ!");
                            ViewData["ThongBaoLoi"] = listError;
                            return View(accModel);
                        }
                        else
                        {
                            string cookieValue = check.UserName + ";" + check.RoleId;
                            BaseClass bs = new BaseClass();
                            cookieValue = bs.hashString(cookieValue);
                            HttpContext.Response.Cookies.Append(
                                "DangNhap",
                                cookieValue,
                                new CookieOptions { Expires = DateTime.Now.AddMonths(1), Path = "/" }
                            );

                            HttpContext.Response.Cookies.Append(
                                "TokenLogOut",
                                cookieValue,
                                new CookieOptions { Expires = DateTime.Now.AddMonths(1), Path = "/" }
                            );

                            if (Request.Cookies["DangNhap"] == null)
                            {
                                listError.Add($"Cookie is null. Vui lòng thử với trình duyệt khác!");
                                ViewData["ThongBaoLoi"] = listError;
                                return View(accModel);
                            }
                            else
                            {
                                check.CountLoginFailed = 0;
                                check.TimeLockOut = null;
                                _context.Update(check);
                                try
                                {
                                    await _context.SaveChangesAsync();
                                }
                                catch (SqlException ex)
                                {
                                    listError.Add($"Lỗi : {ex.Message}");
                                }
                                if (check.RoleId == 99) // tài khoản admin
                                {
                                    return RedirectToAction("Index", "Account");
                                }
                                else if (check.RoleId == 0) // tài khoản nhân viên
                                {
                                    if (_context.NhanViens.Where(nv => nv.UserName == check.UserName).FirstOrDefault() == null)
                                    {
                                        TempData["ThongBaoTaiKhoan"] = "error";
                                        return RedirectToAction(nameof(EditProfile));
                                    }
                                    // redirect về trang đặt phòng
                                    return RedirectToAction("Index", "DatPhong");
                                }
                                else if (check.RoleId == 1) // tài khoản quản lý
                                {
                                    if (_context.NhanViens.Where(nv => nv.UserName == check.UserName).FirstOrDefault() == null)
                                    {
                                        TempData["ThongBaoTaiKhoan"] = "error";
                                        return RedirectToAction(nameof(EditProfile));
                                    }
                                    // redirect về trang quản lý nhân viên
                                    return RedirectToAction("Index", "NhanVien");
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (_context.Accounts != null)
                    {
                        var test = await _context.Accounts.Where(tk => tk.UserName == accModel.UserName).FirstOrDefaultAsync();
                        if (test != null)
                        {
                            if (test.CountLoginFailed == null)
                            {
                                test.CountLoginFailed = 1;
                            }
                            else
                            {
                                test.CountLoginFailed += 1;
                            }
                            _context.Update(test);
                            try
                            {
                                await _context.SaveChangesAsync();
                            }
                            catch (SqlException ex)
                            {
                                listError.Add($"Đã xảy lỗi truy vấn! {ex.Message}");
                            }
                            if (test.CountLoginFailed == 5)
                            {
                                // tiến hành khóa tài khoản trong 30 phút
                                test.TimeLockOut = DateTime.Now.AddMinutes(30);
                                test.CountLoginFailed = 0;
                                _context.Update(test);
                                try
                                {
                                    await _context.SaveChangesAsync();
                                }
                                catch (Exception ex)
                                {
                                    listError.Add($"Đã xảy lỗi truy vấn! {ex.Message}");
                                }
                                listError.Add($"Tài khoản của bạn bị giới hạn đăng nhập đến {test.TimeLockOut.ToString()}");
                                ViewData["ThongBaoLoi"] = listError;
                                return View(accModel);
                            }
                            listError.Add($"Mật khẩu không đúng!");
                            ViewData["ThongBaoLoi"] = listError;
                            return View(accModel);
                        }
                        listError.Add($"Sai tên đăng nhập hoặc mật khẩu!");
                        ViewData["ThongBaoLoi"] = listError;
                        return View(accModel);
                    }
                }
            }
            return View();
        }


        public IActionResult DangXuat()
        {
            if (HttpContext.Request.Cookies["DangNhap"] != null)
            {
                // HttpContext.Response.Cookies.Append("DangNhap", "", new CookieOptions{
                //     Expires = DateTime.Now.AddDays(-1)
                // });
                foreach (var cookie in HttpContext.Request.Cookies)
                {
                    if (cookie.Key.Equals("DangNhap"))
                    {
                        Response.Cookies.Delete(cookie.Key);
                    }
                }
                return RedirectToAction(nameof(DangNhap));
            }
            return base.ChuyenHuong();
        }

        public IActionResult EditProfile()
        {
            if (TempData["ThongBaoTaiKhoan"] != null)
            {
                ViewData["ThongBaoLoi"] = "Bạn phải cập nhật profile để sử dụng dịch vụ!";
            }
            if (base.KiemTraPhanQuyen("NhanVien") || base.KiemTraPhanQuyen("QuanLy"))
            {
                BaseClass bs = new BaseClass();
                var tenDN = bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[0];
                ViewBag.tenDN = tenDN;
                if (_context.NhanViens != null)
                {
                    if (_context.NhanViens.Where(nv => nv.UserName == tenDN).FirstOrDefault() != null)
                    {
                        var nhanVien = _context.NhanViens.Where(nv => nv.UserName == tenDN).FirstOrDefault();
                        if (nhanVien != null)
                            ViewBag.maNV = nhanVien.MaNv;
                        return View(nhanVien);
                    }
                }
                return View();
            }
            return base.ChuyenHuong();
        }


        [HttpPost]
        public async Task<IActionResult> EditProfile(NhanVienModel nvModel)
        {
            if (ModelState.IsValid)
            {
                if (_context.NhanViens != null)
                {
                    BaseClass bs = new BaseClass();
                    var tenDN = bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[0];
                    ViewBag.tenDN = tenDN;
                    ViewBag.MaCv = Convert.ToInt32(bs.deCodeHash(HttpContext.Request.Cookies["DangNhap"]).Split(";")[1]);
                    if (nvModel.FileUpload != null)
                    {
                        var filePath = Path.Combine(_env.WebRootPath, "Images/NhanVien", nvModel.FileUpload.FileName);
                        using var fileStream = new FileStream(filePath, FileMode.Create);
                        //lưu dữ liệu fileUpload và stream
                        nvModel.FileUpload.CopyTo(fileStream);
                        nvModel.HinhAnh = nvModel.FileUpload.FileName;
                    }
                    if (await _context.NhanViens.Where(nv => nv.UserName == tenDN).FirstOrDefaultAsync() != null)
                    {
                        var nhanVien = _context.NhanViens.Where(nv => nv.UserName == tenDN).FirstOrDefault();
                        if (nhanVien != null)
                        {
                            nhanVien.Cccd = nvModel.Cccd;
                            nhanVien.DiaChi = nvModel.DiaChi;
                            nhanVien.HinhAnh = nvModel.HinhAnh;

                            nhanVien.Email = nvModel.Email;
                            nhanVien.SoDt = nvModel.SoDt;
                            nhanVien.HoTen = nvModel.HoTen;
                            _context.Update(nhanVien);
                            try
                            {
                                await _context.SaveChangesAsync();
                            }
                            catch
                            {
                                ViewData["ThongBaoLoi"] = "Cập nhật thông tin thất bại!";
                                ViewBag.MaCv = nvModel.MaCv;
                                ViewBag.tenDN = nvModel.UserName;
                                ViewBag.MaNv = nvModel.MaNv;
                                ViewBag.Duplicate = "CMND/CCCD đã được sử dụng!";
                                return View(nvModel);
                            }
                            ViewData["ThongBaoThanhCong"] = "Cập nhật thông tin thành công!";
                            ViewBag.MaCv = nvModel.MaCv;
                            ViewBag.tenDN = nvModel.UserName;
                            ViewBag.MaNv = nvModel.MaNv;
                            return View(nvModel);
                        }
                    }
                    else
                    {
                        _context.Add(nvModel);
                        try
                        {
                            await _context.SaveChangesAsync();
                            ViewBag.MaCv = nvModel.MaCv;
                            ViewBag.MaNv = nvModel.MaNv;
                            ViewBag.tenDN = nvModel.UserName;
                            ViewData["ThongBaoThanhCong"] = "Thêm mới thông tin thành công!";
                        }
                        catch
                        {
                            ViewBag.MaCv = nvModel.MaCv;
                            ViewBag.MaNv = nvModel.MaNv;
                            ViewBag.tenDN = nvModel.UserName;
                            ViewBag.Duplicate = "CMND/CCCD đã được sử dụng!";
                            ViewData["ThongBaoLoi"] = "Cập nhật thông tin thất bại!";
                        }
                        return View(nvModel);
                    }
                }
            }
            ViewBag.MaCv = nvModel.MaCv;
            ViewBag.MaNv = nvModel.MaNv;
            ViewBag.tenDN = nvModel.UserName;
            ViewData["ThongBaoLoi"] = "Dữ liệu không hợp lệ!";
            return View(nvModel);
        }

        public IActionResult QuenMatKhau()
        {
            if (!base.KiemTraPhanQuyen(null))
            {
                return View();
            }
            // _nofty.Information("Vui lòng đăng xuất trước để sử dụng dịch vụ!");
            return base.ChuyenHuong();
        }

        [HttpPost]
        public async Task<IActionResult> QuenMatKhau(string email)
        {
            if (_context.Accounts != null)
            {
                var tk = await _context.Accounts
                    .Where(tk => tk.Email == email)
                    .FirstOrDefaultAsync();
                string strCodeEmail = "";
                if (tk != null)
                {
                    TempData["PassModelResetPass"] = tk.UserName;
                    //tiến hành gửi email và và gửi mã code lên ViewData
                    BaseClass bs = new BaseClass();
                    strCodeEmail = bs.randEmailCode();
                    TempData["strCodeEmail"] = strCodeEmail;
                    if (bs.sendEmailGetCode(strCodeEmail, email) == true)
                    {
                        TempData["TokenQuenMatKhau"] = bs.randomToken();
                        TempData["SendMailSuccess"] = "Email đã được gửi thành công!";
                        // _nofty.Success("Email đã được gửi thành công!");
                        return RedirectToAction(nameof(XacThucMail));
                    }
                    else
                    {
                        ModelState.AddModelError("name", "Gửi email thất bại!");
                        return View();
                    }
                }
                else
                {
                    // show thong bao email khong ton tai
                    ModelState.AddModelError("", "Email không tồn tại!");
                    // _nofty.Error("Email không tồn tại!");
                    TempData["PassModelResetPass"] = null;
                }
            }
            return View();
        }

        public IActionResult XacThucMail()
        {
            if (TempData["SendMailSuccess"] != null)
            {
                ViewData["SendMailSuccess"] = TempData["SendMailSuccess"].ToString();
            }
            if (TempData["TokenQuenMatKhau"] == null)
            {
                // _nofty.Error("Token không hợp lệ!");
                return RedirectToAction(nameof(QuenMatKhau));
            }
            else
            {
                if (!base.KiemTraPhanQuyen(null))
                {
                    TempData["TokenQuenMatKhau"] = null;
                    return View();
                }
                return base.ChuyenHuong();
            }
        }

        [HttpPost]
        public IActionResult XacThucMail(string code)
        {
            BaseClass bs = new BaseClass();
            string? strCodeEmail = TempData["strCodeEmail"].ToString();
            if (TempData["strCodeEmail"] != null)
            {
                strCodeEmail = TempData["strCodeEmail"].ToString();
            }
            if (code.Equals(strCodeEmail))
            {

                // sang trang đổi mật khẩu
                TempData["TokenQuenMatKhau"] = bs.randomToken();
                return RedirectToAction(nameof(DoiMatKhau));
            }
            TempData["strCodeEmail"] = strCodeEmail;
            TempData["TokenQuenMatKhau"] = bs.randomToken();
            // _nofty.Error("Mã code không hợp lệ!");
            //thông báo lỗi
            return View();
        }

        public IActionResult DoiMatKhau()
        {
            if (TempData["TokenQuenMatKhau"] == null)
            {
                // _nofty.Error("Token không hợp lệ!");
                return RedirectToAction(nameof(QuenMatKhau));
            }
            if (!base.KiemTraPhanQuyen(null))
            {
                return View();
            }
            return base.ChuyenHuong();
        }

        [HttpPost]
        public async Task<IActionResult> DoiMatKhau(string pass, string repass)
        {
            if (pass == repass)
            {
                string? tenDN = "";
                if (TempData["PassModelResetPass"] != null)
                {
                    tenDN = TempData["PassModelResetPass"].ToString();
                    var tkResetPass = _context.Accounts
                        .Where(tkModel => tkModel.UserName == tenDN)
                        .FirstOrDefault();
                    if (tkResetPass != null)
                    {
                        tkResetPass.UserPassword = pass;
                        _context.Update(tkResetPass);
                        await _context.SaveChangesAsync();
                        // _nofty.Success("Đổi mật khẩu thành công!");
                        TempData["DoiMatKhauSuccess"] = "Đổi mật khẩu thành công!";
                        return RedirectToAction(nameof(DangNhap));
                        // Thong bao doi mat khau thanh cong!x`
                    }
                    else
                    {
                        ModelState.AddModelError("", "Đổi mật khẩu thất bại!");
                        // Thong bao doi mat khau that bai
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Mật khẩu không khớp!");
                // thong bao mat khau khong khop
            }
            return View();
        }
    }
}
