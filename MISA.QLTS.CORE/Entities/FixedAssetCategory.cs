using MISA.QLTS.CORE.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Entities
{
    public class FixedAssetCategory
    {
        /// <summary>
        /// Khóa chính 
        /// </summary>
        public Guid FixedAssetCategoryId { get; set; }
        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        [IsNotNullOrEmpty]
        public string FixedAssetCategoryCode { get; set; }
        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        public string FixedAssetCategoryName { get; set; }
        /// <summary>
        /// Khóa phụ id đơn vị
        /// </summary>
        public Guid Organization { get; set; }
        /// <summary>
        /// Tỉ lệ hao mòn năm
        /// </summary>
        public float DepreciationRate { get; set; }
        /// <summary>
        /// Thời gian sử dụng
        /// </summary>
        public int LifeTime { get; set; }
        /// <summary>
        /// Tên người tạo
        /// </summary>
        public string created_by { get; set; }
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTime created_date { get; set; }
        /// <summary>
        /// Tên người chỉnh sửa
        /// </summary>
        public string modified_by { get; set; }
        /// <summary>
        /// Ngày chỉnh sửa
        /// </summary>
        public DateTime modified_date { get; set; }
    }
}
