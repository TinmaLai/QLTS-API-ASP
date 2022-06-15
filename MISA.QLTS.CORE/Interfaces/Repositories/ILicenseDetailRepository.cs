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
        object MultiInsert(LicenseInsert licenseInsert);
    }
}
