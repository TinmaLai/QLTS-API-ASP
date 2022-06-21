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
    public class LicenseController : MISABaseController<License>
    {
        ILicenseRepository _licenseRepository;
        ILicenseService _licenseService;
        public LicenseController(ILicenseService licenseService, ILicenseRepository licenseRepository) : base(licenseService, licenseRepository)
        {
            _licenseRepository = licenseRepository;
            _licenseService = licenseService;
        }
        /// <summary>
        /// Lấy mã chứng từ mới
        /// </summary>
        /// <returns></returns>
        [HttpGet("NewAssetCode")]
        public IActionResult GetNewCode()
        {
            try
            {
                string newLicenseCode = _licenseRepository.GetNewCode();
                return Ok(newLicenseCode);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Sửa chứng từ theo id
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="fixedAsset"></param>
        /// <returns></returns>
        [HttpPut("{licenseId}")]
        public IActionResult Put(Guid licenseId, License license)
        {
            try
            {
                var res = _licenseService.Update(licenseId, license);
                return StatusCode(201, res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Hàm lọc theo số chứng từ, nội dung ở bảng chứng từ
        /// </summary>
        /// <param name="filterContent"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        [HttpGet("filter")]
        public IActionResult Filter(string? filterContent, int? pageSize = 15, int? pageNumber = 1)
        {
            try
            {
                var res = _licenseRepository.Filter(filterContent, pageSize, pageNumber);

                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Xóa nhiều bản ghi
        /// </summary>
        /// <param name="ids">Mảng id được xóa</param>
        /// <returns></returns>
        [HttpDelete("multiDelete")]
        public IActionResult MultiDelete(Guid[] ids)
        {
            try
            {
                var res = _licenseRepository.MultiDelete(ids);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Xử lý khi có lỗi
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
