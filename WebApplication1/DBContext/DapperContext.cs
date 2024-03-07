using System.Data.SqlClient;
using System.Data;

namespace WebApplication1.DBContext
{
    public class DapperContext
    {
        private readonly IConfiguration _configuration;

        private readonly string _connectionString;

        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            //SqlConnection es la cadena de conexion que se encuentra en appsettings.json
            _connectionString = _configuration.GetConnectionString("todoAppDBCon");
        }

        //Crea una nueva instancia de SqlConnection con la cadena de conexión 
        public IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

    }
}

