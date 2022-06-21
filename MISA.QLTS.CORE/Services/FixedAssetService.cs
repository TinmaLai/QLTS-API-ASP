using Microsoft.AspNetCore.Http;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Exceptions;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MISA.QLTS.CORE.Interfaces.Services;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Services
{
    public class FixedAssetService : BaseService<FixedAsset>, IFixedAssetService
    {
        IFixedAssetRepository _fixedAssetRepository;
        public FixedAssetService(IFixedAssetRepository fixedAssetRepository):base(fixedAssetRepository)
        {
            _fixedAssetRepository = fixedAssetRepository;
        }
        /// <summary>
        /// Thực hiện import từ file excel
        /// </summary>
        /// <param name="formFile">File Excel</param>
        /// <returns></returns>
        /// <exception cref="MISAValidateException"></exception>
        public List<FixedAsset> Import(IFormFile formFile)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                var errorService = new ErrorService();
                errorService.UserMsg = Resources.ResourceVN.Error_Validate;
                errorService.Data = ValidateErrorMsgs;
                throw new MISAValidateException(Resources.ResourceVN.NullFile, ValidateErrorMsgs);
            }

            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                var errorService = new ErrorService();
                errorService.UserMsg = Resources.ResourceVN.Error_Validate;
                errorService.Data = ValidateErrorMsgs;
                throw new MISAValidateException(Resources.ResourceVN.NoMatchFormatFile, ValidateErrorMsgs);
            }

            var fixedAssets = new List<FixedAsset>();
            // Tạo ra stream để đọc file
            using (var stream = new MemoryStream())
            {
                formFile.CopyToAsync(stream);
                // Tạo ra package thuộc định dạng excel package với stream đã đọc file ở trên
                using (var package = new ExcelPackage(stream))
                {
                    // Lấy theo từng worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    // Lấy ra số hàng có dữ liệu (khoảng từ hàng bắt đầu có đến hàng có dữ liệu cuối cùng)
                    var rowCount = worksheet.Dimension.Rows;
                    // Duyệt từng hàng để thêm dữ liệu vào FixedAsset
                    for (int row = 2; row <= rowCount; row++)
                    {
                        // Thực hiện thêm từng dữ liệu vào từng thuộc tính của đối tượng fixedAsset để chuẩn bị thực hiện thêm mới
                        FixedAsset fixedAsset = new FixedAsset();

                        fixedAsset.FixedAssetCode = worksheet.Cells[row, 1].Value.ToString().Trim();
                        fixedAsset.FixedAssetName = worksheet.Cells[row, 2].Value.ToString().Trim();
                        fixedAsset.FixedAssetCategoryCode = worksheet.Cells[row, 3].Value.ToString().Trim();
                        fixedAsset.FixedAssetCategoryName = worksheet.Cells[row, 4].Value.ToString().Trim();
                        fixedAsset.DepartmentCode = worksheet.Cells[row, 5].Value.ToString().Trim();
                        fixedAsset.DepartmentName = worksheet.Cells[row, 6].Value.ToString().Trim();
                        fixedAsset.DepreciationRate = float.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());
                        fixedAsset.LifeTime = int.Parse(worksheet.Cells[row, 8].Value.ToString().Trim());
                        fixedAsset.TrackedYear = int.Parse(worksheet.Cells[row, 9].Value.ToString().Trim());
                        var purchaseDateValue = worksheet.Cells[row, 10].Value;
                        var purchaseDate = ProcessStringToDate(purchaseDateValue);
                        fixedAsset.PurchaseDate = (DateTime)purchaseDate;
                        var UseDateValue = worksheet.Cells[row, 11].Value;
                        var UseDate = ProcessStringToDate(UseDateValue);
                        fixedAsset.UseDate = (DateTime)UseDate;
                        fixedAsset.ProductionYear = int.Parse(worksheet.Cells[row, 12].Value.ToString().Trim());

                        // Thực hiện validate dữ liệu
                        base.ValidateObject(fixedAsset,1);
                        if(ValidateErrorMsgs.Count() > 0)
                        {
                            fixedAsset.IsValid = false;
                        }
                        if(fixedAsset.IsValid == true)
                        {
                            fixedAssets.Add(fixedAsset);
                        }
                    }
                }
                var fixedAssetImported = _fixedAssetRepository.Import(fixedAssets);
                return fixedAssetImported;
            }
            
        }
        /// <summary>
        /// Chuyển giá trị về định dạng string, nếu null thì return null
        /// </summary>
        /// <param name="cellValue">Giá trị của ô truyền vào</param>
        /// <returns></returns>
        private string ProcessValueToString(object cellValue)
        {
            if (cellValue != null)
            {
                return cellValue.ToString();
            }
            else return null;
        }
        /// <summary>
        /// Chuyển giá trị của ô thành dạng datetime đúng định dạng được đề ra
        /// </summary>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        protected virtual DateTime? ProcessStringToDate(object cellValue)
        {
            DateTime? dateReturn = null;
            if(cellValue == null)
            {
                return null;
            }
            // Ép kiểu về đúng định dạng của DateTime
            if (cellValue.GetType() == typeof(double))
                return DateTime.FromOADate((double)cellValue);
            var dateString = cellValue.ToString();
            // Ngày tháng phải nhập đúng định dạng 
            Regex dateValidRegex = new Regex(@"^([0]?[1-9]|[1|2][0-9]|[3][0|1])[./-]([0]?[1-9]|[1][0-2])[./-]([0-9]{4}|[0-9]{2})$");
            if (dateValidRegex.IsMatch(dateString))
            {
                // Nếu chuỗi truyền vào không đúng định dạng
                // Cắt chuỗi theo /,.,-
                var dateSplit = dateString.Split(new String[] { "/", ".", "-" }, StringSplitOptions.None);
                var day = int.Parse(dateSplit[0]);
                var month = int.Parse(dateSplit[1]);
                var year = int.Parse(dateSplit[2]);
                dateReturn = new DateTime(year, month, day);
            } else if(DateTime.TryParse(cellValue.ToString(), out DateTime dateTime) == true)
            {
                dateReturn = dateTime;
            }
            return dateReturn;
        }
        /// <summary>
        /// Service cho xóa bản ghi tài sản
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public object DeleteService(Guid[] ids)
        {
            var isValid = CheckForeignLicense(ids); 
            if(isValid == null)
            {
                var res = _fixedAssetRepository.MultiDelete(ids);
                return res;
            }
            else return isValid;


        }
        /// <summary>
        /// Check mã tài sản đã được dùng để phát sinh chứng từ hay chưa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object CheckForeignLicense(Guid[] ids)
        {
            var resCheck = _fixedAssetRepository.CheckForeignLicense(ids);
            return resCheck;
        }
    }
}
