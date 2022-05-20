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
using System.Text.RegularExpressions;

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
        
        public bool IsNumber(string pText)
        {
            Regex regex = new Regex(@"^[-+]?[0-9]*.?[0-9]+$");
            return regex.IsMatch(pText);
        }
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

        public IEnumerable<FixedAsset> Import(List<FixedAsset> fixedAssets)
        {
            foreach(var fixedAsset in fixedAssets)
            {
                Insert(fixedAsset);
            }
            return fixedAssets;
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
