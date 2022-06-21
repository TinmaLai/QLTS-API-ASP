using MISA.QLTS.CORE.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Entities
{
    public class LicenseDetail
    {
        /// <summary>
        /// Id 
        /// </summary>
        [PrimaryKey]
        public Guid LicenseDetailId { get; set; }
        /// <summary>
        /// Khóa ngoại Id của bảng Licnese (chứng từ)
        /// </summary>
        public Guid LicenseId { get; set; }
        /// <summary>
        /// Khóa ngoại Id của bảng FixedAsset
        /// </summary>
        public Guid FixedAssetId { get; set; }
        /// <summary>
        /// Thông tin nguồn tài sản
        /// </summary>
        public string? DetailJson { get; set; }

    }
}
