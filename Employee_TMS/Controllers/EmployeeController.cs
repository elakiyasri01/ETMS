using Employee_TMS.Entities;
using Employee_TMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Employee_TMS.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeServices _employeeServices;
        private readonly TokenService _tokenService;
        private readonly GoogleCaptchaService _captchaService;

        public EmployeeController(EmployeeServices employeeservices, TokenService tokenService, GoogleCaptchaService captchaService)
        {
            this._employeeServices = employeeservices;
            this._tokenService = tokenService;
            this._captchaService = captchaService;

        }


        #region Employee
        public IActionResult Profile()
        {
            Employee employee = _employeeServices.GetEmployeeById(HttpContext.Session.GetString("employeeId"));
            return View(employee);

        }

        [HttpGet]
        public IActionResult LoginEmployee()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginEmployee(EmployeeLogin employeeLogin)
        {
            try
            {
                if (_employeeServices.IsAuthenticatedEmployee(employeeLogin))
                {
                    HttpContext.Session.SetString("employeeId", employeeLogin.EmployeeId);
                    var captchaResult = _captchaService.VerifyToken(employeeLogin.Token);
                    var token = _tokenService.GenerateToken(employeeLogin.EmployeeId, employeeLogin.EmployeePassword);

                    if (await captchaResult)
                    {
                        if (_tokenService.IsTokenValid(token))
                        {
                            return RedirectToAction("EmployeeDashboard");

                        }
                        else
                        {
                            ModelState.AddModelError("", "Token not valid");
                            return View(employeeLogin);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Recaptcha is not in success");
                        return View(employeeLogin);
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Incorrect EmployeeId or Password");
                    return View(employeeLogin);

                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpGet]
        public IActionResult CheckPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CheckPassword(string oldPassword, string newPassword)
        {
            try
            {
                if (oldPassword != null & newPassword != null)
                {
                    if (oldPassword != newPassword)
                    {
                        Employee employee = _employeeServices.GetEmployeeById(HttpContext.Session.GetString("employeeId"));
                        if (BCrypt.Net.BCrypt.EnhancedVerify(oldPassword, employee.EmployeePassword))
                        {
                            _employeeServices.UpdatePassword(employee.EmployeeId, newPassword);
                            return RedirectToAction("Logout");
                        }
                        else
                        {
                            ViewBag.Error = "Old Password is wrong";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Error = "old and new password are same";
                        return View();
                    }
                }
                else
                {
                    ViewBag.Error = "Password is empty";
                    return View();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public IActionResult EditProfile(string EmployeeId)
        {

            try
            {
                Employee employee = _employeeServices.GetEmployeeById(EmployeeId);
                if (employee.ProfileImage == null)
                { 
                    return View("EditProfile", employee);

                }
                else
                {
                    HttpContext.Session.Set("ExistingProfileImage", employee.ProfileImage);
                    return View(employee);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult EditProfile(Employee editEmployee, IFormFile ProfileImage)
        {
            try
            {
                byte[] existingProfileImage = HttpContext.Session.Get("ExistingProfileImage");

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    string fileExtension = Path.GetExtension(ProfileImage.FileName).ToLower();

                    if (!validExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("ProfileImage", "Only image files (jpg, jpeg, png, gif) are allowed.");
                        return View("EditProfile", editEmployee);
                    }
                    else
                    {
                        _employeeServices.EditProfile(editEmployee, ProfileImage);
                        return RedirectToAction("EmployeeDashboard");
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
                        _employeeServices.EditProfile(editEmployee, file);
                        return RedirectToAction("EmployeeDashboard");

                 }
                else
                {
                     _employeeServices.EditProfilewithoutImage(editEmployee);
                    return RedirectToAction("EmployeeDashboard");
                 }
                }

            
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public IActionResult Id_ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Id_ForgotPassword(string employeeId)
        {
            try
            {

                Employee employee = _employeeServices.GetEmployeeById(employeeId);

                if (employee != null)
                {
                    return RedirectToAction("SecurityQuestion", new { employeeId = employee.EmployeeId });
                }
                else
                {
                    ViewBag.Message = "EmployeeId not found";
                    return View("Id_ForgotPassword");
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
       
        public IActionResult SecurityQuestion(string employeeId)
        {
            try
            {
                Employee employee = _employeeServices.GetEmployeeById(employeeId);
                ViewBag.EmployeeId = employee.EmployeeId;
                ViewBag.SecurityQuestion = employee.SecurityQuestion;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult SecurityQuestion(string employeeId,string securityAnswer)
        {
            try
            {

                Employee employee = _employeeServices.GetEmployeeById(employeeId);
                if (securityAnswer != null && employee.SecurityAnswer == securityAnswer)
                {
                    return RedirectToAction("ResetPassword", new { employeeId = employee.EmployeeId });
                }

                else
                {
                    ViewBag.EmployeeId = employee.EmployeeId;
                    ViewBag.SecurityQuestion = employee.SecurityQuestion;
                    ViewBag.ErrorMessage = "Security Answer is Wrong(Case Sensitive)";
                    return View("SecurityQuestion", employee);
                }

            }
            catch (Exception)
            {

                throw;
            }
        }

       [HttpGet]
        public IActionResult ResetPassword(string employeeId) 
        {
            try
            {

                Employee employee = _employeeServices.GetEmployeeById(employeeId);
                ViewBag.Password=employee.EmployeePassword;
                ViewBag.EmployeeId = employee.EmployeeId;
                return View();
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost]
        public IActionResult ResetPassword(string employeeId,string password,string oldpassword)
        {
            if (!BCrypt.Net.BCrypt.EnhancedVerify(password,oldpassword))
            {
                _employeeServices.UpdatePassword(employeeId, password);
                return RedirectToAction("LoginEmployee");
            }
            else
            {
                ViewBag.Error = "Old and new password are same";
                return View();
            }

        }
        public IActionResult Report()
        {
            ViewBag.EmployeeId = HttpContext.Session.GetString("employeeId");
            var employeeReport = _employeeServices.GetEmployeeReport(ViewBag.EmployeeId);
            return View(employeeReport);
            
        }
        [HttpGet]
        public IActionResult SearchEmployee()
        {
            ViewBag.SearchMessage = TempData["SearchMessage"];
            var employee = _employeeServices.GetAllEmployee().ToList();
            return View(employee);
        }
        public IActionResult GetAllEmployeeDepartment()
        {
            var employeeByDepartment=_employeeServices.GetEmployeeByDepartment().ToList();
            return View(employeeByDepartment);
        }
        public IActionResult EmployeeDetails(string EmployeeId)
        {
           Employee employee= _employeeServices.GetEmployeeById(EmployeeId);
            if (employee == null)
            {
                TempData["SearchMessage"] = "No User Found!😢";
                return RedirectToAction("SearchEmployee");

            }
            else
            {
                return View(employee);
            }

        }

        [HttpPost]
        public IActionResult CheckIn()
        {
            ViewBag.employeeId = HttpContext.Session.GetString("employeeId");
            _employeeServices.CheckIn(ViewBag.employeeId);
            return RedirectToAction("EmployeeDashboard");
        }
        public IActionResult CheckOut()
        {
            ViewBag.employeeId = HttpContext.Session.GetString("employeeId");
            _employeeServices.CheckOut(ViewBag.employeeId);
            return RedirectToAction("EmployeeDashboard");
        }
        public IActionResult Logout()
        {          
            HttpContext.Session.Clear();
            return RedirectToAction("LoginEmployee"); 
        }
        public IActionResult EmployeeDashboard()
        {
            
            ViewBag.EmployeeId = HttpContext.Session.GetString("employeeId");
            Employee employee = _employeeServices.GetEmployeeById(ViewBag.EmployeeId);
            ViewBag.Passwordchanged = employee.Passwordchanged;
            
            var employeeReport = _employeeServices.GetEmployeeReport(ViewBag.EmployeeId);
            return View(employeeReport);
        }
        
        #endregion
    }
}
