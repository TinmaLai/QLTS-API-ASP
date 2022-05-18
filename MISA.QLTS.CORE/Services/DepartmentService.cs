using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MISA.QLTS.CORE.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.CORE.Services
{
    public class DepartmentService : BaseService<Department>, IDepartmentService
    {
        IDepartmentRepository _departmentRepository;
        public DepartmentService(IDepartmentRepository departmentRepository) : base(departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }
        protected override List<string> ValidateObjectCustom(Department entity, int mode)
        {
            if(String.IsNullOrEmpty(entity.DepartmentName) == true)
            {
                ValidateErrorMsgs.Add(Resources.ResourceVN.DepartmentNameValidate);
            }
            
            return ValidateErrorMsgs;
        }
        public int Insert(Department entity)
        {
            throw new NotImplementedException();
        }

        public int Update(Guid id, Department entity)
        {
            throw new NotImplementedException();
        }
    }
}
