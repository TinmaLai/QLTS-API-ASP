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
    public class FixedAssetCategoriesController : MISABaseController<FixedAssetCategory>
    {
        IFixedAssetCategoryRepository _fixedAssetCategoryRepository;
        IFixedAssetCategoryService _fixedAssetCategoryService;
        IConfiguration _configuration;
        /// <summary>
        /// Khởi tạo FixedAssetCategoryRepository, FixedAssetCategoryService
        /// </summary>
        /// <param name="fixedAssetCategoryRepository"></param>
        /// <param name="fixedAssetCategoryService"></param>
        public FixedAssetCategoriesController(IFixedAssetCategoryRepository fixedAssetCategoryRepository, IFixedAssetCategoryService fixedAssetCategoryService):base(fixedAssetCategoryService,fixedAssetCategoryRepository)
        {
            _fixedAssetCategoryRepository = fixedAssetCategoryRepository;
            _fixedAssetCategoryService = fixedAssetCategoryService;
        }
    }
}
