using Microsoft.AspNetCore.Http;
using MISA.QLTS.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Interfaces.Services
{
    public interface IFixedAssetService: IBaseService<FixedAsset>
    {
        /// <summary>
        /// Thực hiện đọc tệp dữ liệu và xử lý nghiệp vụ nhập khẩu danh sách nhân viên
        /// </summary>
        /// <param name="file">Tệp chứa hông tin tài sản</param>
        /// <returns>Danh sách nhân viên kèm theo trạng thái chi tiết kết quả import</returns>
        /// CreatedBy: NBTIN (20/05/2022)
        List<FixedAsset> Import(IFormFile file)
        {
            List<FixedAsset> list = new List<FixedAsset>();
            return list;
        }
    }
}
