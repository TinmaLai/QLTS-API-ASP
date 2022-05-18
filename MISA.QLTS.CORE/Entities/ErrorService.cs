using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Entities
{
    public class ErrorService
    {
        /// <summary>
        /// Tin nhắn trả về dành cho developer
        /// </summary>
        public string DevMsg { get; set; }
        /// <summary>
        /// Tin nhắn trả về dành cho người dùng
        /// </summary>
        public string UserMsg { get; set; }
        /// <summary>
        /// Tập hợp các message
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// Tập hợp các mã lôi
        /// </summary>
        public string ErrorCode { get; set; }
    }
}
