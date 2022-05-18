using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Web03.NBTIN.Entities;
using MySqlConnector;
namespace MISA.Web03.NBTIN.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AssetCategoryController : ControllerBase
    {
        /// <summary>
        /// Lấy toàn bộ các loại tài sản
        /// </summary>
        /// <returns></returns>
        /// CreatedBy: NBTIN (04/05/2022)
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                // Khởi tạo kết nối với database
                var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
                //1. Khởi tạo kết nối với MariaDB
                var sqlConnection = new MySqlConnection(connectionString);
                //2. Lấy dữ liệu
                //2.1. Câu lệnh truy vấn dữ liệu
                var sqlCommand = $"SELECT * FROM fixed_asset_category";
                //2.2. Thực hiện lấy dữ liệu
                var assetCategories = sqlConnection.Query<FixedAssetCategory>(sql: sqlCommand);


                //Trả kết quả cho client

                return Ok(assetCategories);
            }
            catch (Exception ex)
            {
                var error = new ErrorService();
                error.DevMsg = ex.Message;
                error.UserMsg = Resources.ResourceVN.Error_Exception;
                error.Data = ex.Data;
                return StatusCode(500, error);
                throw;
            }

        }
        /// <summary>
        /// Lấy bản ghi danh mục theo id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("{categoryId}")]
        public IActionResult GetById(Guid categoryId)
        {
            // Khởi tạo kết nối với database
            var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
            //1. Khởi tạo kết nối với MariaDB
            var sqlConnection = new MySqlConnection(connectionString);
            //2. Lấy dữ liệu
            //2.1. Câu lệnh truy vấn dữ liệu
            var sqlCommand = $"SELECT * FROM fixed_asset_category WHERE fixed_asset_category_id = @CategoryId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@CategoryId", categoryId);
            //2.2. Thực hiện lấy dữ liệu
            var assetCategory = sqlConnection.QueryFirstOrDefault<object>(sql: sqlCommand, param: parameters);


            //Trả kết quả cho client

            return Ok(assetCategory);
        }
        /// <summary>
        /// Check trùng mã hay không
        /// sửa: 0, thêm: 1
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        private bool CheckDuplicateCode(string categoryCode, Guid categoryId, int mode)
        {
            var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
            var sqlConnection = new MySqlConnection(connectionString);
            var sqlCheck = "";

            if (mode == 1)
            {
                sqlCheck = $"SELECT * FROM fixed_asset_category WHERE fixed_asset_category_code = @categoryCode";

            }
            else if (mode == 0)
            {
                sqlCheck = $"SELECT * FROM fixed_asset_category WHERE fixed_asset_category_code = @categoryCode AND fixed_asset_category_id <> @categoryId";
            }

            var parameters = new DynamicParameters();
            parameters.Add("@categoryId", categoryId);
            parameters.Add("@categoryCode", categoryCode);
            var res = sqlConnection.QueryFirstOrDefault<object>(sqlCheck, parameters);

            if (res != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Thực hiện thêm bản ghi
        /// </summary>
        /// <param name="fixedAssetCategory"></param>
        /// <returns>
        ///     201 - Them thanh cong
        ///     400 - Dau vao khong hop le
        ///     500 - Co exception cua server
        /// </returns>

        [HttpPost]

        public IActionResult Post([FromBody] FixedAssetCategory fixedAssetCategory)
        {
            try
            {
                var error = new ErrorService();
                var errorData = new Dictionary<string, string>();
                fixedAssetCategory.fixed_asset_category_id = Guid.NewGuid();
                fixedAssetCategory.organization_id = Guid.NewGuid();
                // Khởi tạo kết nối với Database

                //1. Valiate dữ liệu: 
                //1.1 Thông tin mã loại tài sản bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAssetCategory.fixed_asset_category_code))
                {
                    errorData.Add("AssetCategoryCode", Resources.ResourceVN.CategoryCodeValidate);

                }
                //1.2 Thông tin tên loại tài sản bắt buộc nhập:
                if (string.IsNullOrEmpty(fixedAssetCategory.fixed_asset_category_name))
                {
                    errorData.Add("AssetCategoryName", Resources.ResourceVN.CategoryNameValidate);

                }
                //1.3 Thông tin tỷ lệ hao mòn năm bắt buộc nhập:
                if (float.IsNegative(fixedAssetCategory.depreciation_rate) || float.IsNaN(fixedAssetCategory.depreciation_rate))
                {
                    errorData.Add("DepreRate", Resources.ResourceVN.CategoryDeprerateValidate);

                }
                //1.4 Thông tin thời gian sử dụng tài sản bắt buộc nhập:
                if (fixedAssetCategory.life_time < 0)
                {
                    errorData.Add("LifeTime", Resources.ResourceVN.CategoryLifetimeValidate);

                }
                //1.5 Mã sau không được trùng với các mã đã có trước đó:
                if (CheckDuplicateCode(fixedAssetCategory.fixed_asset_category_code, fixedAssetCategory.fixed_asset_category_id, 1) == true)
                {
                    errorData.Add("AssetCategoryCode2", Resources.ResourceVN.CategoryCodeDuplicateValidate);
                }
                // Kiểm tra nếu danh sách lỗi là khác null thì in ra thông báo
                if (errorData.Count > 0)
                {
                    error.UserMsg = Resources.ResourceVN.Error_Validate;
                    error.Data = errorData;
                    return BadRequest(error);
                }
                var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
                //1. Kết nối với MariaDB
                var sqlConnection = new MySqlConnection(connectionString);
                //2. Lấy dữ liệu
                //2.1. Câu lệnh truy vấn dữ liệu

                var sqlCommand = $"INSERT INTO fixed_asset_category(fixed_asset_category_id,fixed_asset_category_code,fixed_asset_category_name,organization_id,depreciation_rate,life_time) VALUES (@CategoryId,@CategoryCode,@CategoryName,@OrgaId,@DepreRate,@LifeTime)";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@CategoryId", fixedAssetCategory.fixed_asset_category_id);
                parameters.Add("@CategoryCode", fixedAssetCategory.fixed_asset_category_code);
                parameters.Add("@CategoryName", fixedAssetCategory.fixed_asset_category_name);
                parameters.Add("@OrgaId", fixedAssetCategory.organization_id);
                parameters.Add("@DepreRate", fixedAssetCategory.depreciation_rate);
                parameters.Add("@LifeTime", fixedAssetCategory.life_time);
                var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
                return StatusCode(201, res);
            } catch(Exception ex)
            {
                var error = new ErrorService();
                error.DevMsg = ex.Message;
                error.UserMsg = Resources.ResourceVN.Error_Exception;
                error.Data = ex.Data;
                return StatusCode(500, error);
                throw;
            }

        }

        /// <summary>
        /// Hàm sửa theo category id
        /// CreatedBy: NBTin (05/05/2022)
        /// </summary>
        [HttpPut("{categoryId}")]
        public IActionResult Put(FixedAssetCategory fixedAssetCategory, Guid categoryId)
        {
            try
            {
                var error = new ErrorService();
                var errorData = new Dictionary<string, string>();
                /// Validate cac truong du lieu
                if (string.IsNullOrEmpty(fixedAssetCategory.fixed_asset_category_code))
                {
                    errorData.Add("AssetCategoryCode", Resources.ResourceVN.CategoryCodeValidate);

                }
                //1.2 Thong tin ten loai tai san bat buoc nhap:
                if (string.IsNullOrEmpty(fixedAssetCategory.fixed_asset_category_name))
                {
                    errorData.Add("AssetCategoryName", Resources.ResourceVN.CategoryNameValidate);

                }
                //1.3 Thông tin tỷ lệ hao mòn năm bắt buộc nhập:
                if (float.IsNegative(fixedAssetCategory.depreciation_rate) || float.IsNaN(fixedAssetCategory.depreciation_rate))
                {
                    errorData.Add("DepreRate", "");

                }
                //1.4 Thông tin thời gain sử dụng tài sản bắt buộc nhập:
                if (fixedAssetCategory.life_time < 0)
                {
                    errorData.Add("LifeTime", Resources.ResourceVN.CategoryLifetimeValidate);

                }
                if (CheckDuplicateCode(fixedAssetCategory.fixed_asset_category_code, fixedAssetCategory.fixed_asset_category_id, 1) == true)
                {
                    errorData.Add("AssetCategoryCode2", Resources.ResourceVN.CategoryCodeDuplicateValidate);
                }
                if (errorData.Count > 0)
                {
                    error.UserMsg = Resources.ResourceVN.Error_Validate;
                    error.Data = errorData;
                    return BadRequest(error);
                }
                var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
                //1. Khoi tao ket noi voi mariadb
                var sqlConnection = new MySqlConnection(connectionString);
                var sqlCommand = $"UPDATE fixed_asset_category SET fixed_asset_category_code=@CategoryCode, fixed_asset_category_name=@CategoryName, depreciation_rate=@DepreRate, life_time=@LifeTime WHERE fixed_asset_category_id=@CategoryId";
                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", categoryId);
                parameters.Add("@CategoryCode", fixedAssetCategory.fixed_asset_category_code);
                parameters.Add("@CategoryName", fixedAssetCategory.fixed_asset_category_name);
                parameters.Add("@DepreRate", fixedAssetCategory.depreciation_rate);
                parameters.Add("@LifeTime", fixedAssetCategory.life_time);
                var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
                return StatusCode(201, res);
            } catch (Exception ex)
            {
                var error = new ErrorService();
                error.DevMsg = ex.Message;
                error.UserMsg = Resources.ResourceVN.Error_Exception;
                error.Data = ex.Data;
                return StatusCode(500, error);
                throw;
            }
        }
        /// <summary>
        /// Xóa bản ghi theo id
        /// </summary>
        [HttpDelete("{categoryId}")]
        public IActionResult delete(Guid categoryId)
        {
            try
            {
                var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
                //1. Khởi tạo kết nối với MariaDB
                var sqlConnection = new MySqlConnection(connectionString);
                var sqlCommand = $"DELETE FROM fixed_asset_category WHERE fixed_asset_category_id=@CategoryId";
                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", categoryId);
                // Thực hiện truy vấn
                var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
                return Ok(res);
            } catch (Exception ex)
            {
                // Xử lý exception khi có lỗi
                var error = new ErrorService();
                error.DevMsg = ex.Message;
                error.UserMsg = Resources.ResourceVN.Error_Exception;
                error.Data = ex.Data;
                return StatusCode(500, error);
                throw;
            }
        }

    }
}
