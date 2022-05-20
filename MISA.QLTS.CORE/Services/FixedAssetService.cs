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
        
        public List<FixedAsset> Import(IFormFile formFile)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                var errorService = new ErrorService();
                errorService.UserMsg = Resources.ResourceVN.Error_Validate;
                errorService.Data = ValidateErrorMsgs;
                throw new MISAValidateException("Tệp trống", ValidateErrorMsgs);
            }

            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                var errorService = new ErrorService();
                errorService.UserMsg = Resources.ResourceVN.Error_Validate;
                errorService.Data = ValidateErrorMsgs;
                throw new MISAValidateException("Không đúng định dạng", ValidateErrorMsgs);
            }

            var fixedAssets = new List<FixedAsset>();

            using (var stream = new MemoryStream())
            {
               formFile.CopyToAsync(stream);

                using (var package = new ExcelPackage(stream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        FixedAsset fixedAsset = new FixedAsset();

                        fixedAsset.FixedAssetCode = worksheet.Cells[row, 1].Value.ToString().Trim();
                        fixedAsset.FixedAssetName = worksheet.Cells[row, 2].Value.ToString().Trim();
                        fixedAsset.DepreciationRate = float.Parse(worksheet.Cells[row, 3].Value.ToString().Trim());
                        fixedAsset.LifeTime = int.Parse(worksheet.Cells[row, 4].Value.ToString().Trim());

                        fixedAssets.Add(fixedAsset);
                    }
                }
                //var fixedAssetImported = _fixedAssetRepository.Import(fixedAssets);
                return fixedAssets;
            }
            
        }
        private string ProcessValueToString(object cellValue)
        {
            if (cellValue != null)
            {
                return cellValue.ToString();
            }
            else return null;
        }
        public DateTime? ProcessStringToDate(string dateTime)
        {
            return null;
        }
        //public int Insert(FixedAsset fixedAsset)
        //{

        //    var error = new ErrorService();
        //    var errorData = new List<string>();
        //    //1. Valiate dữ liệu: 
        //    //1.1 Thông tin mã loại tài sản bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.AssetCode))
        //    {
        //        errorData.Add(Resources.ResourceVN.AssetCodeValidate);

        //    }
        //    //1.2 Thông tin tên loại tài sản bắt buộc nhập:
        //    if (string.IsNullOrEmpty(fixedAsset.AssetName))
        //    {
        //        errorData.Add(Resources.ResourceVN.AssetNameValidate);

        //    }
        //    //1.3 Thông tin mã bộ phận sử dụng bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.DepartmentCode))
        //    {
        //        errorData.Add(Resources.ResourceVN.DepartmentCodeValidate);

        //    }
        //    //1.4 Thông tin tên bộ phận sử dụng bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.DepartmentName))
        //    {
        //        errorData.Add(Resources.ResourceVN.DepartmentNameValidate);

        //    }
        //    //1.5 Thông tin mã loại tài sản bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.FixedAssetCategoryCode))
        //    {
        //        errorData.Add(Resources.ResourceVN.CategoryCodeValidate);

        //    }
        //    //1.6 Thông tin tên loại tài sản bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.FixedAssetCategoryName))
        //    {
        //        errorData.Add(Resources.ResourceVN.CategoryNameValidate);

        //    }
        //    //1.7 Thông tin nguyên giá bắt buộc lớn hơn 0   
        //    if (fixedAsset.Cost <= 0)
        //    {
        //        errorData.Add(Resources.ResourceVN.CostValidate);

        //    }
        //    //1.8 Thông tin số lượng bắt buộc lớn hơn 0: 
        //    if (fixedAsset.Quantity <= 0)
        //    {
        //        errorData.Add(Resources.ResourceVN.QuantityValidate);

        //    }
        //    //1.8 Thông tin tỷ lệ hao mòn bắt buộc lớn hơn 0: 
        //    if (fixedAsset.DepreciationRate <= 0)
        //    {
        //        errorData.Add(Resources.ResourceVN.DepreciationRateValidate);

        //    }
        //    //1.9 Thông tin năm theo dõi bắt buộc lớn hơn 0: 
        //    if (fixedAsset.TrackedYear <= 0 || fixedAsset.TrackedYear > 2022)
        //    {
        //        errorData.Add(Resources.ResourceVN.TrackedYearValidate);
        //    }
        //    if (_fixedAssetRepository.CheckCodeDuplicate(fixedAsset.AssetId, fixedAsset.AssetCode, 1))
        //    {
        //        errorData.Add(Resources.ResourceVN.AssetCodeDuplicateValidate);
        //    }
        //    if (errorData.Count > 0)
        //    {
        //        error.UserMsg = Resources.ResourceVN.Error_Validate;
        //        error.Data = errorData;
        //        throw new MISAValidateException("Dữ liệu đầu vào không hợp lệ", errorData);
        //        //throw new Exception("tuann ga");
        //    }

        //    var res = _fixedAssetRepository.Insert(fixedAsset);
        //    return res;

        //}

        //public int Update(Guid assetId, FixedAsset fixedAsset)
        //{
        //    var error = new ErrorService();
        //    var errorData = new List<string>();
        //    //1. Valiate dữ liệu: 
        //    //1.1 Thông tin mã loại tài sản bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.FixedAssetCode))
        //    {
        //        errorData.Add(Resources.ResourceVN.AssetCodeValidate);

        //    }
        //    //1.2 Thông tin tên loại tài sản bắt buộc nhập:
        //    if (string.IsNullOrEmpty(fixedAsset.FixedAssetName))
        //    {
        //        errorData.Add(Resources.ResourceVN.AssetNameValidate);

        //    }
        //    //1.3 Thông tin mã bộ phận sử dụng bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.DepartmentCode))
        //    {
        //        errorData.Add(Resources.ResourceVN.DepartmentCodeValidate);

        //    }
        //    //1.4 Thông tin tên bộ phận sử dụng bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.DepartmentName))
        //    {
        //        errorData.Add(Resources.ResourceVN.DepartmentNameValidate);

        //    }
        //    //1.5 Thông tin mã loại tài sản bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.FixedAssetCategoryCode))
        //    {
        //        errorData.Add(Resources.ResourceVN.CategoryCodeValidate);

        //    }
        //    //1.6 Thông tin tên loại tài sản bắt buộc nhập: 
        //    if (string.IsNullOrEmpty(fixedAsset.FixedAssetCategoryName))
        //    {
        //        errorData.Add(Resources.ResourceVN.CategoryNameValidate);

        //    }
        //    //1.7 Thông tin nguyên giá bắt buộc lớn hơn 0   
        //    if (fixedAsset.Cost <= 0)
        //    {
        //        errorData.Add(Resources.ResourceVN.CostValidate);

        //    }
        //    //1.8 Thông tin số lượng bắt buộc lớn hơn 0: 
        //    if (fixedAsset.Quantity <= 0)
        //    {
        //        errorData.Add(Resources.ResourceVN.QuantityValidate);

        //    }
        //    //1.8 Thông tin tỷ lệ hao mòn bắt buộc lớn hơn 0: 
        //    if (fixedAsset.DepreciationRate <= 0)
        //    {
        //        errorData.Add(Resources.ResourceVN.DepreciationRateValidate);

        //    }
        //    //1.9 Thông tin năm theo dõi bắt buộc lớn hơn 0: 
        //    if (fixedAsset.TrackedYear <= 0 || fixedAsset.TrackedYear > 2022)
        //    {
        //        errorData.Add(Resources.ResourceVN.TrackedYearValidate);
        //    }
        //    if (_fixedAssetRepository.CheckCodeDuplicate(assetId, fixedAsset.FixedAssetCode, 0))
        //    {
        //        errorData.Add(Resources.ResourceVN.AssetCodeDuplicateValidate);
        //    }
        //    if (errorData.Count > 0)
        //    {
        //        error.UserMsg = Resources.ResourceVN.Error_Validate;
        //        error.Data = errorData;
        //        throw new MISAValidateException("Dữ liệu đầu vào không hợp lệ", errorData);
        //        //throw new Exception("tuann ga");
        //    }

        //    var res = _fixedAssetRepository.Update(assetId, fixedAsset);
        //    return res;
        //}
    }
}
