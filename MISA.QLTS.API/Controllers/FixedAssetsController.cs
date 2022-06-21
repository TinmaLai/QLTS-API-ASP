using Microsoft.AspNetCore.Authorization;
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
        /// <summary>
        /// Lấy mã mới
        /// </summary>
        /// <returns></returns>
        [HttpGet("NewAssetCode")]
        public IActionResult GetNewCode()
        {
            try
            {
                string newAssetCode = _fixedAssetRepository.GetNewCode();
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
        /// <summary>
        /// Thực hiện import dữ liệu vào từ file Excel
        /// </summary>
        /// <param name="formFile">File import</param>
        /// <returns></returns>
        [HttpPost("import")]
        public IActionResult Import(IFormFile formFile)
        {
            try
            {
                var res = _fixedAssetService.Import(formFile);
                return Ok(res);
            }
            catch(Exception ex)
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
                //var res = _fixedAssetRepository.MultiDelete(ids);
                var res = _fixedAssetService.DeleteService(ids);
                return Ok(res);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        /// <summary>
        /// Hàm gọi vào repository để filter
        /// </summary>
        /// <param name="searchContent">Chuỗi tìm kiếm</param>
        /// <param name="departmentName">Tên bộ phận sử dụng</param>
        /// <param name="fixedAssetCategoryName">Tên loại tài sản</param>
        /// <param name="pageSize">Số bản ghi trong một trang</param>
        /// <param name="pageNumber">Trang số mấy</param>
        /// <returns></returns>
        [HttpGet("filter")]
        public IActionResult GetPaging(string? searchContent, string? departmentName, string? fixedAssetCategoryName, int? pageSize = 15, int? pageNumber = 1)
        {
            try
            {
                var res = _fixedAssetRepository.Filter(searchContent, departmentName, fixedAssetCategoryName, pageSize, pageNumber);
                
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet("LicenseAssets")]
        public IActionResult GetLicenseAssets([FromQuery] Guid[] ids, string? filterContent, int? pageSize = 15, int? pageNumber = 1)
        {
            try
            {

                var res = _fixedAssetRepository.GetLicenseAssets(ids, filterContent, pageSize, pageNumber);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
