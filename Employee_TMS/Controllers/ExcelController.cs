using Employee_TMS.Services;
using Microsoft.AspNetCore.Mvc;
using Employee_TMS.ExcelValidations;
using Employee_TMS.Entities;

namespace Employee_TMS.Controllers
{
    public class ExcelController : Controller
    {
        private readonly ExcelMethods _excelMethods;
        private readonly APIservices _apiService;
        public ExcelController(ExcelMethods excelMethods, APIservices apiservices)
        {
            this._excelMethods = excelMethods;
            this._apiService = apiservices;
        }
        public IActionResult Excel(IFormFile file)
        {
            if (_excelMethods.IsEmptyFile(file))
            {
                TempData["Message"] = "File is empty";
                return RedirectToAction("GetAllEmployee", "Admin");
            }
            else if (_excelMethods.EmptyCellCheck(file))
            {
                TempData["Message"] = "Empty cell identified on required field";
                return RedirectToAction("GetAllEmployee", "Admin");
            }
            else

                _excelMethods.Excel(file);
            TempData["ExcelMessage"] = "successfully Updated";
            return RedirectToAction("GetAllEmployee", "Admin");

        }
        [HttpGet]
        public async Task<IActionResult> DownloadExcel()
        {
            byte[] excelData = await _apiService.GenerateExcel();
            if (excelData != null)
            {
                return File(excelData, "application/vnd.ms-excel", "EmployeeList.xls");
            }
            else
            {
                TempData["Excelerror"] = "error while downloading";
                return RedirectToAction("GetAllEmployee");
            }
        }
    }
}
