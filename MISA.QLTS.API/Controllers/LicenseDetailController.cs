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
                var res = _licenseDetailService.Insert(licenseInsert);
                return Ok(res);
            } 
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet("getLicenseInsertById")]
        public IActionResult GetLicenseInsertById(Guid licenseMasterId)
        {
            try
            {
                var res = _licenseDetailRepository.GetLicenseInsertById(licenseMasterId);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Sửa bản thi master - detail
        /// </summary>
        /// <param name="licenseInsert"></param>
        /// <returns></returns>
        [HttpPut("{licenseId}")]
        public IActionResult UpdateLicenseInsert(LicenseInsert licenseInsert, Guid licenseId)
        {
            try
            {
                var res = _licenseDetailService.Update(licenseInsert, licenseId);
                return StatusCode(201, res);
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
        [HttpGet("getDetailAssets")]
        public IActionResult GetDetailAssets(Guid licenseId)
        {
            try
            {
                var res = _licenseDetailRepository.GetDetailAssets(licenseId);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
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
                return StatusCode(200, res);

            }
            else return StatusCode(500, res);
        }
    }
}
