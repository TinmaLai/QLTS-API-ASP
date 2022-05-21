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
        List<FixedAsset> Filter(string? filterContent, string? departmentName, string? fixedAssetCategoryName, int? pageSize, int? pageNumber);

    }
}
