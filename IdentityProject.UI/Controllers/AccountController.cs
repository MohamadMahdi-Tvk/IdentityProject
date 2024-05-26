using IdentityProject.UI.Models.Dto;
using IdentityProject.UI.Models.Dto.Account;
using IdentityProject.UI.Models.Entities;
using IdentityProject.UI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IdentityProject.UI.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly EmailService _emailService;
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = new EmailService();
        }


        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;
            MyAccountInfoDto myAccount = new MyAccountInfoDto()
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                UserName = user.UserName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled
            };
            return View(myAccount);
        }

        [Authorize]
        [HttpGet]
        public IActionResult TwoFactorEnabled()
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            var result = _userManager.SetTwoFactorEnabledAsync(user, !user.TwoFactorEnabled).Result;

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Register(RegisterDto register)
        {
            if (ModelState.IsValid == false)
            {
                return View(register);
            }

            User newUser = new User()
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                Email = register.Email,
                UserName = register.Email,
            };

            //Username => mtvk@gmail.com password => Mhtv123@
            var result = _userManager.CreateAsync(newUser, register.Password).Result;

            if (result.Succeeded)
            {
                var token = _userManager.GenerateEmailConfirmationTokenAsync(newUser).Result;

                var callBackUrl = Url.Action("EmailConfirm", "Account", new
                {
                    userId = newUser.Id,
                    token = token
                }, protocol: Request.Scheme);


                string body = $"لطفا برای فعالسازی حساب کاربری خود روی لینک زیر کلیک کنید <br/> <a href={callBackUrl}>لینک فعالسازی</a>";

                _emailService.Execute(newUser.Email, body, "فعالسازی حساب کاربری");


                return RedirectToAction("DisplayEmail");
            }

            string message = "";

            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;
            }

            TempData["message"] = message;

            return View(register);
        }

        [HttpGet]
        public IActionResult DisplayEmail()
        {

            return View();
        }

        [HttpGet]
        public IActionResult EmailConfirm(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return View("Error");
            }
            var user = _userManager.FindByIdAsync(userId).Result;

            var result = _userManager.ConfirmEmailAsync(user, token).Result;

            if (result.Succeeded)
            {
                // اگر موفقیت آمیز بود، چه فرآیندی طی شود
            }
            else
            {

            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = "/")
        {
            return View(new LoginDto
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public IActionResult Login(LoginDto login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = _signInManager.UserManager.FindByNameAsync(login.Username).Result;

            if (user == null)
            {
                return NotFound();
            }
            _signInManager.SignOutAsync();

            var result = _signInManager.PasswordSignInAsync(user, login.Password, login.IsPersistent, true).Result;

            if (result.Succeeded)
            {
                return Redirect(login.ReturnUrl);
            }
            if (result.RequiresTwoFactor == true)
            {
                return RedirectToAction("TwoFactorLogin", new { login.Username, login.IsPersistent });
            }
            if (result.IsLockedOut)
            {
                //زمان قفل شدن حساب کاربری بدلیل اینکه کاربر زیادی تلاش برای ورود داشته است
            }

            ModelState.AddModelError(string.Empty, "Login Error");

            return View();
        }

        [HttpGet]
        public IActionResult TwoFactorLogin(string UserName, bool IsPersistent)
        {
            var user = _userManager.FindByNameAsync(UserName).Result;

            if (user == null)
            {
                return BadRequest();
            }

            var providers = _userManager.GetValidTwoFactorProvidersAsync(user).Result;

            TwoFactorLoginDto model = new TwoFactorLoginDto();

            if (providers.Contains("Email"))
            {
                string emailCode = _userManager.GenerateTwoFactorTokenAsync(user, "Email").Result;
                EmailService emailService = new EmailService();
                emailService.Execute(user.Email, $"Two Factor Code Is: {emailCode}", "کد ورود دو مرحله ای شما");

                model.Provider = "Email";
                model.IsPersistent = IsPersistent;
            }

            else if (providers.Contains("Phone"))
            {
                string smsCode = _userManager.GenerateTwoFactorTokenAsync(user, "Phone").Result;

                SmsService smsService = new SmsService();
                smsService.Send(user.PhoneNumber, smsCode);

                model.Provider = "Phone";
                model.IsPersistent = IsPersistent;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult TwoFactorLogin(TwoFactorLoginDto twoFactor)
        {
            if (!ModelState.IsValid)
            {
                return View(twoFactor);
            }

            var user = _signInManager.GetTwoFactorAuthenticationUserAsync().Result;

            if (user == null)
            {
                return BadRequest();
            }

            var result = _signInManager.TwoFactorSignInAsync(twoFactor.Provider, twoFactor.Code, twoFactor.IsPersistent, false).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "حساب کاربری شما قفل شده است");
                return View();
            }
            else
            {
                ModelState.AddModelError("", "کد وارد شده صحیح نمی باشد");
                return View();
            }

        }


        [HttpGet]
        public IActionResult LogOut()
        {
            _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordDto forgot)
        {
            if (!ModelState.IsValid)
            {
                return View(forgot);
            }

            var user = _userManager.FindByEmailAsync(forgot.Email).Result;

            if (user == null || _userManager.IsEmailConfirmedAsync(user).Result == false)
            {
                ViewBag.meesage = "ممکن است ایمیل وارد شده معتبر نباشد! و یا اینکه ایمیل خود را تایید نکرده باشید";
                return View();
            }

            var token = _userManager.GeneratePasswordResetTokenAsync(user).Result;

            var callBackUrl = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = token
            }, protocol: Request.Scheme);

            string body = $"برای تنظیم مجدد کلمه عبور بر روی لینک زیر کلیک کنید <br/> <a href={callBackUrl}> link reset Password </a>";

            _emailService.Execute(user.Email, body, "فراموشی رمز عبور");

            ViewBag.Message = "لینک تنظیم مجدد کلمه عبور برای ایمیل شما ارسال شد";

            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordDto
            {
                UserId = userId,
                Token = token
            });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordDto reset)
        {
            if (!ModelState.IsValid)
            {
                return View(reset);
            }
            if (reset.Password != reset.ConfirmPassword)
            {
                return BadRequest();
            }

            var user = _userManager.FindByIdAsync(reset.UserId).Result;

            if (user == null)
            {
                return BadRequest();
            }

            var result = _userManager.ResetPasswordAsync(user, reset.Token, reset.Password).Result;

            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordSuccessfull));
            }
            else
            {
                ViewBag.Errors = result.Errors;
                return View(result);
            }

        }

        [HttpGet]
        public IActionResult ResetPasswordSuccessfull()
        {
            return View();
        }


        [Authorize]
        [HttpGet]
        public IActionResult SetPhoneNumber()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult SetPhoneNumber(SetPhoneNumberDto phoneNumberDto)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            var setResult = _userManager.SetPhoneNumberAsync(user, phoneNumberDto.PhoneNumber).Result;

            string code = _userManager.GenerateChangePhoneNumberTokenAsync(user, phoneNumberDto.PhoneNumber).Result;

            SmsService smsService = new SmsService();
            smsService.Send(phoneNumberDto.PhoneNumber, code);

            TempData["PhoneNumber"] = phoneNumberDto.PhoneNumber;

            return RedirectToAction(nameof(VerifyPhoneNumber));
        }


        [Authorize]
        [HttpGet]
        public IActionResult VerifyPhoneNumber()
        {
            return View(new VerifyPhoneNumberDto
            {
                PhoneNumber = TempData["PhoneNumber"].ToString()
            });
        }

        [Authorize]
        [HttpPost]
        public IActionResult VerifyPhoneNumber(VerifyPhoneNumberDto verify)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            bool resultVerify = _userManager.VerifyChangePhoneNumberTokenAsync(user, verify.Code, verify.PhoneNumber).Result;

            if (resultVerify == false)
            {
                ViewData["Message"] = $"کد وارد شده برای {verify.PhoneNumber} اشتباه است";
                return View(verify);
            }
            else
            {
                user.PhoneNumberConfirmed = true;
                var resultUpdate = _userManager.UpdateAsync(user).Result;
            }

            return RedirectToAction("VerifySuccess");
        }

        [HttpGet]
        public IActionResult VerifySuccess()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult ExternalLogin(string ReturnUrl)
        {
            string url = Url.Action(nameof(CallBack), "Account", new
            {
                ReturnUrl
            });

            var propertis = _signInManager
                .ConfigureExternalAuthenticationProperties("Google", url);

            return new ChallengeResult("Google", propertis);
        }

        public IActionResult CallBack(string ReturnUrl)
        {
            var loginInfo = _signInManager.GetExternalLoginInfoAsync().Result;

            string email = loginInfo.Principal.FindFirst(ClaimTypes.Email)?.Value ?? null;
            if (email == null)
            {
                return BadRequest();
            }
            string FirstName = loginInfo.Principal.FindFirst(ClaimTypes.GivenName)?.Value ?? null;
            string LastName = loginInfo.Principal.FindFirst(ClaimTypes.Surname)?.Value ?? null;

            var signin = _signInManager.ExternalLoginSignInAsync("Google", loginInfo.ProviderKey
                , false, true).Result;
            if (signin.Succeeded)
            {
                if (Url.IsLocalUrl(ReturnUrl))
                {
                    return Redirect("~/");

                }
                return RedirectToAction("Index", "Home");
            }
            else if (signin.RequiresTwoFactor)
            {
                //
            }

            var user = _userManager.FindByEmailAsync(email).Result;
            if (user == null)
            {
                User newUser = new User()
                {
                    UserName = email,
                    Email = email,
                    FirstName = FirstName,
                    LastName = LastName,
                    EmailConfirmed = true,
                };
                var resultAdduser = _userManager.CreateAsync(newUser).Result;
                user = newUser;
            }
            var resultAddlogin = _userManager.AddLoginAsync(user, loginInfo).Result;
            _signInManager.SignInAsync(user, false).Wait();


            return Redirect("/");
        }

    }
}
