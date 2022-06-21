using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Entities
{
    public class MasterDetailLicense
    {
        // Master license
        public License license;
        // Detail license
        public LicenseDetail licenseDetail;
    }
}
