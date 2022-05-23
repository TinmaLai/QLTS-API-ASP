﻿using Microsoft.Extensions.Configuration;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Text.RegularExpressions;

namespace MISA.QLTS.Infrasructure.Repository
{
    public class FixedAssetRepository : BaseRepository<FixedAsset>, IFixedAssetRepository
    {
        IConfiguration _configuration;
        readonly string _connectionString = string.Empty;
        protected MySqlConnection _sqlConnection;
        public FixedAssetRepository(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("NBTIN");
            _sqlConnection = new MySqlConnection(_connectionString);
        }
        /// <summary>
        /// Check mã trùng
        /// </summary>
        /// <param name="fixedAssetId">Id tài sản</param>
        /// <param name="fixedAssetCode">Code tài sản</param>
        /// <param name="mode">Thêm = 0, sửa = 1</param>
        /// <returns></returns>
        public bool CheckCodeDuplicate(Guid fixedAssetId, string fixedAssetCode, int mode)
        {
            
            var sqlCheck = "";

            if (mode == 1)
            {
                sqlCheck = $"SELECT * FROM FixedAsset WHERE FixedAssetCode = @FixedAssetCode";

            }
            else if (mode == 0)
            {
                sqlCheck = $"SELECT * FROM FixedAsset WHERE FixedAssetCode = @FixedAssetCode AND FixedAssetId <> @FixedAssetId";
            }

            var parameters = new DynamicParameters();
            parameters.Add("@FixedAssetId", fixedAssetId);
            parameters.Add("@FixedAssetCode", fixedAssetCode);
            var res = _sqlConnection.QueryFirstOrDefault<object>(sqlCheck, parameters);

            if (res != null)
            {
                return true;
            }
            return false;
        }
        
        public bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*.?[0-9]+$");
            return regex.IsMatch(pText);
        }
        /// <summary>
        /// Thực hiện lấy mã mới
        /// </summary>
        /// <returns>Mã mới</returns>
        public string getNewCode()
        {
            // câu lệnh sql lấy mã tài sản theo ngày giảm dần;
            string sqlCommand = "SELECT FixedAssetCode FROM FixedAsset ORDER BY CreatedDate DESC";

            // Lấy mã tài sản gần nhất
            var AssetCode = _sqlConnection.QueryFirstOrDefault<string>(sql: sqlCommand);

            // trả về mảng chuỗi và số riêng biệt.
            string[] output = Regex.Matches(AssetCode, "[0-9]+|[^0-9]+")
            .Cast<Match>()
            .Select(match => match.Value)
            .ToArray();

            // khai báo giá trị sau khi cộng
            int currentMax = 0;

            var valueAssetCode = "";
            var numberAssetCode = "";
            // khai báo mã tài sản mới trả vê
            string newAssetCode = "";
            string saveValue = "";
            string newAssetCodeValue = "";
            var checkNumber = false;
            // nếu mảng trả về lớn hơn 1 phần
            // 111ABC
            for (var i = 0; i < output.Length; i++)
            {
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
                else newAssetCode = saveValue + 1;
            }
            return newAssetCode;
        }
        public List<FixedAsset> getPaging(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        public List<FixedAsset> Import(List<FixedAsset> fixedAssets)
        {
            foreach(var fixedAsset in fixedAssets)
            {
                Insert(fixedAsset);
            }
            return fixedAssets;
        }
        /// <summary>
        /// Xóa nhiều
        /// </summary>
        public int MultiDelete(Guid[] ids)
        {
            
            var stringDelete = "";
            foreach(var id in ids)
            {
                stringDelete += "'" + id + "',";
            }
            stringDelete = stringDelete.Remove(stringDelete.Length - 1,1);
            var sqlCommand = $"DELETE FROM FixedAsset WHERE FixedAssetId IN ({stringDelete})";
            var res = _sqlConnection.Execute(sqlCommand);
            return res;
        }

        public List<FixedAsset> Filter(string? filterContent, string? departmentName, string? fixedAssetCategoryName, int? pageSize, int? pageNumber)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@FilterContent", filterContent);
            parameters.Add("@DepartmentName", departmentName);
            parameters.Add("@FixedAssetCategoryName", fixedAssetCategoryName);
            var pageOffset = pageSize * (pageNumber - 1);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageOffset", pageOffset);

            var sqlCommand = $"SELECT * FROM FixedAsset";
            if (filterContent != null) sqlCommand += $" WHERE (FixedAssetName LIKE CONCAT('%',@FilterContent,'%') " +
                $"OR FixedAssetCode LIKE CONCAT('%',@FilterContent,'%')) AND";
            else sqlCommand += " WHERE";

            if (departmentName != null) sqlCommand += $" DepartmentName = @DepartmentName AND";
            else sqlCommand += $" 1 = 1 AND";

            if (fixedAssetCategoryName != null) sqlCommand += $" FixedAssetCategoryName = @FixedAssetCategoryName";
            else sqlCommand += $" 1 = 1";

            sqlCommand += $" ORDER BY CreatedDate DESC LIMIT @PageSize OFFSET @PageOffset";

            var fixedAssets = _sqlConnection.Query<FixedAsset>(sqlCommand,parameters);
            return fixedAssets.ToList();
        }
    }
}
