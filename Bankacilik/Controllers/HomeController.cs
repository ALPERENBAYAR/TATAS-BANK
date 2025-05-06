using Bankacilik.Models;
using BusinessLayer.Concrete;
using DataAccessLayer.Concrete;
using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Bankacilik.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CustomerManager _customerManager;
        private readonly Context _context;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _customerManager = new CustomerManager();
            _context = new Context();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Login");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            // Doðru anahtar isimleri:
            string customerMail = HttpContext.Session.GetString("CustomerMail");
            int? customerId = HttpContext.Session.GetInt32("CustomerId");

            if (customerMail == null || customerId == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            var customer = _context.Customers.FirstOrDefault(c => c.Id == customerId);
            if (customer == null)
            {
                return RedirectToAction("Login", "Customer");
            }

            return View(customer);
        }
        public IActionResult Dashboard2()
        {
            // Küçük harfli anahtarlarla okuma
            string employeeMail = HttpContext.Session.GetString("employeeMail");
            int? employeeId = HttpContext.Session.GetInt32("employeeId");

            if (employeeMail == null || employeeId == null)
            {
                return RedirectToAction("Login", "Employee");
            }

            var employee = _context.Employees.FirstOrDefault(c => c.EmployeeId == employeeId);
            if (employee == null)
            {
                return RedirectToAction("Login", "Employee");
            }

            return View(employee);
        }
        public IActionResult Dashboard3()
        {
            // Küçük harfli anahtarlarla okuma
            string managerMail = HttpContext.Session.GetString("managerMail");
            int? managerId = HttpContext.Session.GetInt32("managerId");

            if (managerMail == null || managerId == null)
            {
                return RedirectToAction("Login", "Manager");
            }

            var manager = _context.Managers.FirstOrDefault(c => c.ManagerId == managerId);
            if (manager == null)
            {
                return RedirectToAction("Login", "Manager");
            }

            return View(manager);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ForgotPassword(string name, string surname, string email, string userType)
        {
            bool isValid = false;

            if (userType == "Customer")
                isValid = _context.Customers.Any(c => c.CustomerName == name && c.CustomerSurname == surname && c.CustomerMail == email);
            else if (userType == "Employee")
                isValid = _context.Employees.Any(e => e.EmployeeName == name && e.EmployeeSurname == surname && e.EmployeeMail == email);
            else if (userType == "Manager")
                isValid = _context.Managers.Any(m => m.ManagerName == name && m.ManagerSurname == surname && m.ManagerMail == email);

            if (!isValid)
            {
                ViewBag.Error = "Bilgiler eþleþmedi. Lütfen tekrar deneyin.";
                return View();
            }

            TempData["ResetEmail"] = email;
            TempData["UserType"] = userType;

            return RedirectToAction("ResetPassword");
        }
        [HttpGet]
        public IActionResult ResetPassword()
        {
            if (TempData["ResetEmail"] == null || TempData["UserType"] == null)
                return RedirectToAction("Login");

            TempData.Keep(); // TempData’nýn 1 yönlendirme daha yaþamasýný saðlar
            ViewBag.Email = TempData["ResetEmail"];
            ViewBag.UserType = TempData["UserType"];
            return View();
        }
        [HttpPost]
        public IActionResult ResetPassword(string email, string newPassword, string userType)
        {
            if (userType == "Customer")
            {
                var customer = _context.Customers.FirstOrDefault(c => c.CustomerMail == email);
                if (customer != null) customer.CustomerPassword = newPassword;
            }
            else if (userType == "Employee")
            {
                var employee = _context.Employees.FirstOrDefault(e => e.EmployeeMail == email);
                if (employee != null) employee.EmployeePassword = newPassword;
            }
            else if (userType == "Manager")
            {
                var manager = _context.Managers.FirstOrDefault(m => m.ManagerMail == email);
                if (manager != null) manager.ManagerPassword = newPassword;
            }

            _context.SaveChanges();
            TempData["Success"] = "Þifreniz baþarýyla güncellendi.";
            return RedirectToAction("Login", userType); // Login metodu ilgili controllerda varsa çalýþýr.
        }
    }
}