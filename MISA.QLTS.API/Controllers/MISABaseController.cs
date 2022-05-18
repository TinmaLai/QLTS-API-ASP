using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.QLTS.CORE.Exceptions;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MISA.QLTS.CORE.Interfaces.Services;

namespace MISA.QLTS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class MISABaseController<T> : ControllerBase
    {
        IBaseService<T> _baseService;
        IBaseRepository<T> _baseRepository;
        public MISABaseController(IBaseService<T> baseService, IBaseRepository<T> baseRepository)
        {
            _baseService = baseService;
            _baseRepository = baseRepository;
        }

        [HttpGet]
        public virtual IActionResult Get()
        {
            try
            {
                var entities = _baseRepository.Get();
                return Ok(entities);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpGet("{entityId}")]
        public virtual IActionResult Get(Guid entityId)
        {
            try
            {
                var entity = _baseRepository.GetById(entityId);
                return Ok(entity);
            }
            catch(Exception ex)
            {
                return HandleException(ex);
            }
        }
        [HttpPost]
        public virtual IActionResult Post(T entity)
        {
            try
            {
                var res = _baseService.Insert(entity);
                return StatusCode(201, res);
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
                return StatusCode(400, res);

            }
            else return StatusCode(500, res);
        }
    }
}
