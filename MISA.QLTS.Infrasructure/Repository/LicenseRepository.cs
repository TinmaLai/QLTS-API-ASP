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
    public class LicenseRepository: BaseRepository<License>, ILicenseRepository
    {
        IConfiguration _configuration;
        readonly string _connectionString = string.Empty;
        protected MySqlConnection _sqlConnection;
        public LicenseRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("NBTIN");
            _sqlConnection = new MySqlConnection(_connectionString);
        }

        
    }
}
