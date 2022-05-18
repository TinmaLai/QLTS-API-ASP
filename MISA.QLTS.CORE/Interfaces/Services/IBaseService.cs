using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Interfaces.Services
{
    public interface IBaseService<T>
    {
        /// <summary>
        /// thêm mới bản ghi
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        int Insert(T entity);
        /// <summary>
        /// sửa bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int Update(Guid id, T entity);
    }
}
