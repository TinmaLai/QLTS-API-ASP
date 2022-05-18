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
        public MISAValidateException(string errorMsg, List<string> errs)
        {
            ErrorMsg = errorMsg;
            ErrorData = new Dictionary<string, List<string>>();
            ErrorData.Add("data", errs);

        }
        public override string Message => this.ErrorMsg;
        public override IDictionary Data => this.ErrorData;
    }
}
