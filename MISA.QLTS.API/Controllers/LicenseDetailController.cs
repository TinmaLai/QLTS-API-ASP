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
    public class LicenseDetailController : MISABaseController<LicenseDetail>
    {
        ILicenseDetailRepository _licenseDetailRepository;
        ILicenseDetailService _licenseDetailService;
        public LicenseDetailController(ILicenseDetailService licenseDetailService, ILicenseDetailRepository licenseDetailRepository) : base(licenseDetailService, licenseDetailRepository)
        {
            _licenseDetailRepository = licenseDetailRepository;
            _licenseDetailService = licenseDetailService;
        }
        [HttpPost("multiData")]
        public IActionResult PostMulti(LicenseInsert licenseInsert)
        {
            try
            {
                var res = _licenseDetailRepository.MultiInsert(licenseInsert);
                return Ok(res);
            } 
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Base xử lý exception nếu có
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
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
                return StatusCode(200, res);

            }
            else return StatusCode(500, res);
        }
    }
}
