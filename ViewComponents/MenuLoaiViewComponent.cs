using ECommerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly Hshop2023Context db;

        // BẢN ĐỒ ÁNH XẠ TÊN LOẠI VỚI ICON CLASS (Font Awesome 5, style fas/far/fab)
        private readonly Dictionary<string, string> IconMap = new Dictionary<string, string>
        {
            {"Đồng hồ", "fa-clock"},
            {"Laptop", "fa-laptop"},
            {"Máy ảnh", "fa-camera"},
            {"Điện thoại", "fa-mobile-alt"},
            {"Nước hoa", "fa-flask"}, // Hoặc fa-perfume nếu có
            {"Trang sức", "fa-gem"},
            {"Giày", "fa-shoe-prints"},
            {"Vali", "fa-suitcase"}
        };

        public MenuLoaiViewComponent(Hshop2023Context context) => db = context;

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai = lo.MaLoai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count(),
                // THÊM THUỘC TÍNH ICON:
                IconClass = IconMap.ContainsKey(lo.TenLoai) ? IconMap[lo.TenLoai] : "fa-tag" // Mặc định là fa-tag nếu không tìm thấy
            }).ToList().OrderBy(p => p.TenLoai);
            // Thêm ToList() để thực thi truy vấn

            return View(data);
        }
    }
}