using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Exceptions;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MISA.QLTS.CORE.Interfaces.Services;
using MISA.QLTS.CORE.MISAAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Services
{
    public class LicenseDetailService:BaseService<LicenseDetail>, ILicenseDetailService
    {
        ILicenseDetailRepository _licenseDetailRepository;
        public LicenseDetailService(ILicenseDetailRepository licenseDetailRepository) : base(licenseDetailRepository)
        {
            _licenseDetailRepository = licenseDetailRepository;
        }
        /// <summary>
        /// Hàm insert nhưng check validate
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        /// <exception cref="MISAValidateException"></exception>
        //public object Insert(LicenseInsert licenseInsert)
        //{
        //    int mode = 1;
        //    // Thực hiện thêm mới dữ liệu
        //    // Thực hiện validate trước khi thêm
        //    var isValid = ValidateObject(licenseInsert, mode);
        //    if (isValid == true && (ValidateErrorMsgs == null || ValidateErrorMsgs.Count() == 0))
        //    {
        //        return _licenseDetailRepository.MultiInsert(licenseInsert);

        //    }
        //    else
        //    {
        //        // Xử lý lỗi khi Validate có lỗi xảy ra
        //        var errorService = new ErrorService();
        //        errorService.UserMsg = Resources.ResourceVN.Error_Validate;
        //        errorService.Data = ValidateErrorMsgs;
        //        throw new MISAValidateException(Resources.ResourceVN.Error_Validate, ValidateErrorMsgs);
        //    }


        //}
        /// <summary>
        /// Sửa bản ghi master - detail
        /// </summary>
        /// <param name="licenseInsert"></param>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        /// <exception cref="MISAValidateException"></exception>
        //public object Update(LicenseInsert licenseInsert, Guid licenseId)
        //{
        //    int mode = 0;
        //    // Lấy toàn bộ thuộc tính của đối tượng
        //    var properties = typeof(LicenseInsert).GetProperties();
        //    foreach (var prop in properties)
        //    {
        //        // Kiểu dữ liệu của prop
        //        var propType = prop.PropertyType;
        //        var isPrimaryKey = prop.IsDefined(typeof(PrimaryKey), true);
        //        if (isPrimaryKey == true)
        //        {
        //            // Set id từ url vào id của entity 
        //            prop.SetValue(licenseInsert, licenseId);
        //        }
        //    }
        //    // Thực hiện sửa dữ liệu
        //    var isValid = ValidateObject(licenseInsert, mode);
        //    if (isValid == true && (ValidateErrorMsgs == null || ValidateErrorMsgs.Count() == 0))
        //    {
        //        return _licenseDetailRepository.UpdateLicenseInsert(licenseInsert, licenseId);

        //    }
        //    else
        //    {
        //        var errorService = new ErrorService();
        //        errorService.UserMsg = Resources.ResourceVN.Error_Validate;
        //        errorService.Data = ValidateErrorMsgs;
        //        throw new MISAValidateException(Resources.ResourceVN.Error_Validate, ValidateErrorMsgs);
        //    }
        //}
        /// <summary>
        /// validate chung
        /// </summary>
        /// <param name="entity">Đối tượng cần validate</param>
        /// <param name="mode">Trạng thái khi validate (thêm/sửa)</param>
        /// <returns>true: Không có lỗi validate, false: Có lỗi validate</returns>
        /// CreatedBy: NBTIN(18/05/2022)
        protected bool ValidateObject(LicenseInsert entity, int mode)
        {
            var isValid = true;
            var propId = Guid.NewGuid();
            // Lấy toàn bộ thuộc tính của đối tượng
            var properties = typeof(LicenseInsert).GetProperties();
            foreach (var prop in properties)
            {

                // Lấy tên của prop
                var propName = prop.Name;
                // Lấy tên gọi được của prop (VD: Tên tài sản, Mã tài sản, ...)
                var propFriendlyName = propName;
                // Lấy giá trị thêm vào
                var propValue = prop.GetValue(entity);
                // kiểm tra prop có phải khóa chính không
                var isPrimaryKey = prop.IsDefined(typeof(PrimaryKey), true);
                // Kiểu dữ liệu của prop
                var propType = prop.PropertyType;
                if (isPrimaryKey == true)
                {
                    // Nếu là khóa chính, lấy ra propId
                    propId = Guid.Parse(propValue.ToString());
                }
                // Kiểm tra xem prop hiện tại có gán attribute PropertyFriendlyName không
                var isFriendlyName = prop.IsDefined(typeof(PropertyNameFriendly), true);
                if (isFriendlyName)
                {
                    propFriendlyName = (prop.GetCustomAttributes(typeof(PropertyNameFriendly), true)[0] as PropertyNameFriendly).Name;
                }
                // 1. Thông tin bắt buộc nhập:
                var isNotNullOrEmpty = prop.IsDefined(typeof(IsNotNullOrEmpty), true);
                if (isNotNullOrEmpty == true && (propValue == null || propValue.ToString() == ""))
                {
                    isValid = false;
                    ValidateErrorMsgs.Add($"Thông tin {propFriendlyName} không được phép để trống");
                }
                // 2. Thông tin không được phép trùng
                var isNotDuplicate = prop.IsDefined(typeof(NotDuplicate), true);
                if (isNotDuplicate == true)
                {
                    var isDup = _licenseDetailRepository.CheckCodeDuplicate(propId, propValue.ToString(), mode);
                    if (isDup == true)
                    {
                        isValid = false;
                        ValidateErrorMsgs.Add($"Thông tin {propFriendlyName} không được phép trùng");
                    }
                    else isValid = true;
                }
                // 3. Các thông tin là chuỗi có yêu cầu giới hạn về độ dài (Mã tài sản không vượt quá 100 kí tự)
                var isMaxLength = prop.IsDefined(typeof(MaxLength), true);
                if (isMaxLength)
                {
                    // Lấy ra maxLength
                    var maxLength = (prop.GetCustomAttributes(typeof(MaxLength), true)[0] as MaxLength).Length;
                    // Kiểm tra xem độ dài của value có lớn hơn giá trị maxLength không
                    if (propValue.ToString().Length > maxLength)
                    {
                        isValid = false;
                        ValidateErrorMsgs.Add(string.Format(Resources.ResourceVN.ErrorValidate_PropertyMaxLength, propFriendlyName, maxLength));
                    }
                }

            }
            return isValid;

            
        }
    }
}
