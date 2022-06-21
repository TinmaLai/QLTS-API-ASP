using MISA.QLTS.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Interfaces.Repositories
{
    public interface ILicenseDetailRepository:IBaseRepository<LicenseDetail>
    {
        /// <summary>
        /// Thêm 1 mảng license detail
        /// </summary>
        /// <param name="licenseDetails"></param>
        /// <returns></returns>
        object MultiInsert(LicenseDetail[] licenseDetails, Guid licenseId);
        /// <summary>
        /// Lấy ra 1 licenseInsert, vừa chứa master, vừa chứa detail
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        object GetLicenseInsertById(Guid licenseId);
        /// <summary>
        /// Lấy ra list detail từ 1 licenseId
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        List<object> GetDetailAssets(Guid licenseId);
        /// <summary>
        /// Sửa bản license detail
        /// </summary>
        /// <param name="licenseInsert"></param>
        /// <returns></returns>
        int UpdateLicenseDetail(Guid licenseDetailId, string detailJson);
        /// <summary>
        /// Lấy ra bộ phận sử dụng và detail json của license detail
        /// </summary>
        /// <param name="licenseDetailId"></param>
        /// <returns></returns>
        object GetMoneySource(Guid licenseDetailId);


    }
}
