using Employee_TMS.Entities;
using Employee_TMS.Services;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using System;
using OfficeOpenXml;
using System.Xml.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Employee_TMS.Controllers
{
    public class AdminController : Controller
    {
        private readonly EmployeeServices _employeeServices;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AdminController(EmployeeServices services, IWebHostEnvironment webHostEnvironment)
        {
            this._employeeServices = services;
            this.webHostEnvironment = webHostEnvironment;
        }
        #region Admin 
        [HttpGet]
        public IActionResult AddAdmin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddAdmin(Admin addAdmin)
        {
            _employeeServices.AddAdmin(addAdmin);
            return RedirectToAction("GetAllEmployee");
        }

        [HttpGet]
        public IActionResult LoginAdmin()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginAdmin(Admin adminLogin)
        {
            if (_employeeServices.IsAuthenticatedAdmin(adminLogin))
            {
                _employeeServices.GetAdminById(adminLogin.AdminId);
                return RedirectToAction("GetAllEmployee");
            }
            else
            {
                ModelState.AddModelError("", "Incorrect AdminId or Password");
                return View(adminLogin);

            }
        }
        public IActionResult Logout()
        {

            return RedirectToAction("LoginAdmin");
        }
      
        [HttpGet]
        public IActionResult AddEmployee()
        {
            var employee = new Employee();
            var employeeManager = _employeeServices.GetAllEmployeeManager().ToList();
            ViewBag.employeeManager=employeeManager;
            return View("AddEmployee", employee);
        }
        [HttpPost]


        public IActionResult AddEmployee(Employee addEmployee, IFormFile ProfileImage)
        {
            if (addEmployee.EmployeeId==null)
            {
                ViewBag.employeeManager = _employeeServices.GetAllEmployeeManager().ToList();
                return View("AddEmployee", addEmployee);
            }
            else
            {
                if (_employeeServices.EmployeeIdExists(addEmployee.EmployeeId))
                {
                    ModelState.AddModelError("EmployeeId", "Employee ID already exists.");
                    ViewBag.employeeManager = _employeeServices.GetAllEmployeeManager().ToList();
                    return View("AddEmployee", addEmployee);
                }

              
                if (_employeeServices.IsImageValid(ProfileImage))
                {
                    _employeeServices.AddEmployee(addEmployee, ProfileImage);
                    return RedirectToAction("GetAllEmployee");
                }
                else
                {
                    _employeeServices.AddEmployeewithoutProfile(addEmployee);
                    return RedirectToAction("GetAllEmployee");
                }
            }
        }

        [HttpGet]
        public ActionResult<IEnumerable<Employee>> GetAllEmployee()
        {
            IEnumerable<Employee> employee = _employeeServices.GetAllEmployee();
            ViewBag.Message = TempData["Message"];
            ViewBag.ExcelMessage = TempData["ExcelMessage"];
            ViewBag.SearchMessage = TempData["SearchMessage"];
            ViewBag.Excelerror = TempData["Excelerror"];
            return View(employee);
           
        }
       
        [HttpGet]
        public IActionResult EditEmployee(string EmployeeId)
        {
            string defaultImagePath = Path.Combine(webHostEnvironment.WebRootPath, "images", "default-profile.jpg");
            Employee employee = _employeeServices.GetEmployeeById(EmployeeId);
            if (employee.ProfileImage == null)
            {
                var employeeManager = _employeeServices.GetAllEmployeeManager().ToList();
                ViewBag.employeeManager = employeeManager;
                return View("EditEmployee", employee);
              
            }
            else
            {
                HttpContext.Session.Set("ExistingProfileImage", employee.ProfileImage);
                var employeeManager = _employeeServices.GetAllEmployeeManager().ToList();
                ViewBag.employeeManager = employeeManager;
                return View("EditEmployee", employee);
            }
        }
        [HttpPost]
        public IActionResult EditEmployee(Employee editEmployee, IFormFile ProfileImage)
        {
          
            byte[] existingProfileImage = HttpContext.Session.Get("ExistingProfileImage");

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(ProfileImage.FileName).ToLower();

                if (!validExtensions.Contains(fileExtension))
                {
                    ModelState.AddModelError("ProfileImage", "Only image files (jpg, jpeg, png, gif) are allowed.");
                    return View("EditEmployee", editEmployee);
                }
                else
                {
                    _employeeServices.EditEmployee(editEmployee, ProfileImage);
                    return RedirectToAction("GetAllEmployee");
                }
            }
            else if (existingProfileImage != null)
            {
                var stream = new MemoryStream(existingProfileImage);

                var file = new FormFile(stream, 0, existingProfileImage.Length, "profileImage", "profile.jpg")
                {
                    Headers = new HeaderDictionary(),
                    ContentType = "image/jpeg/png" // Set the appropriate content type for your image
                };
                _employeeServices.EditEmployee(editEmployee, file);
                return RedirectToAction("GetAllEmployee");

            }
            else
            {
                _employeeServices.EditEmployeewithoutProfile(editEmployee);
                return RedirectToAction("GetAllEmployee");
            }

            
        }


        public IActionResult DeleteEmployee(string EmployeeId)
        {
            _employeeServices.DeleteEmployee(EmployeeId);
            return RedirectToAction("GetAllEmployee");
        }

        public IActionResult GetReport(string EmployeeId)
        {
            List<Report> employeeReport = _employeeServices.GetEmployeeReport(EmployeeId).ToList();
            ViewBag.EmployeeId = EmployeeId;
            return View(employeeReport);
        }
        public IActionResult SearchEmployee(string EmployeeId)
        {
            Employee employee = _employeeServices.GetEmployeeById(EmployeeId);
            ViewBag.EmployeeId = EmployeeId;
            if (employee == null)
            {
                TempData["SearchMessage"] = "No User Found!😢";
                return RedirectToAction("GetAllEmployee");
            }
            else
            {
                return RedirectToAction("GetReport", new { EmployeeId = EmployeeId });
            }
        }
       


    }
}
        #endregion


