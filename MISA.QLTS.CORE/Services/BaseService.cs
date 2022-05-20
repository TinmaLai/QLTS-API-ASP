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
    public class BaseService<T>: IBaseService<T>
    {
        IBaseRepository<T> _baseRepository;
        protected List<string> ValidateErrorMsgs;
        public BaseService(IBaseRepository<T> baseRepository)
        {
            _baseRepository = baseRepository;
            ValidateErrorMsgs = new List<string>();
        }
        public int Insert(T entity)
        {
            int mode = 1;
            // Thực hiện thêm mới dữ liệu
            //TODO: ANH QUYEN GA
            var isValid = ValidateObject(entity, mode);
            if (isValid == true && (ValidateErrorMsgs == null || ValidateErrorMsgs.Count() == 0))
            {
                return _baseRepository.Insert(entity);
                
            } else
            {
                var errorService = new ErrorService();
                errorService.UserMsg = Resources.ResourceVN.Error_Validate;
                errorService.Data = ValidateErrorMsgs;
                throw new MISAValidateException(Resources.ResourceVN.Error_Validate, ValidateErrorMsgs);
            }
                

        }
        /// <summary>
        /// validate chung
        /// </summary>
        /// <param name="entity">Đối tượng cần validate</param>
        /// <param name="mode">Trạng thái khi validate (thêm/sửa)</param>
        /// <returns></returns>
        /// CreatedBy: NBTIN(18/05/2022)
        private bool ValidateObject(T entity, int mode)
        {
            var isValid = true;
            var propId = Guid.NewGuid();
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                //
                
                // Lấy tên của prop
                var propName = prop.Name;
                var propFriendlyName = propName;
                // Lấy giá trị thêm vào
                var propValue = prop.GetValue(entity);
                // Kiểu dữ liệu của prop
                var isPrimaryKey = prop.IsDefined(typeof(PrimaryKey), true);
                var propType = prop.PropertyType;
                if (isPrimaryKey == true)
                {
                    propId = Guid.Parse(propValue.ToString());
                }
                // Kiểm tra xem prop hiện tại có gán attribute PropertyFriendlyName không
                var isFriendlyName = prop.IsDefined(typeof(PropertyNameFriendly), true);
                if (isFriendlyName)
                {
                    propFriendlyName = (prop.GetCustomAttributes(typeof(PropertyNameFriendly),true)[0] as PropertyNameFriendly).Name;
                }
                // 1. Thông tin bắt buộc nhập:
                var isNotDuplicate = prop.IsDefined(typeof(NotDuplicate), true);
                var isNotNullOrEmpty = prop.IsDefined(typeof(IsNotNullOrEmpty), true);
                if (isNotDuplicate == true)
                {
                    var isDup = _baseRepository.CheckCodeDuplicate(propId, propValue.ToString(), mode);
                    if (isDup == true)
                    {
                        isValid = false;
                        ValidateErrorMsgs.Add($"Thông tin {propFriendlyName} không được phép trùng");
                    }
                    else isValid = true;
                }
                if(isNotNullOrEmpty == true && (propValue == null || propValue.ToString() == ""))
                {
                    isValid = false;
                    ValidateErrorMsgs.Add($"Thông tin {propFriendlyName} không được phép để trống");
                }
                var isMaxLength = prop.IsDefined(typeof(MaxLength), true);
                if (isMaxLength)
                {
                    // Lấy ra maxLength
                    var maxLength = (prop.GetCustomAttributes(typeof(MaxLength), true)[0] as MaxLength).Length;
                    if (propValue.ToString().Length > maxLength)
                    {
                        isValid = false;
                        ValidateErrorMsgs.Add(string.Format(Resources.ResourceVN.ErrorValidate_PropertyMaxLength, propFriendlyName, maxLength));
                    }
                }
                
                
                //if (isPrimaryKey == true && propType == typeof(Guid))
                //{
                //    prop.SetValue(entity, Guid.NewGuid());
                //}
                // 2. Các thông tin là chuỗi có yêu cầu giới hạn về độ dài (Mã tài sản không vượt quá 100 kí tự)

                // 3. Ngày tháng không vượt qua ngày hiện tại

            }
            return isValid;

            // Thực hiện validate đặc thù cho từng đối tượng khác nhau
            ValidateObjectCustom(entity, mode);
            return isValid;
        }

        protected virtual List<string> ValidateObjectCustom(T entity, int mode)
        {
            return null;
        }
        public int Update(Guid id, T entity)
        {
            int mode = 0;
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                // Kiểu dữ liệu của prop
                var propType = prop.PropertyType;
                var isPrimaryKey = prop.IsDefined(typeof(PrimaryKey), true);
                if (isPrimaryKey == true)
                {
                    // Set id từ url vào id của entity 
                    prop.SetValue(entity, id);
                }

            }
            // Thực hiện sửa dữ liệu
            var isValid = ValidateObject(entity, mode);
            if (isValid == true && (ValidateErrorMsgs == null || ValidateErrorMsgs.Count() == 0))
            {
                return _baseRepository.Update(id, entity);

            }
            else
            {
                var errorService = new ErrorService();
                errorService.UserMsg = Resources.ResourceVN.Error_Validate;
                errorService.Data = ValidateErrorMsgs;
                throw new MISAValidateException(Resources.ResourceVN.Error_Validate, ValidateErrorMsgs);
            }
        }
    }
}
