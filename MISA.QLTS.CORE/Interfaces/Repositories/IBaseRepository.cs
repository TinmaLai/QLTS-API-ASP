using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Interfaces.Repositories
{
    public interface IBaseRepository<T>
    {
        /// <summary>
        /// Lấy toàn bộ danh sách tài sản
        /// </summary>
        /// <returns>Danh sách tài sản</returns>
        List<T> Get();
        /// <summary>
        /// Thêm tài sản
        /// </summary>
        /// <param name="fixedAsset">Đối tượng cần thêm</param>
        /// <returns>Số bản ghi được thêm</returns>
        int Insert(T entity);
        /// <summary>
        /// Sửa bản ghi tài sản
        /// </summary>
        /// <param name="fixedAsset">Tài sản cần được sửa</param>
        /// <returns>Số bản ghi bị sửa</returns>
        int Update(Guid id, T entity);
        /// <summary>
        /// Xóa bản ghi
        /// </summary>
        /// <param name="fixedAssetId">Id bản ghi muốn xóa</param>
        /// <returns>Số bản ghi bị xóa</returns>
        int Delete(Guid id);
        /// <summary>
        /// Lấy danh sách trong 1 trang
        /// </summary>
        /// <param name="pageIndex">Số trang</param>
        /// <param name="pageSize">Số bản ghi 1 trang</param>
        /// <returns>Danh sách tài sản</returns>
        List<T> getPaging(int pageIndex, int pageSize);
        /// <summary>
        /// Tự sinh mã mới
        /// </summary>
        /// <returns>Mã</returns>
        string getNewCode();
        /// <summary>
        /// Check trùng mã
        /// </summary>
        /// <param name="fixedAssetCode">Mã cần check</param>
        /// <returns>Trùng hay không, true/false</returns>
        bool CheckCodeDuplicate(Guid id, string code, int mode);
        /// <summary>
        /// Lấy tài sản theo id
        /// </summary>
        /// <param name="assetId">id</param>
        /// <returns></returns>
        T GetById(Guid id);
    }
}
