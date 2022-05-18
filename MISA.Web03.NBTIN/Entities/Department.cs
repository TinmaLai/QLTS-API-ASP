namespace MISA.Web03.NBTIN.Entities
{
    public class Department
    {
        /// <summary>
        /// Khóa chính id của bộ phận
        /// </summary>
        public Guid department_id { get; set; }
        /// <summary>
        /// Mã bộ phận
        /// </summary>
        public string department_code { get; set; }
        /// <summary>
        /// Tên bộ phận
        /// </summary>
        public string department_name { get; set; }
        /// <summary>
        /// Id đơn vị tổ chức
        /// </summary>
        public Guid organization_id { get; set; }

    }
}
