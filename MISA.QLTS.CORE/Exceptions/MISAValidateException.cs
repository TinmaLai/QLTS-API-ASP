using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Exceptions
{
    public class MISAValidateException: Exception
    {
        public string ErrorMsg;
        IDictionary ErrorData;
        /// <summary>
        /// Hàm khởi tạo MISAValidateException
        /// </summary>
        /// <param name="errorMsg">Chuỗi lỗi khi gọi</param>
        /// <param name="errs">Mảng các chuỗi lỗi</param>
        public MISAValidateException(string errorMsg, List<string> errs)
        {
            ErrorMsg = errorMsg;
            ErrorData = new Dictionary<string, List<string>>();
            ErrorData.Add("data", errs);

        }
        // Override lại msg truyền vào cho Message
        public override string Message => this.ErrorMsg;
        public override IDictionary Data => this.ErrorData;
    }
}
