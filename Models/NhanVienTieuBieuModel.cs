using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QLKSMVC.Models
{
    public class NhanVienTieuBieuModel
    {
        public int MaNv{get;set;}
        public string HoTen{get;set;}
        public string ChucVu{get;set;}
        public string Email{get;set;}
        public string HinhAnh{get;set;}
        public int SoNgayLamViec {get;set;}
        public int SLThanhToan{get;set;}
        public decimal TongLuong{get;set;}
        public decimal TongTien {get;set;}
        public bool TrangThai {get;set;}
    }
}