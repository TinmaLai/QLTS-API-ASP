using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Exceptions;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MISA.QLTS.CORE.Interfaces.Services;

namespace MISA.QLTS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentsController : MISABaseController<Department>
    {
        IDepartmentRepository _departmentRepository;
        IDepartmentService _departmentService;
        public DepartmentsController(IDepartmentRepository departmentRepository, IDepartmentService departmentService):base(departmentService,departmentRepository)
        {
            _departmentRepository = departmentRepository;
            _departmentService = departmentService;
        }
        private IActionResult HandleException(Exception ex)
        {
            var res = new
            {
                devMsg = ex.Message,
                userMsg = "Có lỗi xảy ra, vui lòng liên hệ MISA để được hỗ trợ.",
                errorCode = "001",
                data = ex.Data
            };
            if (ex is MISAValidateException)
            {
                return StatusCode(400, res);

            }
            else return StatusCode(500, res);
        }
    }
}
