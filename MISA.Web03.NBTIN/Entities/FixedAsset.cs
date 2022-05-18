namespace MISA.Web03.NBTIN.Entities
{
    public class FixedAsset
    {
        /// <summary>
        /// Khóa chính
        /// </summary>
        public Guid AssetId{get; set;}
        /// <summary>
        /// Mã tài sản
        /// </summary>
        public string? AssetCode { get; set; }
        /// <summary>
        /// Tên tài sản
        /// </summary>
        public string AssetName { get; set; }
        /// <summary>
        /// Khóa ngoại bộ phận sử dụng
        /// </summary>
         public Guid DepartmentId { get; set; }
        /// <summary>
        /// Mã bộ phận sử dụng
        /// </summary>
        public string DepartmentCode { get; set; }
        /// <summary>
        /// Tên bộ phận sử dụng
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Khóa ngoại id loại tài sản
        /// </summary>
        public Guid FixedAssetCategoryId { get; set; }
        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        public string FixedAssetCategoryCode { get; set; }
        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        public string FixedAssetCategoryName { get; set; }
        /// <summary>
        /// Ngày mua
        /// </summary>
        public DateTime PurchaseDate { get; set; }
        /// <summary>
        /// Nguyên giá
        /// </summary>
        public decimal Cost { get; set; }
        /// <summary>
        /// Số lượng
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// Tỷ lệ hao mòn
        /// </summary>
        public float DepreciationRate { get; set; }
        /// <summary>
        /// Năm theo dõi
        /// </summary>
        public int TrackedYear { get; set; }
        /// <summary>
        /// Số năm sử  dụng
        /// </summary>
        public int LifeTime { get; set; }
        /// <summary>
        /// Ngày bắt đầu sử dụng
        /// </summary>
        public DateTime ProductionYear { get; set; }
    }
}
