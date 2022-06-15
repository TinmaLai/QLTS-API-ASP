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
