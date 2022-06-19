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
        
        [HttpGet("GetMoneySource/{licenseDetailId}")]
        public IActionResult GetMoneySource(Guid licenseDetailId)
        {
            try
            {
                var res = _licenseDetailRepository.GetMoneySource(licenseDetailId);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        
        /// <summary>
        /// Sửa bản ghi master - detail
        /// </summary>
        /// <param name="licenseInsert"></param>
        /// <returns></returns>
        [HttpPut("{licenseDetailId}")]
        public IActionResult UpdateLicenseDetail(Guid licenseDetailId, LicenseDetail licenseDetail)
        {
            try
            {
                var res = _licenseDetailRepository.UpdateLicenseDetail(licenseDetailId, licenseDetail.DetailJson);
                return StatusCode(201, res);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Lấy toàn bộ license detail theo license id của master
        /// </summary>
        /// <param name="licenseId"></param>
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
        //[HttpGet("{licenseDetailId}")]
        //public IActionResult GetDetailJsonById(Guid licenseDetailId)
        //{
        //    try
        //    {
        //        var res = _licenseDetailRepository.GetDetailJsonById(licenseDetailId);
        //        return Ok(res);
        //    }
        //    catch(Exception ex)
        //    {
        //        return HandleException(ex);
        //    }
        //}
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
