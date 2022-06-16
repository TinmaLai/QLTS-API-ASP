using MISA.QLTS.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Interfaces.Repositories
{
    public interface ILicenseRepository: IBaseRepository<License>
    {
        /// <summary>
        /// Hàm tìm kiếm, phân trang bảng chứng từ master
        /// </summary>
        /// <param name="filterContent"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        object Filter(string? filterContent, int? pageSize, int? pageNumber);
    }
}
