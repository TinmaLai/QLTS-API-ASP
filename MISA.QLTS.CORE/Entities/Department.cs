using MISA.QLTS.CORE.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Entities
{
    public class Department
    {
        /// <summary>
        /// Khóa chính id của bộ phận
        /// </summary>
        public Guid DepartmentId { get; set; }
        /// <summary>
        /// Mã bộ phận
        /// </summary>

        [IsNotNullOrEmpty]
        public string DepartmentCode { get; set; }
        /// <summary>
        /// Tên bộ phận
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// Id đơn vị tổ chức
        /// </summary>
        public Guid OrganizationId { get; set; }
    }
}
