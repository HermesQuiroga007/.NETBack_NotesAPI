using Dapper;
using WebApplication1.DBContext;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            var consulta = @"INSERT INTO users
                            (email, password) VALUES (@Email, @Password);

                            SELECT SCOPE_IDENTITY();";

            using (var connection = _context.CreateConnection())
            {
                // Ejecutar la consulta SQL y obtener el ID generado por la base de datos
                var nuevoId = await connection.QuerySingleAsync<int>(consulta, user);

                //// Asignar el ID generado al usuario
                //user.ideUser = nuevoId;

                // Devolver el usuario
                return user;
            }
        }

        public User CheckCredentials(User user)
        {
            using (var connection = _context.CreateConnection())
            {
                connection.Open();
                var query = "SELECT * FROM users WHERE email = @Email";
                return connection.QueryFirstOrDefault<User>(query, user);
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var consulta = @"SELECT * FROM users as u
            where u.email = @Email;";

            using (var connection = _context.CreateConnection())
            {
                var usuario = await connection.QuerySingleOrDefaultAsync<User>(consulta, new { Email = email });
                return usuario;
            }
        }

        public async Task<bool> RegisterUser(User user)
        {
            // Verificar si el usuario ya existe

            var existingUser = await GetUserByEmail(user.email);
            if (existingUser != null)
            {
                // El usuario ya existe
                return false;
            }

            // Hashear la contraseña antes de guardarla en la base de datos
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
            user.password = passwordHash;

            // Guardar el usuario en la base de datos
            var result = await AddUser(user);

            // Devolver verdadero si se guardó correctamente
            return result != null;

        }
    }
}
