using MISA.QLTS.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Interfaces.Repositories
{
    public interface IFixedAssetRepository: IBaseRepository<FixedAsset>
    {
        /// <summary>
        /// Thực hiện import dữ liệu vào database
        /// </summary>
        /// <param name="fixedAssets">Danh sách nhân viên hợp lệ</param>
        /// <returns>Trả về danh sách đã thêm đc/returns>
        /// CreatedBy: NBTIN(20/05/2022)
        List<FixedAsset> Import(List<FixedAsset> fixedAssets);
        /// <summary>
        /// Thực hiện xóa nhiều bản ghi
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        int MultiDelete(Guid[] ids);
        /// <summary>
        /// Thực hiện filter
        /// </summary>
        /// <param name="filterContent">Chuỗi tìm kiếm</param>
        /// <param name="departmentName">Tên bộ phận sử dụng</param>
        /// <param name="fixedAssetCategoryName">Tên loại tài sản</param>
        /// <param name="pageSize">Số bản ghi trong 1 trang</param>
        /// <param name="pageNumber">Trang số bao nhiêu</param>
        /// <returns></returns>
        object Filter(string? filterContent, string? departmentName, string? fixedAssetCategoryName, int? pageSize, int? pageNumber);

    }
}
