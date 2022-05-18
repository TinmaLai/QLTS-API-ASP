namespace MISA.Web03.NBTIN.Entities
{
    public class FixedAssetCategory
    {
        /// <summary>
        /// Khóa chính 
        /// </summary>
        public Guid fixed_asset_category_id { get; set; }
        /// <summary>
        /// Mã loại tài sản
        /// </summary>
        public string fixed_asset_category_code { get; set; }
        /// <summary>
        /// Tên loại tài sản
        /// </summary>
        public string fixed_asset_category_name { get; set; }
        /// <summary>
        /// Khóa phụ id đơn vị
        /// </summary>
        public Guid organization_id { get; set; }
        /// <summary>
        /// Tỉ lệ hao mòn năm
        /// </summary>
        public float depreciation_rate { get; set; }
        /// <summary>
        /// Thời gian sử dụng
        /// </summary>
        public int life_time { get; set; }
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
