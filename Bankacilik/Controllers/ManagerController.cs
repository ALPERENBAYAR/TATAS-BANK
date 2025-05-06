using Bankacilik.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using DataAccessLayer.Repositories;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Bankacilik.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ManagerManager _managerManager;
        private readonly Context _context;
        private readonly AccountRepository _accountRepository;
        private readonly CustomerRepository _customerRepository;
        private readonly BranchRepository _branchRepository;
        private readonly BankRepository _bankRepository;
        public ManagerController()
        {
            _managerManager = new ManagerManager();
            _context = new Context();
            _accountRepository = new AccountRepository();
            _customerRepository = new CustomerRepository();
            _branchRepository = new BranchRepository();
            _bankRepository = new BankRepository();
            

        }

        [HttpGet]
        public IActionResult Login()
        {
           
            return View();
        }

        [HttpPost]
        public IActionResult LoginPost(string managerMail, string managerPassword)
        {
            var hashedPassword = PasswordHasher.HashPassword(managerPassword);
            var manager = _context.Managers
                .FirstOrDefault(m =>
                    m.ManagerMail == managerMail &&
                    m.ManagerPassword == hashedPassword);

            if (manager != null)
            {
                // Küçük harfli anahtarlarla set ediyoruz
                HttpContext.Session.SetString("managerMail", managerMail);
                HttpContext.Session.SetInt32("managerId", manager.ManagerId);

                // Dashboard2'ye yönlendir
                return RedirectToAction("Dashboard3", "Home");
            }

            ViewBag.ErrorMessage = "Geçersiz e-posta veya şifre";
            return View("Login");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Manager");
        }

        [HttpGet]
        public IActionResult AddEmployeeGet()
        {
            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            var model = new EmployeeViewModel
            {
                Branches = _context.Branches.ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployeePost(EmployeeViewModel model)
        {

            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            try
            {
                await _managerManager.AddEmployee(model.EmployeeName, model.EmployeeSurname, model.EmployeeMail, model.EmployeePassword, model.BranchId);
                ViewBag.SuccessMessage = "Çalışan başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            model.Branches = _context.Branches.ToList();
            return View("AddEmployeeGet",new EmployeeViewModel());
        }

        [HttpGet]
        public IActionResult DeleteEmployeeGet()
        {
            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            var model = new EmployeeViewModel
            {
                Branches = _context.Branches.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult DeleteEmployeePost(EmployeeViewModel model)
        {
            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }

            try
            {
                _managerManager.DeleteEmployee(model.EmployeeMail);
                ViewBag.SuccessMessage = "Çalışan başarıyla silindi.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            model.Branches = _context.Branches.ToList();
            return View("DeleteEmployeeGet",new EmployeeViewModel());
        }

        [HttpGet]
        public IActionResult AddBranch()
        {
            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBranchPost(string branchName, string branchCity)
        {

            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            try
            {
                await _managerManager.AddBranch(branchName, branchCity);
                ViewBag.SuccessMessage = "Şube başarıyla eklendi";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            var branches = _context.Branches.ToList();
            ViewBag.Branches = branches;
            return View("AddBranch");
        }
        
        [HttpGet]
        public IActionResult ListEmployee()
        {

            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            var employees = _context.Employees.ToList();   // verileri belleğe al
            var branches = _context.Branches.ToList();     // verileri belleğe al

            var model = new EmployeeViewModel
            {
                EmployeeList = employees.Select(e => new EmployeeViewModel
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeName = e.EmployeeName,
                    EmployeeSurname = e.EmployeeSurname,
                    EmployeeMail = e.EmployeeMail,
                    BranchId = e.BranchId,
                    BranchName = branches.FirstOrDefault(b => b.BranchId == e.BranchId)?.BranchName ?? "Tanımsız"
                }).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult CreateAccount()
            {
            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            // Branch ve Bank listelerini doldur
            ViewBag.BranchList = new SelectList(_context.Branches, "BranchId", "BranchName");
                ViewBag.BankList = new SelectList(_context.Banks, "BankId", "BankName");

                var model = new AccountViewModel
                {
                    Accounts = new List<AccountViewModel>()
                };
                return View(model);
            }

        [HttpPost]
        public async Task<IActionResult> CreateAccountPost(AccountViewModel model)
            {

            if (HttpContext.Session.GetString("managerMail") == null)
            {
                return RedirectToAction("Login", "Manager");
            }
            try
                {
                    // 1) CustomerMail'e göre müşteriyi bul
                    var customer = _context.Customers.FirstOrDefault(c => c.CustomerMail == model.CustomerMail);
                    if (customer == null)
                        throw new Exception("Bu e‑posta ile kayıtlı bir müşteri bulunamadı.");

                    // 2) Şube ve banka
                    var branch = _context.Branches.Find(model.SelectedBranchId)
                                 ?? throw new Exception("Şube bulunamadı.");
                    var bank = _context.Banks.Find(model.SelectedBankId)
                                 ?? throw new Exception("Banka bulunamadı.");

                    // 3) Random hesap no oluştur ve metota gönder
                    var created = await _managerManager.CreateAccount(
                        customer.Id,
                        model.Balance,
                        null, // accountNumber parametresi; ManagerManager kendi random'unu kullanacak
                        model.SelectedBranchId,
                        model.SelectedBankId
                    );

                    ViewBag.SuccessMessage = $"Hesap ({created.AccountNumber}) başarıyla oluşturuldu.";
                }
                catch (Exception ex)
                {
                // Eğer repository’de EF hatasını sarmaladıysanız, burada InnerException’a bakın:
                var detailed = ex.InnerException?.Message ?? ex.Message;
                // Örneğin:
                Console.WriteLine("Save hatası: " + detailed);
                throw;  // ya da ViewBag.ErrorMessage = detailed;
            }

                // Form tekrar doldurma
                ViewBag.BranchList = new SelectList(_context.Branches, "BranchId", "BranchName", model.SelectedBranchId);
                ViewBag.BankList = new SelectList(_context.Banks, "BankId", "BankName", model.SelectedBankId);

                // Var olan hesaplar
                model.Accounts = _context.Accounts
                    .Include(a => a.Customer)
                    .Include(a => a.Branch)
                    .Include(a => a.Bank)
                    .Select(a => new AccountViewModel
                    {
                        AccountNumber = a.AccountNumber,
                        Balance = a.Balance,
                        CustomerMail = a.Customer.CustomerMail,
                        BranchName = a.Branch.BranchName,
                        BankName = a.Bank.BankName
                    })
                    .ToList();

                return View("CreateAccount", model);
            }
   
        [HttpGet]
        public IActionResult ChangePassword()
        {
            string ManagerMail = HttpContext.Session.GetString("managerMail");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            string ManagerMail = HttpContext.Session.GetString("managerMail");

            if (string.IsNullOrEmpty(ManagerMail))
            {
                return RedirectToAction("Login"); // Giriş yapılmamışsa Login'e at
            }
            var newHashedPassword = PasswordHasher.HashPassword(newPassword);
            var result = await _managerManager.ChangePassword(ManagerMail, oldPassword, newHashedPassword);

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









