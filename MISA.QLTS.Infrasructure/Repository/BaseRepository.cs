using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Exceptions;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MISA.QLTS.CORE.MISAAttribute;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.QLTS.Infrasructure.Repository
{
    public class BaseRepository<T> : IBaseRepository<T>
    {
        IConfiguration _configuration;
        readonly string _connectionString = string.Empty;
        protected MySqlConnection _sqlConnection;
        private string _tableName;
        protected List<string> ValidateErrorMsgs;
        public BaseRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("NBTIN");
            _sqlConnection = new MySqlConnection(_connectionString);
            _tableName = typeof(T).Name;

        }
        /// <summary>
        /// Hàm check mã trùng
        /// </summary>
        /// <param name="entityId">Id của bản ghi</param>
        /// <param name="entityCode">Mã của bản ghi</param>
        /// <param name="mode">Trạng thái khi thực hiện validate (Thêm, sửa)</param>
        /// <returns></returns>
        public bool CheckCodeDuplicate(Guid entityId, string entityCode, int mode)
        {
            // Khởi tạo câu sql
            var sqlCheck = "";
            // Nếu trạng thái là thêm
            if (mode == 1)
            {
                // Câu lệnh check mã trùng ở trong database
                sqlCheck = $"SELECT * FROM {_tableName} WHERE {_tableName}Code = @{_tableName}Code";

            }
            else if (mode == 0) // Nếu trạng thái là sửa
            {
                // Câu lệnh check mã trùng ở trong database
                sqlCheck = $"SELECT * FROM {_tableName} WHERE {_tableName}Code = @{_tableName}Code AND {_tableName}Id <> @{_tableName}Id";
            }
            // Add các giá trị của biến vào parameters
            var parameters = new DynamicParameters();
            parameters.Add($"@{_tableName}Id", entityId);
            parameters.Add($"@{_tableName}Code", entityCode);
            var res = _sqlConnection.QueryFirstOrDefault<object>(sqlCheck, parameters);
            // Nếu truy vấn trả về kết quả là null (Không có bản ghi có trùng mã) thì return true (Không bị trùng mã)
            if (res != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Thực hiện xóa bản ghi theo id truyền vào
        /// </summary>
        /// <param name="entityId">Id của đối tượng</param>
        /// <returns>Số bản ghi bị xóa</returns>
        public int Delete(Guid entityId)
        {
            var sqlCommand = $"DELETE FROM {_tableName} WHERE {_tableName}Id=@EntityId";
            var parameters = new DynamicParameters();
            parameters.Add("@EntityId", entityId);
            // Thực hiện truy vấn
            var res = _sqlConnection.Execute(sql: sqlCommand, param: parameters);
            return res;
        }
        /// <summary>
        /// Lấy tất cả bản ghi
        /// </summary>
        /// <returns>Mảng tất cả bản ghi</returns>
        public List<T> Get()
        {
            // Khai báo câu truy vấn
            var sqlCommand = $"SELECT * FROM {_tableName}";
            var entities = _sqlConnection.Query<T>(sqlCommand);
            return entities.ToList();
        }
        /// <summary>
        /// Lấy bản ghi theo id truyền vào
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns>Bản ghi</returns>
        public T GetById(Guid entityId)
        {
            var sqlCommand = $"SELECT * FROM {_tableName} WHERE {_tableName}Id = @EntityId";
            var parameter = new DynamicParameters();
            parameter.Add("@EntityId", entityId);
            var entity = _sqlConnection.QueryFirstOrDefault<T>(sql: sqlCommand, param: parameter);
            //Trả kết quả cho client

            return entity;
        }
        /// <summary>
        /// Check 1 kí tự có phải là số hay không
        /// </summary>
        /// <param name="pText"></param>
        /// <returns></returns>
        public bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*.?[0-9]+$");
            return regex.IsMatch(pText);
        }
        /// <summary>
        /// Thực hiện lấy mã mới
        /// </summary>
        /// <returns>Mã mới</returns>
        public string GetNewCode()
        {
            // câu lệnh sql lấy mã tài sản theo ngày giảm dần;
            string sqlCommand = $"SELECT {_tableName}Code FROM {_tableName} ORDER BY CreatedDate DESC";

            // Lấy mã tài sản gần nhất
            var AssetCode = _sqlConnection.QueryFirstOrDefault<string>(sql: sqlCommand);
            // khai báo mã tài sản mới trả vê
            var newAssetCode = "";

            if (string.IsNullOrEmpty(AssetCode))
            {
                newAssetCode = "TS00001";
            }

            else
            {
                // trả về mảng chuỗi và số riêng biệt.
                string[] output = Regex.Matches(AssetCode, "[0-9]+|[^0-9]+")
                .Cast<Match>()
                .Select(match => match.Value)
                .ToArray();

                // khai báo giá trị sau khi cộng
                int currentMax = 0;

                var valueAssetCode = "";
                var numberAssetCode = "";

                // lưu các giá trị trước đó
                var saveValue = "";
                var newAssetCodeValue = "";

                // nếu mảng trả về lớn hơn 1 phần
                for (var i = 0; i < output.Length; i++)
                {
                    // kiêm tra xem phần tử cuố là số hay chuỗi
                    if (IsNumber(output[output.Length - 1]) == false)
                    {
                        newAssetCode = saveValue + output[i] + 1;

                    }
                    else
                    {

                        newAssetCodeValue = saveValue;
                    }

                    saveValue += output[i];

                }
                // Kiểm tra xem có giá trị lưu trước đó không?
                if (saveValue != "")
                {
                    if (IsNumber(output[output.Length - 1]) == true)
                    {
                        //chuyển chuỗi về dạng số nếu có số 0;
                        var partNumber = int.Parse(output[output.Length - 1]);
                        if (currentMax < partNumber)
                        {
                            // giá trị được tăng lên 1
                            currentMax = partNumber + 1;
                        }

                        // Ghép chuỗi;
                        newAssetCode = newAssetCodeValue + currentMax;
                        // Nếu chuỗi hiện tại mà nhỏ hơn chuỗi lấy về từ sql
                        if (newAssetCode.Length < AssetCode.Length)
                        {
                            // kiểm tra chừng nào chuỗi hiện tại mà nhỏ hơn chuỗi lấy về từ sql
                            while (newAssetCode.Length < AssetCode.Length)
                            {
                                string[] newOutput = Regex.Matches(newAssetCode, "[0-9]+|[^0-9]+")
                                   .Cast<Match>()
                                   .Select(match => match.Value)
                                   .ToArray();
                                // chuỗi kí tự
                                valueAssetCode = newAssetCodeValue;
                                // chuối số
                                numberAssetCode = newOutput[newOutput.Length - 1];
                                // chèn 0 vào giữa chuỗi kí tự với chuỗi số
                                newAssetCode = valueAssetCode + "0" + numberAssetCode;
                            }

                        }
                    }
                    else
                    {
                        newAssetCode = saveValue + 1;
                    }
                }
            }

            return newAssetCode;
        }

        public List<T> getPaging(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Thêm bản ghi base
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Insert(T entity)
        {
            var columnNames = "";
            var columnValues = "";
            var parameter = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            
            int mode = 1;
            foreach(var prop in properties)
            {
                // Lấy tên của prop
                var propName = prop.Name;
                // Lấy giá trị thêm vào
                var propValue = prop.GetValue(entity);
                parameter.Add($"@{propName}", $"{propValue}");
                // Kiểu dữ liệu của prop
                var propType = prop.PropertyType;
                var isNotInsert = prop.IsDefined(typeof(NotInsertColumn), true);
                if (isNotInsert)
                {
                    continue;
                }
                var isPrimaryKey = prop.IsDefined(typeof(PrimaryKey), true);
                if(isPrimaryKey == true && propType == typeof(Guid))
                {
                    prop.SetValue(entity, Guid.NewGuid());
                }
                
                propValue = prop.GetValue(entity);
                // Bổ sung prop vào chuỗi truy vấn
                columnNames += $"{propName},";
                columnValues += $"@{propName},"; 
            }
            columnNames = columnNames.Remove(columnNames.Length - 1,1);
            columnValues = columnValues.Remove(columnValues.Length - 1, 1);
            var sqlCommand = $"INSERT INTO {_tableName}({columnNames}) VALUES ({columnValues})";
            var rowBeAffects = _sqlConnection.Execute(sqlCommand, param: entity);
            return rowBeAffects;
        }
        
        /// <summary>
        /// Sửa bản ghi base
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(Guid id, T entity)
        {
            var columnNames = "";
            var columnValues = ""; 
            var parameter = new DynamicParameters();
            var properties = typeof(T).GetProperties();
            var sqlCommand = $"UPDATE {_tableName} SET ";
            foreach (var prop in properties)
            {
                // Check xem có phải key ko, key thì ko add vào param

                var isPrimaryKey = prop.IsDefined(typeof(PrimaryKey), true);
                if (isPrimaryKey == true)
                {
                    continue;
                }
                var isNotInsertColumn = prop.IsDefined(typeof(NotInsertColumn), true);
                if(isNotInsertColumn == true)
                {
                    continue;
                }
                // Lấy tên của prop
                var propName = prop.Name;
                // Lấy giá trị thêm vào
                var propValue = prop.GetValue(entity);
                parameter.Add($"@{propName}", $"{propValue}");
                // Kiểu dữ liệu của prop
                var propType = prop.PropertyType;
               
                propValue = prop.GetValue(entity);
                // Bổ sung prop vào chuỗi truy vấn
                sqlCommand += $"{propName}=@{propName},"; 
            }
            sqlCommand = sqlCommand.Remove(sqlCommand.Length - 1, 1);
            sqlCommand += $" WHERE {_tableName}Id='{id}'";
            var rowBeAffects = _sqlConnection.Execute(sqlCommand, param: entity);
            return rowBeAffects;
        }

        
    }
}
