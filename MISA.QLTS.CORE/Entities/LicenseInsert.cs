using MISA.QLTS.CORE.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Entities
{
    public class LicenseInsert
    {
        /// <summary>
        /// Id chứng từ
        /// </summary>
        [PrimaryKey]
        public Guid LicenseId { get; set; }
        /// <summary>
        /// Mã chứng từ
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Mã chứng từ")]
        [NotDuplicate]
        public string LicenseCode { get; set; }
        /// <summary>
        /// Ngày bắt đầu sử dụng
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Ngày bắt đầu sử dụng")]
        public DateTime UseDate { get; set; }
        /// <summary>
        /// Ngày ghi tăng
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Ngày ghi tăng")]
        public DateTime WriteUpdate { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Tổng nguyên giá
        /// </summary>
        public float Total { get; set; }
        /// <summary>
        /// Danh sách tài sản ở bảng thông tin chi tiết
        /// </summary>
        public LicenseDetail[] licenseDetails { get; set; }
    }
}
