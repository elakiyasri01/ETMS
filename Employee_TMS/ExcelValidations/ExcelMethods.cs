using Employee_TMS.Repositories;
using Employee_TMS.Entities;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using BCrypt.Net;
using OfficeOpenXml;

namespace Employee_TMS.ExcelValidations
{
    public class ExcelMethods
    {
        private readonly ETMSRepository _etmsRepository;
        public ExcelMethods(ETMSRepository etmsRepository)
        {
            _etmsRepository = etmsRepository;
        }
        public bool IsEmptyFile(IFormFile file)
        {
            if (file.Length < 0)
            {
                return true;
            }
            return false;
        }
        public bool EmptyCellCheck(IFormFile file)
        {
            var list = new List<Employee>();
            using (var stream = new MemoryStream())
            {
                file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;
                    for (int row = 2; row <= rowcount; row++)
                    {
                        if (worksheet.Cells[row, 1].Value == null || worksheet.Cells[row, 3].Value == null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void Excel(IFormFile file)
        {
            var list = new List<Employee>();
            using (var stream = new MemoryStream())
            {
                file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowcount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowcount; row++)
                    {
                        string plainTextPassword = worksheet.Cells[row, 2].Value?.ToString() ?? string.Empty;
                        string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(plainTextPassword);

                        var employeeId = worksheet.Cells[row, 1].Value?.ToString() ?? string.Empty;

                        var existingEmployee = _etmsRepository.GetEmployeeById(employeeId);
                        if (existingEmployee != null)
                        {
                            existingEmployee.EmployeeId = employeeId;
                            existingEmployee.EmployeePassword = hashedPassword;
                            existingEmployee.FullName = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty;
                            existingEmployee.Department = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty;
                            existingEmployee.Email = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty;
                            existingEmployee.Gender = worksheet.Cells[row, 6].Value?.ToString() ?? string.Empty;
                            _etmsRepository.UpdateExcel(existingEmployee);
                            
                        }
                        else
                        {
                            list.Add(new Employee()
                            {
                                EmployeeId = employeeId,
                                EmployeePassword = hashedPassword,
                                FullName = worksheet.Cells[row, 3].Value?.ToString() ?? string.Empty,
                                Department = worksheet.Cells[row, 4].Value?.ToString() ?? string.Empty,
                                Email = worksheet.Cells[row, 5].Value?.ToString() ?? string.Empty,
                                Gender = worksheet.Cells[row, 6].Value?.ToString()??string.Empty   ,
                        });
                            
                        }
                    }
                    _etmsRepository.AddExcel(list);
                }
            }
        }
    }
}
