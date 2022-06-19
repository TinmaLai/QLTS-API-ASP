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
    public class LicenseInsertRepository: BaseRepository<LicenseInsert>, ILicenseInsertRepository
    {
        IConfiguration _configuration;
        readonly string _connectionString = string.Empty;
        protected MySqlConnection _sqlConnection;
        public LicenseInsertRepository(IConfiguration configuration) : base(configuration)
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
        public object MultiInsert(LicenseInsert licenseInsert)
        {
            //License license = new License();
            var parameter = new DynamicParameters();
            licenseInsert.LicenseId = Guid.NewGuid();

            var sqlInsertMaster = $"INSERT INTO License (LicenseId, LicenseCode, UseDate, WriteUpdate, Description, Total) VALUES " +
                "(@LicenseId, @LicenseCode, @UseDate, @WriteUpdate, @Description, @Total)";
            parameter.Add("@LicenseId", licenseInsert.LicenseId);
            parameter.Add("@LicenseCode", licenseInsert.LicenseCode);
            parameter.Add("@UseDate", licenseInsert.UseDate);
            parameter.Add("@WriteUpdate", licenseInsert.WriteUpdate);
            parameter.Add("@Description", licenseInsert.Description);
            parameter.Add("@Total", licenseInsert.Total);
            var masterRes = _sqlConnection.Execute(sqlInsertMaster, parameter);

            var count = 0;

            for (int i = 0; i < licenseInsert.licenseDetails.Length; i++)
            {
                // Sinh Id không trùng cho licenseDetail
                licenseInsert.licenseDetails[i].LicenseDetailId = Guid.NewGuid();

                var sqlInsertDetail = $"INSERT INTO LicenseDetail (LicenseDetailId, LicenseId, FixedAssetId, DetailJson) VALUES (@LicenseDetailId,@LicenseId,@FixedAssetId,@DetailJson)";

                parameter.Add("@FixedAssetId", licenseInsert.licenseDetails[i].FixedAssetId);
                parameter.Add("@LicenseId", licenseInsert.LicenseId);
                parameter.Add("@LicenseDetailId", licenseInsert.licenseDetails[i].LicenseDetailId);
                parameter.Add("@DetailJson", licenseInsert.licenseDetails[i].DetailJson);
                var res = _sqlConnection.Execute(sqlInsertDetail, parameter);
                count += res;
            }


            var lastRes = new
            {
                detail = count,
                masterRes = masterRes
            };
            return lastRes;
        }
        /// <summary>
        /// Sửa 1 bản ghi master - detail
        /// </summary>
        /// <param name="licenseInsert"></param>
        /// <returns></returns>
        public object UpdateLicenseInsert(LicenseInsert licenseInsert, Guid licenseId)
        {
            var parameter = new DynamicParameters();

            var sqlUpdateMaster = $"UPDATE License SET LicenseCode = @LicenseCode, UseDate = @UseDate, WriteUpdate = @WriteUpdate, " +
                $"Description = @Description, Total = @Total WHERE LicenseId = @LicenseId";
            parameter.Add("@LicenseId", licenseId);
            parameter.Add("@LicenseCode", licenseInsert.LicenseCode);
            parameter.Add("@UseDate", licenseInsert.UseDate);
            parameter.Add("@WriteUpdate", licenseInsert.WriteUpdate);
            parameter.Add("@Description", licenseInsert.Description);
            parameter.Add("@Total", licenseInsert.Total);
            // Sửa bán ghi license master
            var masterRes = _sqlConnection.Execute(sqlUpdateMaster, parameter);

            var sqlOldLicenseDetails = $"SELECT * FROM LicenseDetail WHERE LicenseId = @LicenseId";

            var oldLicenseDetails = _sqlConnection.Query<LicenseDetail>(sqlOldLicenseDetails, parameter);
            //  1 2 3 4 5 --- 1 2 3
            // 1 2 3 4 5 --- 6 7 8 9 10
            // Filter từ mảng cũ thành mảng mới vừa được push lên, thằng nào cũ có rồi thì để nguyên, chưa có thì xóa đi để push cái mới
            for (int i = 0; i < oldLicenseDetails.Count(); i++)
            {
                var check = true;
                for (int j = 0; j < licenseInsert.licenseDetails.Length; j++)
                {
                    if (oldLicenseDetails.ElementAt(i).FixedAssetId.Equals(licenseInsert.licenseDetails[j].FixedAssetId) == true)
                    {
                        licenseInsert.licenseDetails = licenseInsert.licenseDetails.Where(val => val.FixedAssetId != licenseInsert.licenseDetails[j].FixedAssetId).ToArray();
                        check = false;
                        break;
                    }

                }
                if (check == true)
                {
                    parameter.Add("@FixedAssetId", oldLicenseDetails.ElementAt(i).FixedAssetId);
                    var deleteOldDetail = $"DELETE FROM LicenseDetail WHERE LicenseId = @LicenseId AND FixedAssetId = @FixedAssetId";
                    var resDeleteOldDetail = _sqlConnection.Execute(deleteOldDetail, parameter);
                }
            }
            var count = 0;
            for (int i = 0; i < licenseInsert.licenseDetails.Length; i++)
            {
                // Sinh Id không trùng cho licenseDetail
                licenseInsert.licenseDetails[i].LicenseDetailId = Guid.NewGuid();

                var sqlInsertDetail = $"INSERT INTO LicenseDetail (LicenseDetailId, LicenseId, FixedAssetId, DetailJson) VALUES (@LicenseDetailId,@LicenseId,@FixedAssetId,@DetailJson)";

                parameter.Add("@FixedAssetId", licenseInsert.licenseDetails[i].FixedAssetId);
                parameter.Add("@LicenseId", licenseId);
                parameter.Add("@LicenseDetailId", licenseInsert.licenseDetails[i].LicenseDetailId);
                parameter.Add("@DetailJson", licenseInsert.licenseDetails[i].DetailJson);
                var res = _sqlConnection.Execute(sqlInsertDetail, parameter);
                count += res;
            }
            return new
            {
                masterRes = masterRes,
                detailRes = count
            };
        }

        
    }
}
