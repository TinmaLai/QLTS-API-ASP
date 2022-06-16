using MISA.QLTS.CORE.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Interfaces.Services
{
    public interface ILicenseDetailService: IBaseService<LicenseDetail>
    {
        /// <summary>
        /// Hàm insert license
        /// </summary>
        /// <param name="licenseInsert"></param>
        /// <returns></returns>
        object Insert(LicenseInsert licenseInsert);
        /// <summary>
        /// Sửa bản ghi master - detail
        /// </summary>
        /// <param name="licenseInsert"></param>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        object Update(LicenseInsert licenseInsert, Guid licenseId);
    }
}
