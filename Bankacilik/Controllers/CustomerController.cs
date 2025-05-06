using Bankacilik.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;


namespace Bankacilik.Controllers
{

    public class CustomerController : Controller
    {
        private readonly CustomerManager _customerManager;
        private readonly AccountManager _accountManager;
        private readonly  Context _context;

        public CustomerController()
        {
            _customerManager = new CustomerManager();
            _accountManager = new AccountManager();
            _context = new Context();
        }
        [HttpGet]
        public IActionResult Login()
        {
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

                
                return RedirectToAction("Dashboard","Home"); 
            }

            
            ViewBag.ErrorMessage = "Geçersiz e-posta veya şifre";

           
            return View("Login"); 
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Customer");
        }

        [HttpGet]
        public IActionResult ListAccount()
        {

            if (HttpContext.Session.GetString("CustomerMail") == null)
            {
                return RedirectToAction("Login", "Customer");
            }
            var accounts = _context.Accounts.ToList();   // verileri belleğe al
            var branches = _context.Branches.ToList();     // verileri belleğe al

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
            return View(model);

        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            string customerMail = HttpContext.Session.GetString("CustomerMail");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            string customerMail = HttpContext.Session.GetString("CustomerMail");

            if (string.IsNullOrEmpty(customerMail))
            {
                return RedirectToAction("Login");
            }
            var newHashedPassword = PasswordHasher.HashPassword(newPassword);
            var result = await _customerManager.ChangePassword(customerMail, oldPassword, newHashedPassword);

            if (result)
            {
                TempData["Success"] = "Şifre başarıyla değiştirildi.";
            }
            else
            {
                TempData["Error"] = "Mevcut şifreniz hatalı.";
            }

            return View();
        }

        [HttpGet]
        public IActionResult TransferMoney()
        {
            string customerMail = HttpContext.Session.GetString("CustomerMail");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TransferMoney(TransferViewModel model)
        {
            string customerMail = HttpContext.Session.GetString("CustomerMail");
            try
            {
                if (ModelState.IsValid)
                {
                    var success = await _customerManager.TransferMoney(model.FromAccount, model.ToAccount, model.Amount);

                    if (success)
                    {
                        ViewBag.SuccessMessage = $"Havale işlemi başarıyla gerçekleşti,{model.Amount} TL gönderildi. %1 işlem ücreti kesildi.";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Transfer sırasında hata oluştu!";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult TransferEft()
        {
            string customerMail = HttpContext.Session.GetString("CustomerMail");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TransferEft(TransferViewModel model)
        {
            string customerMail = HttpContext.Session.GetString("CustomerMail");
            try
            {
                await _customerManager.TransferEft(model.FromAccount, model.Amount);
                ViewBag.SuccessMessage = $"EFT işlemi başarıyla gerçekleşti. {model.Amount} TL gönderildi. %1 işlem ücreti kesildi.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Hata: {ex.Message}";
            }

            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> CheckBalance(string AccountNumber)
        {
            string customerMail = HttpContext.Session.GetString("CustomerMail");
            if (AccountNumber == null)
            {
                return BadRequest("Hesap numarası boş olamaz.");
            }

            try
            {
                var balance = await _accountManager.CheckBalance(AccountNumber);
                ViewBag.Balance = balance;
                ViewBag.AccountNumber = AccountNumber;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }
    }
}

    

