using ECommerceMVC.Data;
using Microsoft.AspNetCore.Mvc;
using ECommerceMVC.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ECommerceMVC.Helpers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceMVC.Controllers
{
    public class KhachHangController : Controller
    {
        private readonly Hshop2023Context db;
        private readonly IMapper _mapper;

        public KhachHangController(Hshop2023Context context, IMapper mapper) 
        {
            db = context;
            _mapper = mapper;
        }

        #region Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterVM());
        }

        [HttpPost]
        public IActionResult Register(RegisterVM model, IFormFile? Hinh)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // map viewmodel voi entity
                    var khachHang = _mapper.Map<KhachHang>(model);

                    // hash bpkdf2
                    khachHang.MatKhau = PasswordHasher.HashPassword(model.MatKhau);
                    khachHang.HieuLuc = true;      // Bật tài khoản
                    khachHang.VaiTro = 0;          // User bình thường

                    if (Hinh != null)
                    {
                        khachHang.Hinh = MyUtil.UploadHinh(Hinh, "KhachHang");
                    }

                    db.Add(khachHang);
                    db.SaveChanges();

                    return RedirectToAction("Index", "HangHoa");
                }
                catch
                {

                }
            }
            return View();
        }
        #endregion

        #region Login in
        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var khachHang = db.KhachHangs.SingleOrDefault(kh => kh.MaKh == model.UserName);
                if (khachHang == null)
                {
                    ModelState.AddModelError("loi", "Không có khách hàng này");
                }
                else
                {
                    if (!khachHang.HieuLuc)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã bị khóa. Vui lòng liên hệ Admin.");
                    }
                    else
                    {
                        // Check Mat Khau
                        bool valid = PasswordHasher.VerifyPassword(model.Password, khachHang.MatKhau);
                        if (!valid)
                        {
                            ModelState.AddModelError("loi", "Sai thông tin đăng nhập!");
                        }
                        else
                        {
                            // Ghi nhận
                            var claims = new List<Claim>
                            {
                                new Claim(ClaimTypes.Email, khachHang.Email),
                                new Claim(ClaimTypes.Name, khachHang.HoTen),
                                new Claim("CustomerId", khachHang.MaKh),

                                // Claim - role động
                                new Claim(ClaimTypes.Role, "Customer"),
                            };

                            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                            await HttpContext.SignInAsync(claimsPrincipal);

                            if (Url.IsLocalUrl(ReturnUrl))
                            {
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                return Redirect("/");
                            }
                        }
                    }       
                }
            }
            return View();
        }
        #endregion

        [Authorize]
        public IActionResult Profile()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
