using Bankacilik.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using EntityLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bankacilik.Controllers
{
    public class CustomerController : Controller
    {
        private readonly CustomerManager _customerManager;
        private readonly AccountManager _accountManager;
        private readonly Context _context;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ILogger<CustomerController> logger)
        {
            _customerManager = new CustomerManager();
            _accountManager = new AccountManager();
            _context = new Context();
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            _logger.LogInformation("Login sayfası açıldı.");
            return View();
        }

        [HttpPost]
        public IActionResult LoginPost(string customerMail, string customerPassword)
        {
            var hashedPassword = PasswordHasher.HashPassword(customerPassword);
            var customer = _context.Customers.FirstOrDefault(m => m.CustomerMail == customerMail && m.CustomerPassword == hashedPassword);

            if (customer != null)
            {
                HttpContext.Session.SetString("CustomerMail", customerMail);
                HttpContext.Session.SetInt32("CustomerId", customer.Id);
                _logger.LogInformation("Müşteri giriş yaptı: {CustomerMail}", customerMail);
                return RedirectToAction("Dashboard", "Home");
            }

            _logger.LogWarning("Geçersiz giriş denemesi: {CustomerMail}", customerMail);
            ViewBag.ErrorMessage = "Geçersiz e-posta veya şifre";
            return View("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            _logger.LogInformation("Müşteri çıkış yaptı: {CustomerMail}", HttpContext.Session.GetString("CustomerMail"));
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Customer");
        }

        [HttpGet]
        public IActionResult ListAccount()
        {
            if (HttpContext.Session.GetString("CustomerMail") == null)
            {
                _logger.LogWarning("Oturum açmadan ListAccount sayfasına erişim denemesi.");
                return RedirectToAction("Login", "Customer");
            }

            int? customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                _logger.LogWarning("CustomerId boş. Session süresi dolmuş olabilir.");
                return RedirectToAction("Login", "Customer");
            }

            var accounts = _context.Accounts.Where(a => a.CustomerId == customerId).ToList();
            var branches = _context.Branches.ToList();

            var model = new AccountViewModel
            {
                Accounts = accounts.Select(a => new AccountViewModel
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    BranchName = branches.FirstOrDefault(b => b.BranchId == a.BranchId)?.BranchName ?? "Tanımsız",
                    Balance = a.Balance
                }).ToList()
            };

            _logger.LogInformation("Müşteri hesaplarını listeledi: {CustomerId}", customerId);
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            string customerMail = HttpContext.Session.GetString("CustomerMail");

            if (string.IsNullOrEmpty(customerMail))
            {
                _logger.LogWarning("Şifre değiştirme girişimi ama session boş.");
                return RedirectToAction("Login");
            }

            var newHashedPassword = PasswordHasher.HashPassword(newPassword);
            var result = await _customerManager.ChangePassword(customerMail, oldPassword, newHashedPassword);

            if (result)
            {
                _logger.LogInformation("Müşteri şifresini başarıyla değiştirdi: {CustomerMail}", customerMail);
                TempData["Success"] = "Şifre başarıyla değiştirildi.";
            }
            else
            {
                _logger.LogWarning("Şifre değiştirme başarısız: {CustomerMail}", customerMail);
                TempData["Error"] = "Mevcut şifreniz hatalı.";
            }

            return View();
        }

        [HttpGet]
        public IActionResult TransferMoney() => View();

        [HttpPost]
        public async Task<IActionResult> TransferMoney(TransferViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var success = await _customerManager.TransferMoney(model.FromAccount, model.ToAccount, model.Amount);

                    if (success)
                    {
                        _logger.LogInformation("TransferMoney başarılı: {From} → {To}, {Amount} TL", model.FromAccount, model.ToAccount, model.Amount);
                        ViewBag.SuccessMessage = $"Havale işlemi başarıyla gerçekleşti, {model.Amount} TL gönderildi. %1 işlem ücreti kesildi.";
                    }
                    else
                    {
                        _logger.LogWarning("TransferMoney başarısız: {From} → {To}", model.FromAccount, model.ToAccount);
                        ViewBag.ErrorMessage = "Transfer sırasında hata oluştu!";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransferMoney sırasında beklenmeyen hata");
                ViewBag.ErrorMessage = ex.Message;
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult TransferEft() => View();

        [HttpPost]
        public async Task<IActionResult> TransferEft(TransferViewModel model)
        {
            try
            {
                await _customerManager.TransferEft(model.FromAccount, model.Amount);
                _logger.LogInformation("TransferEft başarılı: {From} → EFT ile {Amount} TL", model.FromAccount, model.Amount);
                ViewBag.SuccessMessage = $"EFT işlemi başarıyla gerçekleşti. {model.Amount} TL gönderildi. %1 işlem ücreti kesildi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TransferEft sırasında hata");
                ViewBag.ErrorMessage = $"Hata: {ex.Message}";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> CheckBalance(string AccountNumber)
        {
            if (string.IsNullOrEmpty(AccountNumber))
            {
                _logger.LogWarning("CheckBalance çağrıldı ama hesap numarası boş.");
                return BadRequest("Hesap numarası boş olamaz.");
            }

            try
            {
                var balance = await _accountManager.CheckBalance(AccountNumber);
                ViewBag.Balance = balance;
                ViewBag.AccountNumber = AccountNumber;
                _logger.LogInformation("Bakiyeye bakıldı: {AccountNumber} → {Balance} TL", AccountNumber, balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CheckBalance sırasında hata: {AccountNumber}", AccountNumber);
                ViewBag.Error = ex.Message;
            }

            return View();
        }

        [HttpGet]
        public IActionResult KrediIslemleri()
        {
            _logger.LogInformation("Kredi işlemleri sayfası açıldı.");
            return View();
        }

        [HttpGet]
        public IActionResult KrediBasvuru()
        {
            _logger.LogInformation("KrediBaşvuru sayfası açıldı.");
            return View();
        }

        [HttpPost]
        public IActionResult KrediBasvuru(string accountNumber, decimal krediTutari)
        {
            if (string.IsNullOrEmpty(accountNumber) || krediTutari <= 0)
            {
                _logger.LogWarning("Geçersiz kredi başvurusu: {AccountNumber}, {KrediTutari}", accountNumber, krediTutari);
                ViewBag.ErrorMessage = "Hesap numarası ve kredi tutarı geçerli olmalıdır.";
                return View("KrediIslemleri");
            }

            var hesap = _context.Accounts.FirstOrDefault(a => a.AccountNumber == accountNumber);

            if (hesap == null)
            {
                _logger.LogWarning("Kredi başvurusu için hesap bulunamadı: {AccountNumber}", accountNumber);
                ViewBag.ErrorMessage = "Hesap bulunamadı.";
                return View("KrediIslemleri");
            }

            hesap.Balance += krediTutari;
            _context.Accounts.Update(hesap);
            _context.SaveChanges();

            _logger.LogInformation("Kredi tanımlandı: {AccountNumber} → +{KrediTutari} TL", accountNumber, krediTutari);
            ViewBag.SuccessMessage = $"Kredi başarıyla tanımlandı. {krediTutari} ₺ hesabınıza eklendi.";
            return View("KrediBasvuru");
        }

        [HttpPost]
        public IActionResult FaturaOdeme(string hesapNo, decimal tutar, string firma)
        {
            if (string.IsNullOrEmpty(hesapNo) || tutar <= 0 || string.IsNullOrEmpty(firma))
            {
                _logger.LogWarning("Geçersiz fatura ödeme isteği.");
                ViewBag.ErrorMessage = "Lütfen tüm alanları doğru şekilde doldurun.";
                return View("KrediIslemleri");
            }

            var hesap = _context.Accounts.FirstOrDefault(a => a.AccountNumber == hesapNo);

            if (hesap == null)
            {
                _logger.LogWarning("Fatura ödeme için hesap bulunamadı: {HesapNo}", hesapNo);
                ViewBag.ErrorMessage = "Hesap bulunamadı.";
                return View("KrediIslemleri");
            }

            if (hesap.Balance < tutar)
            {
                _logger.LogWarning("Yetersiz bakiye: {HesapNo} - Bakiye: {Bakiye}, Tutar: {Tutar}", hesapNo, hesap.Balance, tutar);
                ViewBag.ErrorMessage = "Yetersiz bakiye.";
                return View("KrediIslemleri");
            }

            hesap.Balance -= tutar;
            _context.Accounts.Update(hesap);
            _context.SaveChanges();

            _logger.LogInformation("Fatura ödendi: {HesapNo} - {Firma} - {Tutar}₺", hesapNo, firma, tutar);
            ViewBag.SuccessMessage = $"{firma} faturası için {tutar} ₺ ödeme başarıyla gerçekleştirildi.";

            return View("KrediIslemleri");
        }
    }
}
