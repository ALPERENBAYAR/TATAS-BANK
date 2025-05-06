using Bankacilik.Models;
using BusinessLayer.Abstract;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repositories;
using EntityLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc;

namespace Bankacilik.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeManager _employeeManager;
        private readonly Context _context;
        
        public EmployeeController()
        {
            _employeeManager = new EmployeeManager();
            _context = new Context();
        }

        [HttpGet]
        public IActionResult AddCustomer()
        {

            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }
            var model = new CustomerViewModel()
            {
                customers = new List<CustomerViewModel>(),
                branches = _context.Branches.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomerPost(CustomerViewModel model)
        {
            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }
            try
            {
                await _employeeManager.AddCustomer(model.CustomerName, model.CustomerSurname, model.CustomerMail, model.CustomerPassword);
                ViewBag.SuccessMessage = "Müşteri başarılı bir şekilde eklendi.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            model.customers = _context.Customers.Select(c => new CustomerViewModel
            {
                CustomerId = c.Id,
                CustomerName = c.CustomerName,
                CustomerSurname = c.CustomerSurname,
                CustomerMail = c.CustomerMail,
                BranchName = c.Branch.BranchName,
            }).ToList();
            model.branches = _context.Branches.ToList();
            return View("AddCustomer", model);
        }

        [HttpGet]
        public IActionResult DeleteCustomer()
        {

            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }
            var model = new CustomerViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCustomerPost(CustomerViewModel model)
        {
            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }
            try
            {
               await _employeeManager.DeleteCustomer(model.CustomerMail);
                ViewBag.SuccessMessage = "Müşteri başarılı bir şekilde silindi";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            
            return View("DeleteCustomer" , new CustomerViewModel());
        }

        [HttpGet]
        public IActionResult Login()
        {
            
            return View(); 
        }

        [HttpPost]
        public IActionResult LoginPost(string employeeMail , string employeePassword) {

            var hashedPassword = PasswordHasher.HashPassword(employeePassword);
            var employee = _context.Employees.FirstOrDefault(m => m.EmployeeMail == employeeMail && m.EmployeePassword == hashedPassword);


            if (employee != null)
            {

                HttpContext.Session.SetString("employeeMail", employeeMail);
                HttpContext.Session.SetInt32("employeeId", employee.EmployeeId);


                return RedirectToAction("Dashboard2", "Home");
            }


            ViewBag.ErrorMessage = "Geçersiz e-posta veya şifre";


            return View("Login");

        }

        [HttpGet]
        public IActionResult TransferMoney()
        {
            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TransferMoneyPost(TransferViewModel model)
        {
            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }
            try
            {
                // Önce toplam kesilecek tutarı hesapla
                decimal totalDebit = model.Amount + model.TransactionFee;

                var fromAccount = _context.Accounts.FirstOrDefault(a => a.AccountNumber == model.FromAccount);
                if (fromAccount == null)
                {
                    ViewBag.ErrorMessage = "Gönderen hesap bulunamadı.";
                    return View("TransferMoney");
                }

                if (fromAccount.Balance < totalDebit)
                {
                    ViewBag.ErrorMessage = "Yetersiz bakiye. (Transfer + işlem ücreti)";
                    return View("TransferMoney");
                }

                // 1. Parayı alıcıya gönder
                await _employeeManager.TransferMoney(model.FromAccount, model.ToAccount, model.Amount);

                // 2. İşlem ücretini düş
                await _employeeManager.TransactionFee(model.TransactionFee, model.FromAccount);

                ViewBag.SuccessMessage = "Para transferi ve işlem ücreti başarılı.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View("TransferMoney");
        }
        
        [HttpGet]
        public IActionResult WithdrawMoney()
        {
            string employeeMail = HttpContext.Session.GetString("employeeMail");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> WithdrawMoneyPost(TransferViewModel model)
        {
            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }

            try
            {
                await _employeeManager.WithdrawMoney(model.FromAccount, model.Amount, model.TransactionFee);
                ViewBag.SuccessMessage = "Hesaptan para çekimi ve işlem ücreti başarılı.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View("WithdrawMoney");
        }

        [HttpGet]
        public IActionResult FastTransfer()
        {
            string employeeMail = HttpContext.Session.GetString("employeeMail");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> FastTransferPost(TransferViewModel model)
        {
            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }

            try
            {
                await _employeeManager.FastTransfer(model.FromAccount, model.Amount, model.TransactionFee);
                ViewBag.SuccessMessage = "FAST işlemi başarıyla gerçekleşti.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View("FastTransfer");
        }

        [HttpGet]
        public IActionResult Deposit()
        {
            string employeeMail = HttpContext.Session.GetString("employeeMail");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DepositPost(DepositViewModel model)
        {
            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }

            try
            {
                await _employeeManager.DepositMoney(model.ToAccount, model.Amount);
                ViewBag.SuccessMessage = "Para başarıyla yatırıldı.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }

            return View("Deposit");
        }

        [HttpGet]
        public IActionResult ListCustomer()
        {

            if (HttpContext.Session.GetString("employeeMail") == null)
            {
                return RedirectToAction("Login", "Employee");
            }
            var customers = _context.Customers.ToList();   // verileri belleğe al
            var branches = _context.Branches.ToList();     // verileri belleğe al

            var model = new CustomerViewModel
            {
                Customers = customers.Select(c => new CustomerViewModel
                {
                   CustomerName = c.CustomerName,
                   CustomerSurname = c.CustomerSurname,
                   CustomerMail = c.CustomerMail,
                    BranchName = branches.FirstOrDefault(b => b.BranchId == c.BranchId)?.BranchName ?? "Tanımsız"
                }).ToList()
            };
            return View(model);

        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Employee");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            string employeeMail = HttpContext.Session.GetString("employeeMail");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            string employeeMail = HttpContext.Session.GetString("employeeMail");

            if (string.IsNullOrEmpty(employeeMail))
            {
                return RedirectToAction("Login"); // Giriş yapılmamışsa Login'e yönlendir
            }
            var newHashedPassword = PasswordHasher.HashPassword(newPassword);
            var result = await _employeeManager.ChangePassword(employeeMail, oldPassword, newHashedPassword);

            if (result)
            {
                TempData["Success"] = "Şifreniz başarıyla güncellendi.";
            }
            else
            {
                TempData["Error"] = "Mevcut şifreniz hatalı!";
            }

            return View();
        }


    }  
}
