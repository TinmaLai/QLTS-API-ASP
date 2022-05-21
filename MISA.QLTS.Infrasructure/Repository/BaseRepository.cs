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
        public bool CheckCodeDuplicate(Guid entityId, string entityCode, int mode)
        {
            
            var sqlCheck = "";

            if (mode == 1)
            {
                sqlCheck = $"SELECT * FROM FixedAsset WHERE {_tableName}Code = @{_tableName}Code";

            }
            else if (mode == 0)
            {
                sqlCheck = $"SELECT * FROM FixedAsset WHERE {_tableName}Code = @{_tableName}Code AND {_tableName}Id <> @{_tableName}Id";
            }

            var parameters = new DynamicParameters();
            parameters.Add($"@{_tableName}Id", entityId);
            parameters.Add($"@{_tableName}Code", entityCode);
            var res = _sqlConnection.QueryFirstOrDefault<object>(sqlCheck, parameters);

            if (res != null)
            {
                return true;
            }
            return false;
        }

        public int Delete(Guid entityId)
        {
            var sqlCommand = $"DELETE FROM {_tableName} WHERE FixedAssetId=@FixedAssetId";
            var parameters = new DynamicParameters();
            parameters.Add("@FixedAssetId", entityId);
            // Thực hiện truy vấn
            var res = _sqlConnection.Execute(sql: sqlCommand, param: parameters);
            return res;
        }

        public List<T> Get()
        {
            // Khai báo câu truy vấn
            var sqlCommand = $"SELECT * FROM {_tableName} ORDER BY {_tableName}Code";
            var entities = _sqlConnection.Query<T>(sqlCommand);
            return entities.ToList();
        }

        public T GetById(Guid entityId)
        {
            var sqlCommand = $"SELECT * FROM {_tableName} WHERE {_tableName}Id = @EntityId";
            var parameter = new DynamicParameters();
            parameter.Add("@EntityId", entityId);
            var entity = _sqlConnection.QueryFirstOrDefault<T>(sql: sqlCommand, param: parameter);
            //Trả kết quả cho client

            return entity;
        }

        public string getNewCode()
        {
            throw new NotImplementedException();
        }

        public List<T> getPaging(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }
        

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
