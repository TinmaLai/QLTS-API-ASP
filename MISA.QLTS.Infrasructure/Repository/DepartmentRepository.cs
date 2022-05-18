using Dapper;
using Microsoft.Extensions.Configuration;
using MISA.QLTS.CORE.Entities;
using MISA.QLTS.CORE.Interfaces.Repositories;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.QLTS.Infrasructure.Repository
{
    public class DepartmentRepository: BaseRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(IConfiguration configuration) : base(configuration)
        {

        }
        public bool CheckCodeDuplicate(Guid id, string code, int mode)
        {
            throw new NotImplementedException();
        }

        public int Delete(Guid id)
        {
            throw new NotImplementedException();
        }


        public Department GetById(Guid assetId)
        {
            throw new NotImplementedException();
        }

        public string getNewCode()
        {
            throw new NotImplementedException();
        }

        public List<Department> getPaging(int pageIndex, int pageSize)
        {
            throw new NotImplementedException();
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
