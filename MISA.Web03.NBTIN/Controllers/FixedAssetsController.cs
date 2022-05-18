using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Web03.NBTIN.Entities;
using MySqlConnector;
using System.Web.Http.Cors;

namespace MISA.Web03.NBTIN.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [EnableCors(origins: "https://localhost:7115/api/v1/FixedAssets", headers: "*", methods: "*")]
    public class FixedAssetsController : ControllerBase
    {
        /// <summary>
        /// Lấy toàn bộ danh sách tài sản
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
                var sqlCommand = $"SELECT * FROM fixed_asset";
                //2.2. Thực hiện lấy dữ liệu
                var fixedAssets = sqlConnection.Query<FixedAsset>(sql: sqlCommand);

                //Trả kết quả cho client

                return Ok(fixedAssets);
            }
            catch (Exception ex)
            {
                //Xử lý khi có lỗi xảy ra 
                var error = new ErrorService();
                error.DevMsg = ex.Message;
                error.UserMsg = Resources.ResourceVN.Error_Exception;
                error.Data = ex.Data;
                return StatusCode(500, error);
                throw;
            }
        }
        /// <summary>
        /// Lấy bản ghi tài sản theo id truyền vào
        /// </summary>
        /// <param name="fixedAssetId"></param>
        /// <returns></returns>
        [HttpGet("{assetId}")]
        public IActionResult GetById(Guid assetId)
        {
            // Khởi tạo kết nối với database
            var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
            //1. Khởi tạo kết nối với MariaDB
            var sqlConnection = new MySqlConnection(connectionString);
            //2. Lấy dữ liệu
            //2.1. Câu lệnh truy vấn dữ liệu
            var sqlCommand = $"SELECT * FROM fixed_asset WHERE AssetId = @FixedAssetId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@FixedAssetId", assetId);
            //2.2. Thực hiện lấy dữ liệu
            var fixedAsset = sqlConnection.QueryFirstOrDefault<object>(sql: sqlCommand, param: parameters);


            //Trả kết quả cho client

            return Ok(fixedAsset);
        }
        /// <summary>
        /// Check trùng mã hay không
        /// sửa: 0, thêm: 1
        /// </summary>
        /// <param name="fixedAssetId"></param>
        /// <param name="fixedAssetCode"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private bool CheckDuplicateCode(string fixedAssetCode, Guid fixedAssetId, int mode)
        {
            var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
            var sqlConnection = new MySqlConnection(connectionString);
            var sqlCheck = "";

            if (mode == 1)
            {
                sqlCheck = $"SELECT * FROM fixed_asset WHERE AssetCode = @FixedAssetCode";

            }
            else if (mode == 0)
            {
                sqlCheck = $"SELECT * FROM fixed_asset WHERE AssetCode = @FixedAssetCode AND AssetId <> @FixedAssetId";
            }

            var parameters = new DynamicParameters();
            parameters.Add("@FixedAssetId", fixedAssetId);
            parameters.Add("@FixedAssetCode", fixedAssetCode);
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

        public IActionResult Post([FromBody] FixedAsset fixedAsset)
        {
            try
            {
                var error = new ErrorService();
                var errorData = new Dictionary<string, string>();
                fixedAsset.AssetId = Guid.NewGuid();

                // Khởi tạo kết nối với Database

                //1. Valiate dữ liệu: 
                //1.1 Thông tin mã loại tài sản bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.AssetCode))
                {
                    errorData.Add("AssetCode", Resources.ResourceVN.AssetCodeValidate);

                }
                //1.2 Thông tin tên loại tài sản bắt buộc nhập:
                if (string.IsNullOrEmpty(fixedAsset.AssetName))
                {
                    errorData.Add("AssetName", Resources.ResourceVN.AssetNameValidate);

                }
                //1.3 Thông tin mã bộ phận sử dụng bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.DepartmentCode))
                {
                    errorData.Add("DepartmentCode", Resources.ResourceVN.DepartmentCodeValidate);

                }
                //1.4 Thông tin tên bộ phận sử dụng bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.DepartmentName))
                {
                    errorData.Add("DepartmentName", Resources.ResourceVN.DepartmentNameValidate);

                }
                //1.5 Thông tin mã loại tài sản bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.FixedAssetCategoryCode))
                {
                    errorData.Add("FixedAssetCategoryCode", Resources.ResourceVN.CategoryCodeValidate);

                }
                //1.6 Thông tin tên loại tài sản bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.FixedAssetCategoryName))
                {
                    errorData.Add("FixedAssetCategoryName", Resources.ResourceVN.CategoryNameValidate);

                }
                //1.7 Thông tin nguyên giá bắt buộc lớn hơn 0   
                if (fixedAsset.Cost <= 0)
                {
                    errorData.Add("Cost", Resources.ResourceVN.CostValidate);

                }
                //1.8 Thông tin số lượng bắt buộc lớn hơn 0: 
                if (fixedAsset.Quantity <= 0)
                {
                    errorData.Add("Quantity", Resources.ResourceVN.QuantityValidate);

                }
                //1.8 Thông tin tỷ lệ hao mòn bắt buộc lớn hơn 0: 
                if (fixedAsset.DepreciationRate <= 0)
                {
                    errorData.Add("DepreciationRate", Resources.ResourceVN.DepreciationRateValidate);

                }
                //1.9 Thông tin năm theo dõi bắt buộc lớn hơn 0: 
                if (fixedAsset.TrackedYear <= 0 || fixedAsset.TrackedYear > 2022)
                {
                    errorData.Add("DepreciationRate", Resources.ResourceVN.TrackedYearValidate);

                }
                if (CheckDuplicateCode(fixedAsset.AssetCode, fixedAsset.AssetId, 1))
                {
                    errorData.Add("AssetCode2", Resources.ResourceVN.AssetCodeDuplicateValidate);
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

                var sqlCommand = $"INSERT INTO fixed_asset (AssetID, AssetCode, AssetName, DepartmentId, DepartmentCode, DepartmentName, FixedAssetCategoryId, FixedAssetCategoryCode, FixedAssetCategoryName, PurchaseDate, Cost, Quantity, DepreciationRate, TrackedYear, LifeTime, ProductionYear) VALUES (@AssetID, @AssetCode, @AssetName, @DepartmentId, @DepartmentCode, @DepartmentName, @FixedAssetCategoryId, @FixedAssetCategoryCode, @FixedAssetCategoryName, @PurchaseDate, @Cost, @Quantity, @DepreciationRate, @TrackedYear, @LifeTime, @ProductionYear)";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@AssetId", fixedAsset.AssetId);
                parameters.Add("@AssetCode", fixedAsset.AssetCode);
                parameters.Add("@AssetName", fixedAsset.AssetName);
                parameters.Add("@DepartmentId", fixedAsset.DepartmentId);
                parameters.Add("@DepartmentCode", fixedAsset.DepartmentCode);
                parameters.Add("@DepartmentName", fixedAsset.DepartmentName);
                parameters.Add("@FixedAssetCategoryId", fixedAsset.FixedAssetCategoryId);
                parameters.Add("@FixedAssetCategoryCode", fixedAsset.FixedAssetCategoryCode);
                parameters.Add("@FixedAssetCategoryName", fixedAsset.FixedAssetCategoryName);
                parameters.Add("@PurchaseDate", fixedAsset.PurchaseDate);
                parameters.Add("@Cost", fixedAsset.Cost);
                parameters.Add("@Quantity", fixedAsset.Quantity);
                parameters.Add("@DepreciationRate", fixedAsset.DepreciationRate);
                parameters.Add("@TrackedYear", fixedAsset.TrackedYear);
                parameters.Add("@LifeTime", fixedAsset.LifeTime);
                parameters.Add("@ProductionYear", fixedAsset.ProductionYear);

                var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
                return StatusCode(201, res);
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
        /// Hàm sửa theo category id
        /// CreatedBy: NBTin (05/05/2022)
        /// </summary>
        [HttpPut("{assetId}")]
        public IActionResult Put(FixedAsset fixedAsset, Guid assetId)
        {
            try
            {
                var error = new ErrorService();
                var errorData = new Dictionary<string, string>();
                //1. Valiate dữ liệu: 
                //1.1 Thông tin mã tài sản bắt buộc nhập:
                if (string.IsNullOrEmpty(fixedAsset.AssetName))
                {
                    errorData.Add("AssetName", Resources.ResourceVN.AssetNameValidate);

                }
                //1.2 Thông tin tên tài sản bắt buộc nhập:
                if (string.IsNullOrEmpty(fixedAsset.AssetName))
                {
                    errorData.Add("AssetName", Resources.ResourceVN.AssetNameValidate);

                }
                //1.3 Thông tin mã bộ phận sử dụng bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.DepartmentCode))
                {
                    errorData.Add("DepartmentCode", Resources.ResourceVN.DepartmentCodeValidate);

                }
                //1.4 Thông tin tên bộ phận sử dụng bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.DepartmentName))
                {
                    errorData.Add("DepartmentName", Resources.ResourceVN.DepartmentNameValidate);

                }
                //1.5 Thông tin mã loại tài sản bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.FixedAssetCategoryCode))
                {
                    errorData.Add("FixedAssetCategoryCode", Resources.ResourceVN.CategoryCodeValidate);

                }
                //1.6 Thông tin tên loại tài sản bắt buộc nhập: 
                if (string.IsNullOrEmpty(fixedAsset.FixedAssetCategoryName))
                {
                    errorData.Add("FixedAssetCategoryName", Resources.ResourceVN.CategoryNameValidate);

                }
                //1.7 Thông tin nguyên giá bắt buộc lớn hơn 0   
                if (fixedAsset.Cost <= 0)
                {
                    errorData.Add("Cost", Resources.ResourceVN.CostValidate);

                }
                //1.8 Thông tin số lượng bắt buộc lớn hơn 0: 
                if (fixedAsset.Quantity <= 0)
                {
                    errorData.Add("Quantity", Resources.ResourceVN.QuantityValidate);

                }
                //1.8 Thông tin tỷ lệ hao mòn bắt buộc lớn hơn 0: 
                if (fixedAsset.DepreciationRate <= 0)
                {
                    errorData.Add("DepreciationRate", Resources.ResourceVN.DepreciationRateValidate);

                }
                //1.9 Thông tin năm theo dõi bắt buộc lớn hơn 0: 
                if (fixedAsset.TrackedYear <= 0 || fixedAsset.TrackedYear > 2022)
                {
                    errorData.Add("DepreciationRate", Resources.ResourceVN.TrackedYearValidate);

                }
                // Check trùng mã khác trong table hay ko
                if (CheckDuplicateCode(fixedAsset.AssetCode, fixedAsset.AssetId, 0))
                {
                    errorData.Add("AssetCode2", Resources.ResourceVN.AssetCodeDuplicateValidate);
                }
                // Kiểm tra nếu danh sách lỗi là khác null thì in ra thông báo
                if (errorData.Count > 0)
                {
                    error.UserMsg = Resources.ResourceVN.Error_Validate;
                    error.Data = errorData;
                    return BadRequest(error);
                }
                var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
                //1. Khoi tao ket noi voi mariadb
                var sqlConnection = new MySqlConnection(connectionString);
                var sqlCommand = $"UPDATE fixed_asset SET AssetCode=@AssetCode, AssetName=@AssetName, DepartmentCode=@DepartmentCode,DepartmentName=@DepartmentName" +
                    $",FixedAssetCategoryCode=@FixedAssetCategoryCode,FixedAssetCategoryName=@FixedAssetCategoryName" +
                    $",PurchaseDate=@PurchaseDate,Cost=@Cost,Quantity=@Quantity,DepreciationRate=@DepreciationRate,TrackedYear=@TrackedYear" +
                    $",LifeTime=@LifeTime,ProductionYear=@ProductionYear WHERE AssetId=@AssetId";
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@AssetId", assetId);
                parameters.Add("@AssetCode", fixedAsset.AssetCode);
                parameters.Add("@AssetName", fixedAsset.AssetName);
                parameters.Add("@DepartmentCode", fixedAsset.DepartmentCode);
                parameters.Add("@DepartmentName", fixedAsset.DepartmentName);
                parameters.Add("@FixedAssetCategoryCode", fixedAsset.FixedAssetCategoryCode);
                parameters.Add("@FixedAssetCategoryName", fixedAsset.FixedAssetCategoryName);
                parameters.Add("@PurchaseDate", fixedAsset.PurchaseDate);
                parameters.Add("@Cost", fixedAsset.Cost);
                parameters.Add("@Quantity", fixedAsset.Quantity);
                parameters.Add("@DepreciationRate", fixedAsset.DepreciationRate);
                parameters.Add("@TrackedYear", fixedAsset.TrackedYear);
                parameters.Add("@LifeTime", fixedAsset.LifeTime);
                parameters.Add("@ProductionYear", fixedAsset.ProductionYear);
                var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
                return StatusCode(201, res);
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
        /// Xóa bản ghi theo id
        /// </summary>
        [HttpDelete("{assetId}")]
        public IActionResult delete(Guid assetId)
        {
            try
            {
                var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
                //1. Khởi tạo kết nối với MariaDB
                var sqlConnection = new MySqlConnection(connectionString);
                var sqlCommand = $"DELETE FROM fixed_asset WHERE AssetId=@assetId";
                var parameters = new DynamicParameters();
                parameters.Add("@AssetId", assetId);
                // Thực hiện truy vấn
                var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
                return Ok(res);
            }
            catch (Exception ex)
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
        /// <summary>
        /// Lấy mã tài sản mới 
        /// </summary>
        /// <returns></returns>
        [HttpGet("NewAssetCode")]
        public IActionResult GetNewCode()
        {
            var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
            //1. Khởi tạo kết nối với MariaDB
            var sqlConnection = new MySqlConnection(connectionString);
            // Truy vấn ra danh sách các mã có tiền tố là TS
            string sqlCommand = $"SELECT AssetCode FROM fixed_asset WHERE AssetCode IS NOT NULL AND AssetCode LIKE '%TS%' ORDER BY  LENGTH(AssetCode) DESC, AssetCode DESC";
            // Lấy bản ghi cuối cùng (giá trị gần nhất)
            var AssetCode = sqlConnection.QueryFirstOrDefault<string>(sqlCommand);

            int currentMax = 0;
            int codeValue = int.Parse(AssetCode.Substring(2).ToString());
            if (currentMax < codeValue)
            {
                currentMax = codeValue;
            }
            string newAssetCode = "TS" + (currentMax + 1);
            return Ok(newAssetCode);
        }
    }
}
