using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Infrasructure.Repository
{
    public class LicenseDetailRepository:BaseRepository<LicenseDetail>, ILicenseDetailRepository
    {
        IConfiguration _configuration;
        readonly string _connectionString = string.Empty;
        protected MySqlConnection _sqlConnection;
        public LicenseDetailRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("NBTIN");
            _sqlConnection = new MySqlConnection(_connectionString);
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
                sqlCheck = $"SELECT * FROM License WHERE LicenseCode = @LicenseCode";

            }
            else if (mode == 0) // Nếu trạng thái là sửa
            {
                // Câu lệnh check mã trùng ở trong database
                sqlCheck = $"SELECT * FROM License WHERE LicenseCode = @LicenseCode AND LicenseId <> @LicenseId";
            }
            // Add các giá trị của biến vào parameters
            var parameters = new DynamicParameters();
            parameters.Add($"@LicenseId", entityId);
            parameters.Add($"@LicenseCode", entityCode);
            var res = _sqlConnection.QueryFirstOrDefault<object>(sqlCheck, parameters);
            // Nếu truy vấn trả về kết quả là null (Không có bản ghi có trùng mã) thì return true (Không bị trùng mã)
            if (res != null)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Lấy ra 1 bản ghi có cả master, cả detail để bind vào form
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        public object GetLicenseInsertById(Guid licenseId)
        {
            var sqlGetLicenseMaster = "SELECT * FROM License WHERE LicenseId = @LicenseId";
            var parameter = new DynamicParameters();
            parameter.Add("@LicenseId", licenseId);
            var license = _sqlConnection.QueryFirstOrDefault<License>(sqlGetLicenseMaster, parameter);
            var sqlGetLicenseDetail = "SELECT ld.LicenseDetailId, ld.DetailJson, fa.FixedAssetId, fa.FixedAssetCode, fa.FixedAssetName, fa.DepartmentName,fa.Cost," +
                " fa.DepreciationPerYear,fa.ProductionYear FROM License l LEFT JOIN LicenseDetail ld ON l.LicenseId = " +
                "ld.LicenseId LEFT JOIN FixedAsset fa ON ld.FixedAssetId = fa.FixedAssetId" +
                " WHERE l.LicenseId = @LicenseId ";
            var detailAssets = _sqlConnection.Query<object>(sqlGetLicenseDetail, parameter);
            
            var res = new 
            {
                LicenseId = license.LicenseId,
                LicenseCode = license.LicenseCode,
                UseDate = license.UseDate,
                WriteUpdate = license.WriteUpdate,
                Description = license.Description,
                Total = license.Total,
                detailAssets = detailAssets.ToArray()
            };
            return res;
        }
        /// <summary>
        /// Thêm mảng license detail
        /// </summary>
        public object MultiInsert(LicenseDetail[] licenseDetails, Guid licenseId)
        {
            

            var count = 0;

            for (int i = 0; i < licenseDetails.Length;  i++)
            {
                licenseDetails[i].LicenseId = licenseId;
                licenseDetails[i].LicenseDetailId = Guid.NewGuid();
                int res = Insert(licenseDetails[i]);

                count += res;
                // Sinh Id không trùng cho licenseDetail
            }

            return count;
        }
        
        /// <summary>
        /// Sửa 1 bản ghi Json detail
        /// </summary>
        /// <param name="licenseInsert"></param>
        /// <returns></returns>
        public int UpdateLicenseDetail(Guid licenseDetailId, string detailJson)
        {
            var sqlUpdate = "UPDATE LicenseDetail SET DetailJson = @DetailJson WHERE LicenseDetailId = @LicenseDetailId";
            var parameter = new DynamicParameters();
            parameter.Add("@DetailJson", detailJson);
            parameter.Add("@LicenseDetailId", licenseDetailId);
            var res = _sqlConnection.Execute(sqlUpdate, parameter);
            return res; 
        }
        
        /// <summary>
        /// Lấy ra bộ phận sử dụng và detail json từ detail json
        /// </summary>
        /// <param name="licenseDetailId"></param>
        /// <returns></returns>
        public object GetMoneySource(Guid licenseDetailId)
        {
            var sqlGet = $"SELECT ld.DetailJson, fa.DepartmentName FROM LicenseDetail ld LEFT JOIN FixedAsset fa ON ld.FixedAssetId " +
                $"= fa.FixedAssetId WHERE ld.LicenseDetailId = @LicenseDetailId";
            var parameter = new DynamicParameters();
            parameter.Add("@LicenseDetailId", licenseDetailId);
            var res =  _sqlConnection.QueryFirstOrDefault<object>(sqlGet, parameter);
            return res;
        }
        /// <summary>
        /// Lấy ra 1 mảng detail từ 1 id bảng master
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        public List<object> GetDetailAssets(Guid licenseId)
        {
            var sqlCommand = "SELECT ld.LicenseDetailId, fa.FixedAssetId, fa.FixedAssetCode, fa.FixedAssetName, fa.DepartmentName,fa.Cost," +
                " fa.DepreciationPerYear FROM License l LEFT JOIN LicenseDetail ld ON l.LicenseId = " +
                "ld.LicenseId LEFT JOIN FixedAsset fa ON ld.FixedAssetId = fa.FixedAssetId" +
                " WHERE l.LicenseId = @LicenseId ";
            var parameter = new DynamicParameters();
            parameter.Add("@LicenseId", licenseId);
            var res = _sqlConnection.Query<object>(sqlCommand, parameter);
            return res.ToList();
        }
        
    }
}
