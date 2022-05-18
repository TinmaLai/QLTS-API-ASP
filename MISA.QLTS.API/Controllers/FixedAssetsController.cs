using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Exceptions;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MISA.QLTS.CORE.Interfaces.Services;
using MISA.QLTS.Infrasructure.Repository;

namespace MISA.QLTS.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FixedAssetsController : MISABaseController<FixedAsset>
    {
        IFixedAssetRepository _fixedAssetRepository;
        IFixedAssetService _fixedAssetService;
        public FixedAssetsController(IFixedAssetRepository fixedAssetRepository,IFixedAssetService fixedAssetService):base(fixedAssetService,fixedAssetRepository)
        {
            _fixedAssetRepository = fixedAssetRepository;
            _fixedAssetService = fixedAssetService;
        }
        [HttpGet("NewAssetCode")]
        public IActionResult GetNewCode()
        {
            try
            {
                string newAssetCode = _fixedAssetRepository.getNewCode();
                return Ok(newAssetCode);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Sửa tài sản theo id
        /// </summary>
        /// <param name="assetId"></param>
        /// <param name="fixedAsset"></param>
        /// <returns></returns>
        [HttpPut("{fixedAssetId}")]
        public IActionResult Put(Guid fixedAssetId, FixedAsset fixedAsset)
        {
            try
            {
                var res = _fixedAssetService.Update(fixedAssetId, fixedAsset);
                return StatusCode(201, res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Xóa bản ghi theo id
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        [HttpDelete("{assetId}")]
        public IActionResult Delete(Guid assetId)
        {
            try
            {
                var res = _fixedAssetRepository.Delete(assetId);
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
                return StatusCode(400, res);

            }
            else return StatusCode(500, res);
        }
    }
}
