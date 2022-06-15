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
    public class LicenseService: BaseService<License>, ILicenseService
    {
        ILicenseRepository _licenseRepository;
        public LicenseService(ILicenseRepository licenseRepository) : base(licenseRepository)
        {
            _licenseRepository = licenseRepository;
        }
    }
}
