using MISA.QLTS.CORE.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Entities
{
    public class License
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
        [NotDuplicate]
        public string LicenseCode { get; set; }
        /// <summary>
        /// Ngày bắt đầu sử dụng
        /// </summary>
        [IsNotNullOrEmpty]
        public DateTime UseDate { get; set; }
        /// <summary>
        /// Ngày ghi tăng
        /// </summary>
        [IsNotNullOrEmpty]
        public DateTime WriteUpdate { get; set; }
        /// <summary>
        /// Ghi chú
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Tổng nguyên giá
        /// </summary>
        public float Total { get; set; }


    }
}
