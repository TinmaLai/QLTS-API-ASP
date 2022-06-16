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
    public class LicenseRepository: BaseRepository<License>, ILicenseRepository
    {
        IConfiguration _configuration;
        readonly string _connectionString = string.Empty;
        protected MySqlConnection _sqlConnection;
        public LicenseRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("NBTIN");
            _sqlConnection = new MySqlConnection(_connectionString);
        }
        /// <summary>
        /// Hàm thực hiện tìm kiếm theo tham số truyền vào
        /// </summary>
        /// <param name="filterContent">Nội dung tìm kiếm</param>
        /// <param name="departmentName">Tên bộ phận sử dụng</param>
        /// <param name="fixedAssetCategoryName">Tên loại tài sản</param>
        /// <param name="pageSize">Số bản ghi trong một trang</param>
        /// <param name="pageNumber">Trang số bao nhiêu</param>
        /// <returns></returns>
        public object Filter(string? filterContent, int? pageSize, int? pageNumber)
        {
            // Thêm các giá trị vào parameters
            var parameters = new DynamicParameters();
            parameters.Add("@FilterContent", filterContent);
            var pageOffset = pageSize * (pageNumber - 1);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageOffset", pageOffset);
            // Khởi tạo câu lệnh thực hiện tìm kiếm
            var sqlCommand = $"SELECT * FROM License";
            if (filterContent != null) sqlCommand += $" WHERE (LicenseCode LIKE CONCAT('%',@FilterContent,'%') " +
                $"OR Description LIKE CONCAT('%',@FilterContent,'%'))";

            var licensesNoOffset = _sqlConnection.Query<License>(sqlCommand, parameters);
            sqlCommand += $" ORDER BY CreatedDate DESC LIMIT @PageSize OFFSET @PageOffset";
            // Thực hiện tìm kiếm
            var licenses = _sqlConnection.Query<License>(sqlCommand, parameters);
            var res = new
            {
                licenses = (List<License>)licenses.ToList(),
                count = (int)licensesNoOffset.Count()
            };
            return res;
            //return fixedAssets.ToList();
        }

    }
}
