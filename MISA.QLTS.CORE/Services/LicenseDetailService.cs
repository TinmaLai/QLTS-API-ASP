using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MISA.QLTS.CORE.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Services
{
    public class LicenseDetailService:BaseService<LicenseDetail>, ILicenseDetailService
    {
        ILicenseDetailRepository _licenseDetailRepository;
        public LicenseDetailService(ILicenseDetailRepository licenseDetailRepository) : base(licenseDetailRepository)
        {
            _licenseDetailRepository = licenseDetailRepository;
        }
    }
}
