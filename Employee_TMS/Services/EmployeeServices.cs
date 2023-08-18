using Employee_TMS.Entities;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;
using Employee_TMS.Repositories;

using static System.Net.Mime.MediaTypeNames;


namespace Employee_TMS.Services
{
    public class EmployeeServices
    {
        private readonly ETMSRepository _etmsRepository;
       
        public EmployeeServices(ETMSRepository etmsRepository)
        {
            _etmsRepository = etmsRepository;
           
        }
        #region Admin


        public void AddAdmin(Admin addAdmin)
        {
            
            _etmsRepository.AddAdmin(addAdmin);

        }
        public Admin GetAdminById(string AdminId)
        {
            {
                Admin admin = _etmsRepository.GetAdminById(AdminId);
                return admin;
            }
        }
        public bool IsAuthenticatedAdmin(Admin adminLogin)
        {
            Admin authenticatedAdmin = _etmsRepository.GetAdminById(adminLogin.AdminId);
            if (authenticatedAdmin != null)
            {
                bool isAuthenticated = BCrypt.Net.BCrypt.EnhancedVerify(adminLogin.AdminPassword, authenticatedAdmin.AdminPassword);
                return isAuthenticated;
            }
            return false;

        }
       public void AddEmployeewithoutProfile(Employee addemployee)
        {
            addemployee.EmployeePassword = BCrypt.Net.BCrypt.EnhancedHashPassword(addemployee.EmployeePassword);
            _etmsRepository.AddEmployee(addemployee);
        }
        public void AddEmployee(Employee addemployee,IFormFile ProfileImage)
        {
            addemployee.EmployeePassword = BCrypt.Net.BCrypt.EnhancedHashPassword(addemployee.EmployeePassword);
            addemployee.ProfileImageName = Path.GetFileName(ProfileImage.FileName);
            using (var memoryStream = new MemoryStream())
            {
                ProfileImage.CopyTo(memoryStream);
                addemployee.ProfileImage = memoryStream.ToArray();
            }

            _etmsRepository.AddEmployee(addemployee);

        }
        public IEnumerable<Employee> GetAllEmployee()
        {
            IEnumerable<Employee> employee = _etmsRepository.GetAllEmployee();
            return employee;
        }
        public Employee GetEmployeeById(string EmployeeId)
        {

            Employee employee = _etmsRepository.GetEmployeeById(EmployeeId);
            return employee;

        }
        public void EditEmployeewithoutProfile(Employee employee)
        {
            _etmsRepository.EditEmployee(employee);
        }
        public void EditProfilewithoutImage(Employee employee)
        {
            _etmsRepository.EditProfile(employee);
        }
        public void EditEmployee(Employee editemployee,IFormFile ProfileImage)

        {
            if (ProfileImage != null)
            {
                editemployee.ProfileImageName = Path.GetFileName(ProfileImage.FileName);

                using (var memoryStream = new MemoryStream())
                {
                    ProfileImage.CopyTo(memoryStream);
                    editemployee.ProfileImage = memoryStream.ToArray();
                }

                _etmsRepository.EditEmployee(editemployee);
            }
            else
            {
                _etmsRepository.EditEmployee(editemployee);
            }
        }


        public void DeleteEmployee(string EmployeeId)
        {
            List<Report> reports = _etmsRepository.GetEmployeeReport(EmployeeId).ToList();
            _etmsRepository.DeleteReports(reports);

            Employee employee = _etmsRepository.GetEmployeeById(EmployeeId);
            _etmsRepository.DeleteEmployee(employee);


        }
        public bool IsImageValid(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return false;
            }

            string[] validExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            string fileExtension = Path.GetExtension(image.FileName).ToLower();

            return validExtensions.Contains(fileExtension);
        }
        public bool EmployeeIdExists(string employeeId)
        {
            if (_etmsRepository.GetEmployeeById(employeeId) != null)
            {

                return true;
            }
            else
            { 
                return false; 
            }
        }



        #endregion


        #region Employee
        public IEnumerable<Employee> GetAllEmployeeManager()
        {
            return _etmsRepository.GetAllEmployeeManager();
        }

        public bool IsAuthenticatedEmployee(EmployeeLogin employeeLogin)
        {
            Employee authenticatedEmployee = _etmsRepository.GetEmployeeById(employeeLogin.EmployeeId);

            if (authenticatedEmployee != null)
            {
                bool isAuthenticated = BCrypt.Net.BCrypt.EnhancedVerify(employeeLogin.EmployeePassword, authenticatedEmployee.EmployeePassword);
                return isAuthenticated;
            }

            return false;
        }
        public void UpdatePassword(string employeeId,string newPassword)
        {
          
            _etmsRepository.UpdatePassword(employeeId,newPassword);
        }
        public void EditProfile(Employee editemployee,IFormFile ProfileImage)
        {
            editemployee.ProfileImageName = Path.GetFileName(ProfileImage.FileName);

            using (var memoryStream = new MemoryStream())
            {
                ProfileImage.CopyTo(memoryStream);
                editemployee.ProfileImage = memoryStream.ToArray();
            }
          
            _etmsRepository.EditProfile(editemployee);
        }
        public void CheckIn(string employeeId)
        {
           
            var todayReport = _etmsRepository.GetTodayReport(employeeId);
            if (todayReport == null)
            {
                var checkInRecord = new Report
                {
                    EmployeeId = employeeId,
                    CheckIn = DateTime.Now
                };

                _etmsRepository.AddCheckInRecord(checkInRecord);
            }
        }

        public void CheckOut(string employeeId)
        {
            var todayReport = _etmsRepository.GetTodayReport(employeeId);

            if (todayReport.CheckOut == null)
            { 
                
                todayReport.CheckOut = DateTime.Now;
                todayReport.WorkedHours = todayReport.CheckOut - todayReport.CheckIn;
                _etmsRepository.UpdateCheckInRecord(todayReport);
            }
           

        }


        public List<Report> GetEmployeeReport(string employeeId)
        {
            List<Report> reports = _etmsRepository.GetEmployeeReport(employeeId).ToList();
            return reports;
        }
        public List<List<Employee>> GetEmployeeByDepartment()
        {
            var  employeeByDepartment = _etmsRepository.GetEmployeeByDepartment().ToList();
            return employeeByDepartment;
        }

        
        #endregion
    }


}

