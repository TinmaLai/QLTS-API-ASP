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
    public class FixedAssetCategoryService : BaseService<FixedAssetCategory>, IFixedAssetCategoryService
    {
        IFixedAssetCategoryRepository _fixedAssetCategoryRepository;
        public FixedAssetCategoryService(IFixedAssetCategoryRepository fixedAssetCategoryRepository) : base(fixedAssetCategoryRepository)
        {
            _fixedAssetCategoryRepository = fixedAssetCategoryRepository;
        }
        public int Insert(FixedAssetCategory entity)
        {
            throw new NotImplementedException();
        }

        public int Update(Guid id, FixedAssetCategory entity)
        {
            throw new NotImplementedException();
        }
    }
}
