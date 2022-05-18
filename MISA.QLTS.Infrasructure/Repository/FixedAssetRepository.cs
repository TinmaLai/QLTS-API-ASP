using Microsoft.Extensions.Configuration;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MISA.QLTS.Infrasructure.Repository
{
    public class FixedAssetRepository : BaseRepository<FixedAsset>, IFixedAssetRepository
    {
        IConfiguration _configuration;
        public FixedAssetRepository(IConfiguration configuration):base(configuration)
        {
            _configuration = configuration;
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
            var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
            var sqlConnection = new MySqlConnection(connectionString);
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
            var res = sqlConnection.QueryFirstOrDefault<object>(sqlCheck, parameters);

            if (res != null)
            {
                return true;
            }
            return false;
        }

        //public int Delete(Guid fixedAssetId)
        //{
        //    var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
        //    //1. Khởi tạo kết nối với MariaDB
        //    var sqlConnection = new MySqlConnection(connectionString);
        //    var sqlCommand = $"DELETE FROM fixed_asset WHERE AssetId=@assetId";
        //    var parameters = new DynamicParameters();
        //    parameters.Add("@AssetId", fixedAssetId);
        //    // Thực hiện truy vấn
        //    var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
        //    return res;
        //}
        //public FixedAsset GetById(Guid assetId)
        //{
        //    // Khởi tạo kết nối với database
        //    var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
        //    //1. Khởi tạo kết nối với MariaDB
        //    var sqlConnection = new MySqlConnection(connectionString);
        //    //2. Lấy dữ liệu
        //    //2.1. Câu lệnh truy vấn dữ liệu
        //    var sqlCommand = $"SELECT * FROM fixed_asset WHERE AssetId = @FixedAssetId";
        //    DynamicParameters parameters = new DynamicParameters();
        //    parameters.Add("@FixedAssetId", assetId);
        //    //2.2. Thực hiện lấy dữ liệu
        //    var fixedAsset = sqlConnection.QueryFirstOrDefault<FixedAsset>(sql: sqlCommand, param: parameters);


        //    //Trả kết quả cho client

        //    return fixedAsset;
        //}
        public string getNewCode()
        {
            
            // Truy vấn ra danh sách các mã có tiền tố là TS
            string sqlCommand = $"SELECT FixedAssetCode FROM FixedAsset WHERE FixedAssetCode IS NOT NULL AND FixedAssetCode LIKE '%TS%' ORDER BY  LENGTH(FixedAssetCode) DESC, FixedAssetCode DESC";
            // Lấy bản ghi cuối cùng (giá trị gần nhất)
            var FixedAssetCode = _sqlConnection.QueryFirstOrDefault<string>(sqlCommand);

            int currentMax = 0;
            int codeValue = int.Parse(FixedAssetCode.Substring(2).ToString());
            if (currentMax < codeValue)
            {
                currentMax = codeValue;
            }
            currentMax = currentMax + 1;
            string newAssetCode = "TS";
            for (int i = 1; i < FixedAssetCode.Length - Math.Floor(Math.Log10(currentMax) + 2); i++)
            {
                newAssetCode += "0";
            }
            newAssetCode += currentMax;
            return newAssetCode;
        }

        public List<FixedAsset> getPaging(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
        }

        //public int Insert(FixedAsset fixedAsset)
        //{
        //    fixedAsset.AssetId = Guid.NewGuid();
        //    var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
        //    //1. Kết nối với MariaDB
        //    var sqlConnection = new MySqlConnection(connectionString);
        //    //2. Lấy dữ liệu
        //    //2.1. Câu lệnh truy vấn dữ liệu

        //    var sqlCommand = $"INSERT INTO fixed_asset (AssetID, AssetCode, AssetName, DepartmentId, DepartmentCode, DepartmentName, FixedAssetCategoryId, FixedAssetCategoryCode, FixedAssetCategoryName, PurchaseDate, Cost, Quantity, DepreciationRate, TrackedYear, LifeTime, ProductionYear) VALUES (@AssetID, @AssetCode, @AssetName, @DepartmentId, @DepartmentCode, @DepartmentName, @FixedAssetCategoryId, @FixedAssetCategoryCode, @FixedAssetCategoryName, @PurchaseDate, @Cost, @Quantity, @DepreciationRate, @TrackedYear, @LifeTime, @ProductionYear)";

        //    DynamicParameters parameters = new DynamicParameters();
        //    parameters.Add("@AssetId", fixedAsset.AssetId);
        //    parameters.Add("@AssetCode", fixedAsset.AssetCode);
        //    parameters.Add("@AssetName", fixedAsset.AssetName);
        //    parameters.Add("@DepartmentId", fixedAsset.DepartmentId);
        //    parameters.Add("@DepartmentCode", fixedAsset.DepartmentCode);
        //    parameters.Add("@DepartmentName", fixedAsset.DepartmentName);
        //    parameters.Add("@FixedAssetCategoryId", fixedAsset.FixedAssetCategoryId);
        //    parameters.Add("@FixedAssetCategoryCode", fixedAsset.FixedAssetCategoryCode);
        //    parameters.Add("@FixedAssetCategoryName", fixedAsset.FixedAssetCategoryName);
        //    parameters.Add("@PurchaseDate", fixedAsset.PurchaseDate);
        //    parameters.Add("@Cost", fixedAsset.Cost);
        //    parameters.Add("@Quantity", fixedAsset.Quantity);
        //    parameters.Add("@DepreciationRate", fixedAsset.DepreciationRate);
        //    parameters.Add("@TrackedYear", fixedAsset.TrackedYear);
        //    parameters.Add("@LifeTime", fixedAsset.LifeTime);
        //    parameters.Add("@ProductionYear", fixedAsset.ProductionYear);

        //    var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
        //    return res;

        //}

        //public int Update(Guid fixedAssetId, FixedAsset fixedAsset)
        //{
        //    var connectionString = "Host=3.0.89.182; Port=3306; Database=MISA.WEB03.NBTIN; User id=dev; Password=12345678";
        //    //1. Kết nối với MariaDB
        //    var sqlConnection = new MySqlConnection(connectionString);
        //    //2. Lấy dữ liệu
        //    //2.1. Câu lệnh truy vấn dữ liệu

        //    var sqlCommand = $"UPDATE FixedAsset SET FixedAssetCode=@AssetCode, FixedAssetName=@AssetName, DepartmentCode=@DepartmentCode,DepartmentName=@DepartmentName" +
        //            $",FixedAssetCategoryCode=@FixedAssetCategoryCode,FixedAssetCategoryName=@FixedAssetCategoryName" +
        //            $",PurchaseDate=@PurchaseDate,Cost=@Cost,Quantity=@Quantity,DepreciationRate=@DepreciationRate,TrackedYear=@TrackedYear" +
        //            $",LifeTime=@LifeTime,ProductionYear=@ProductionYear WHERE FixedAssetId=@AssetId";

        //    DynamicParameters parameters = new DynamicParameters();
        //    parameters.Add("@AssetId", fixedAssetId);
        //    parameters.Add("@AssetCode", fixedAsset.FixedAssetCode);
        //    parameters.Add("@AssetName", fixedAsset.FixedAssetName);
        //    parameters.Add("@DepartmentId", fixedAsset.DepartmentId);
        //    parameters.Add("@DepartmentCode", fixedAsset.DepartmentCode);
        //    parameters.Add("@DepartmentName", fixedAsset.DepartmentName);
        //    parameters.Add("@FixedAssetCategoryId", fixedAsset.FixedAssetCategoryId);
        //    parameters.Add("@FixedAssetCategoryCode", fixedAsset.FixedAssetCategoryCode);
        //    parameters.Add("@FixedAssetCategoryName", fixedAsset.FixedAssetCategoryName);
        //    parameters.Add("@PurchaseDate", fixedAsset.PurchaseDate);
        //    parameters.Add("@Cost", fixedAsset.Cost);
        //    parameters.Add("@Quantity", fixedAsset.Quantity);
        //    parameters.Add("@DepreciationRate", fixedAsset.DepreciationRate);
        //    parameters.Add("@TrackedYear", fixedAsset.TrackedYear);
        //    parameters.Add("@LifeTime", fixedAsset.LifeTime);
        //    parameters.Add("@ProductionYear", fixedAsset.ProductionYear);

        //    var res = sqlConnection.Execute(sql: sqlCommand, param: parameters);
        //    return res;
        //}

    }
}
