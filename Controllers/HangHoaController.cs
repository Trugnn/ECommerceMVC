using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

namespace ECommerceMVC.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly Hshop2023Context db;

        public HangHoaController(Hshop2023Context context)
        {
            db = context;
        }
        public IActionResult Index(int? loai, int page = 1)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            string tenLoaiHienThi = "Tất cả sản phẩm"; // Tên khởi tạo ban đầu khi chưa ấn danh mục sản phẩm
            if (loai.HasValue)
            {
                hangHoas = hangHoas.Where(p => p.MaLoai == loai.Value);
                var loaiSp = db.Loais.FirstOrDefault(l => l.MaLoai == loai.Value);
                if (loaiSp != null)
                {
                    tenLoaiHienThi = loaiSp.TenLoai;
                }
            }
            //Truyen viewdata
            ViewData["TenLoaiSp"] = tenLoaiHienThi;

            // Paging 
            int NoOfRecordPerPage = 9;
            int NoOfPages = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(hangHoas.Count()) / NoOfRecordPerPage));
            int NoOfRecordToSkip = (page - 1) * NoOfRecordPerPage;
            ViewBag.Page = page;
            ViewBag.NoOfPages = NoOfPages;

            var result = hangHoas
                .Skip(NoOfRecordToSkip)
                .Take(NoOfRecordPerPage)
                .Select(p => new HangHoaVM
                {
                    MaHh = p.MaHh,
                    TenHh = p.TenHh,
                    Hinh = p.Hinh ?? "",
                    DonGia = p.DonGia ?? 0,
                    MoTaNgan = p.MoTaDonVi ?? "",
                    TenLoai = p.MaLoaiNavigation.TenLoai ?? ""
                }).ToList();

            return View(result);
        }

        [HttpGet]
        public IActionResult Search(string query)
        {
            var hangHoas = db.HangHoas.AsQueryable();
            if(query != null)
            {
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHh = p.TenHh,
                TenLoai = p.MaLoaiNavigation.TenLoai,
                Hinh = p.Hinh ?? "",
                DonGia = p.DonGia ?? 0,
                MoTaNgan = p.MoTaDonVi ?? ""
            }).ToList();

            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
                .Include(p => p.MaLoaiNavigation)
                .SingleOrDefault(p => p.MaHh == id);
            if(data == null)
            {
                TempData["Message"] = $"Không thấy sản phầm có mã {id}" ;
                return Redirect("/404");
            }

            var result = new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHh = data.TenHh,
                DonGia = data.DonGia ?? 0,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTaDonVi ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoLuongTon = 10,
                DiemDanhGia = 5
            };

            return View(result);
        }
    }
}
