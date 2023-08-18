
using Employee_TMS.Entities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System.Diagnostics.CodeAnalysis;

namespace Employee_TMS.Repositories
{

    public class ETMSRepository
    {
        private readonly ETMSContext _etmsContext;
        public ETMSRepository(ETMSContext etmsContext)
        {
            _etmsContext = etmsContext;
        }


        public void AddEmployee(Employee addemployee)
        {
           
            _etmsContext.Employees.Add(addemployee);
            _etmsContext.SaveChanges();
        }

        public Employee GetEmployeeById(string EmployeeId)
        {
            return _etmsContext.Employees.Find(EmployeeId);

        }
      
        public void EditEmployee(Employee editemployee)
        {

            _etmsContext.Employees.Update(editemployee);
            _etmsContext.SaveChanges();
        }

       
        public void DeleteReports(List<Report> reports)
        {
            _etmsContext.Reports.RemoveRange(reports);
            _etmsContext.SaveChanges();
        }
        public void DeleteEmployee(Employee employee)
        {
            _etmsContext.Employees.Remove(employee);
            _etmsContext.SaveChanges();
        }
        public IEnumerable<Employee> GetAllEmployee()
        {
            IEnumerable<Employee> Employees = _etmsContext.Employees.ToList();
            return Employees;
        }
        public IEnumerable<Employee> GetAllEmployeeManager()
        {
            return _etmsContext.Employees.Where(x => x.Designation == "Manager").ToList();
        }
        public List<List<Employee>> GetEmployeeByDepartment()
        {
            var employeesByDepartment = _etmsContext.Employees
                                       .GroupBy(dept => dept.Department)
                                      .Select(group => group.ToList())
                                      .ToList();
            return employeesByDepartment;
        }
        public List<Report> GetEmployeeReport(string employeeId)
        {
           return _etmsContext.Reports.Where(id => id.EmployeeId == employeeId).ToList();
            
         }
        public Admin GetAdminById(string AdminId)
        {
            
                return _etmsContext.Admins.Find(AdminId);            
        }
        public void AddAdmin(Admin Addadmin)
        {
            Addadmin.AdminPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(Addadmin.AdminPassword);
            _etmsContext.Admins.Add(Addadmin);
            _etmsContext.SaveChanges();
        }
       
        public void UpdateExcel(Employee employee)
        {
            _etmsContext.UpdateRange(employee);
            _etmsContext.SaveChanges();
         }
        public void AddExcel(List<Employee> employee)
        {
            _etmsContext.AddRange(employee);
            _etmsContext.SaveChanges();
        }


        #region employee

        public void UpdatePassword(string employeeId,string newPassword)
        {
           
            Employee employee = _etmsContext.Employees.Find(employeeId);
            employee.EmployeePassword = BCrypt.Net.BCrypt.EnhancedHashPassword(newPassword);
            employee.Passwordchanged = DateTime.Now;
            _etmsContext.Employees.Update(employee);
            _etmsContext.SaveChanges();
        }

        public void EditProfile(Employee editemployee)
        {

            _etmsContext.Employees.Update(editemployee);
            _etmsContext.SaveChanges();
        }


        public void AddCheckInRecord(Report checkInRecord)
        {
            _etmsContext.Reports.Add(checkInRecord);
             _etmsContext.SaveChanges();
        }
        public Report LastCheckInRecord(string employeeId)
        {
            return _etmsContext.Reports
                .Where(info => info.EmployeeId == employeeId)
                .OrderByDescending(check => check.CheckIn)
                .FirstOrDefault();
        }

        public void UpdateCheckInRecord(Report checkInRecord)
        {
            _etmsContext.Reports.Update(checkInRecord);
            _etmsContext.SaveChanges();
        }
        public Report GetTodayReport(string employeeId)
        {
            return _etmsContext.Reports
                .Where(info => info.EmployeeId == employeeId && info.CheckIn.Value.Date == DateTime.Now.Date)
                .FirstOrDefault();
        }
      

    }
}
   


        #endregion
       

    
