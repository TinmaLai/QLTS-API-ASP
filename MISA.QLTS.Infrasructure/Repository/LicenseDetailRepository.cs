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

            for(int i = 0; i < licenseInsert.licenseDetails.Length; i++)
            {
                // Sinh Id không trùng cho licenseDetail
                licenseInsert.licenseDetails[i].LicenseDetailId = Guid.NewGuid();

                var sqlInsertDetail = $"INSERT INTO LicenseDetail (LicenseDetailId, LicenseId, FixedAssetId) VALUES (@LicenseDetailId,@LicenseId,@FixedAssetId)";

                parameter.Add("@FixedAssetId", licenseInsert.licenseDetails[i].FixedAssetId);
                parameter.Add("@LicenseId", licenseInsert.LicenseId);
                parameter.Add("@LicenseDetailId", licenseInsert.licenseDetails[i].LicenseDetailId);
                var res =  _sqlConnection.Execute(sqlInsertDetail, parameter);
                count += res;
            }
            
            
            var lastRes = new
            {
                detail = count,
                masterRes = masterRes
            };
            return lastRes;
        }

      
    }
}
