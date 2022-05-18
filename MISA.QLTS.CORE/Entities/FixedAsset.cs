using MISA.QLTS.CORE.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Entities
{
    public  class FixedAsset
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        [PrimaryKey]
        public Guid FixedAssetId { get; set; }
        /// <summary>
        /// Mã tài sản
        /// </summary>
        [IsNotNullOrEmpty]
        [NotDuplicate]
        [PropertyNameFriendly("Mã tài sản")]
        public string? FixedAssetCode { get; set; }
        /// <summary>
        /// Tên tài sản
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Tên tài sản")]
        public string FixedAssetName { get; set; }
        /// <summary>
        /// Khóa ngoại bộ phận sử dụng
        /// </summary>
        public Guid DepartmentId { get; set; }
        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Mã phòng ban")]
        public string DepartmentCode { get; set; }
        /// <summary>
        /// Tên bộ phận sử dụng
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Tên phòng ban")]
        public string DepartmentName { get; set; }
        /// <summary>
        /// Khóa ngoại id loại tài sản
        /// </summary>
        public Guid FixedAssetCategoryId { get; set; }
        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        [IsNotNullOrEmpty]
        [MaxLength(20)]
        [PropertyNameFriendly("Mã loại tài sản")]
        public string FixedAssetCategoryCode { get; set; }
        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Tên loại tài sản")]
        public string FixedAssetCategoryName { get; set; }
        /// <summary>
        /// Ngày mua
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Ngày mua")]
        public DateTime PurchaseDate { get; set; }
        /// <summary>
        /// Nguyên giá
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Nguyên giá")]
        public decimal Cost { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Số lượng")]
        public int Quantity { get; set; }
        /// <summary>
        /// Tỷ lệ hao mòn
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Tỷ lệ hao mòn")]
        public float DepreciationRate { get; set; }
        /// <summary>
        /// Năm theo dõi
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Năm theo dõi")]
        public int TrackedYear { get; set; }
        /// <summary>
        /// Số năm sử  dụng
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Số năm sử dụng")]
        public int LifeTime { get; set; }
        /// <summary>
        /// Ngày bắt đầu sử dụng
        /// </summary>
        [IsNotNullOrEmpty]
        [PropertyNameFriendly("Ngày bắt đầu sử dụng")]
        public DateTime ProductionYear { get; set; }
    }

}
