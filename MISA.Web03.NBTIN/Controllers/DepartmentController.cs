using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.Web03.NBTIN.Entities;
using MySqlConnector;

namespace MISA.Web03.NBTIN.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        /// <summary>
        /// Lấy tất cả dữ liệu từ bảng department
        /// </summary>
        /// <returns></returns>
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
                var sqlCommand = $"SELECT * FROM department";
                //2.2. Thực hiện lấy dữ liệu
                var departments = sqlConnection.Query<Department>(sql: sqlCommand);

                //Trả kết quả cho client
                return Ok(departments);
            }
            catch (Exception ex)
            {
                // Xử lý khi có lỗi: Trả thông báo, StatusCode
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
        /// <param name="departmentId"></param>
        /// <returns></returns>
        [HttpGet("{departmentId}")]
        public IActionResult GetById(Guid departmentId)
        {
            // Khởi tạo kết nối với database
            var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
            //1. Khởi tạo kết nối với MariaDB
            var sqlConnection = new MySqlConnection(connectionString);
            //2. Lấy dữ liệu
            //2.1. Câu lệnh truy vấn dữ liệu
            var sqlCommand = $"SELECT * FROM department WHERE department_id = @DepartmentId";
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@DepartmentId", departmentId);
            //2.2. Thực hiện lấy dữ liệu
            var department = sqlConnection.QueryFirstOrDefault<object>(sql: sqlCommand, param: parameters);


            //Trả kết quả cho client

            return Ok(department);
        }
        /// <summary>
        /// Check trùng mã hay không
        /// sửa: 0, thêm: 1
        /// </summary>
        /// <param name="departmentId">Id của department để tìm bản ghi khi chọn sửa(ko trùng với chính nó)</param>
        /// <param name="departmentCode">Check mã trùng</param>
        /// <param name="mode">Trạng thái</param>
        /// <returns></returns>
        private bool CheckDuplicateCode(string departmentCode, Guid departmentId, int mode)
        {
            var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
            var sqlConnection = new MySqlConnection(connectionString);
            var sqlCheck = "";

            if (mode == 1)
            {
                sqlCheck = $"SELECT * FROM department WHERE department_code = @DepartmentCode";

            }
            else if (mode == 0)
            {
                sqlCheck = $"SELECT * FROM department WHERE department_code = @DepartmentCode AND department_id <> @DepartmentId";
            }

            var parameters = new DynamicParameters();
            parameters.Add("@DepartmentId", departmentId);
            parameters.Add("@DepartmentCode", departmentCode);
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

        public IActionResult Post([FromBody] Department department)
        {
            try
            {
                var error = new ErrorService();
                var errorData = new Dictionary<string, string>();
                department.department_id = Guid.NewGuid();
                department.organization_id = Guid.NewGuid();
                // Khởi tạo kết nối với Database

                //1. Valiate dữ liệu: 
                //1.1 Thông tin mã phòng ban bắt buộc nhập: 
                if (string.IsNullOrEmpty(department.department_code))
                {
                    errorData.Add("DepartmentCode", Resources.ResourceVN.DepartmentCodeValidate);

                }
                //1.2 Thông tin tên phòng ban bắt buộc nhập:
                if (string.IsNullOrEmpty(department.department_name))
                {
                    errorData.Add("DepartmentName", Resources.ResourceVN.DepartmentNameValidate);

                }
                //1.3 Mã sau không được trùng với các mã đã có trước đó:
                if (CheckDuplicateCode(department.department_code, department.department_id, 1) == true)
                {
                    errorData.Add("DepartmentCode2", Resources.ResourceVN.DuplicateDepartmentCodeValidate);
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

                var sqlCommand = $"INSERT INTO department(department_id,department_code,department_name,organization_id) VALUES (@DepartmentId,@DepartmentCode,@DepartmentName,@OrgaId)";

                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@DepartmentId", department.department_id);
                parameters.Add("@DepartmentCode", department.department_code);
                parameters.Add("@DepartmentName", department.department_name);
                parameters.Add("@OrgaId", department.organization_id);
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
        [HttpPut("{departmentId}")]
        public IActionResult Put(Department department, Guid departmentId)
        {
            try
            {
                var error = new ErrorService();
                var errorData = new Dictionary<string, string>();
                // Khởi tạo kết nối với Database

                //1. Valiate dữ liệu: 
                //1.1 Thông tin mã phòng ban bắt buộc nhập: 
                if (string.IsNullOrEmpty(department.department_code))
                {
                    errorData.Add("AssetCategoryCode", Resources.ResourceVN.DepartmentCodeValidate);

                }
                //1.2 Thông tin tên phòng ban bắt buộc nhập:
                if (string.IsNullOrEmpty(department.department_name))
                {
                    errorData.Add("AssetCategoryName", Resources.ResourceVN.DepartmentNameValidate);

                }
                //1.3 Mã sau không được trùng với các mã đã có trước đó:
                if (CheckDuplicateCode(department.department_code, department.department_id, 1) == true)
                {
                    errorData.Add("AssetCategoryCode2", Resources.ResourceVN.DuplicateDepartmentCodeValidate);
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
                var sqlCommand = $"UPDATE department SET department_code=@DepartmentCode, department_name=@DepartmentName WHERE department_id=@DepartmentId";
                var parameters = new DynamicParameters();
                parameters.Add("@DepartmentId", departmentId);
                parameters.Add("@DepartmentCode", department.department_code);
                parameters.Add("@DepartmentName", department.department_name);
                var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
                return StatusCode(201, res);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có lỗi: Trả ra StatusCode, thông báo lỗi
                var error = new ErrorService();
                error.DevMsg = ex.Message;
                error.UserMsg = Resources.ResourceVN.Error_Exception;
                error.Data = ex.Data;
                return StatusCode(500, error);
                throw;
            }
        }
        /// <summary>
        /// Sửa bản ghi theo id
        /// <param name="departmentId"/></param>
        /// </summary>
        [HttpDelete("{departmentId}")]
        public IActionResult delete(Guid departmentId)
        {
            try
            {
                var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
                //1. Khởi tạo kết nối với MariaDB
                var sqlConnection = new MySqlConnection(connectionString);
                var sqlCommand = $"DELETE FROM department WHERE department_id=@DepartmentId";
                var parameters = new DynamicParameters();
                parameters.Add("@DepartmentId", departmentId);
                // Thực hiện truy vấn
                var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
                if(res == 0)
                {
                    return BadRequest(Resources.ResourceVN.NotValidId);
                }
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
    }
}
