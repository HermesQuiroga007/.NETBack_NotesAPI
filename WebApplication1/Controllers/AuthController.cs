using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

[Route("/api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenManager _tokenManager;
    private readonly IUserRepository _userRepository;

    public AuthController(ITokenManager tokenManager, IUserRepository userRepository)
    {
        _tokenManager = tokenManager;
        _userRepository = userRepository;
    }

    [HttpPost("RegisterUser")]
    public async Task<ActionResult> Create(User user)
    {
        // Validar si el email es una dirección de correo electrónico válida
        if (!IsValidEmail(user.email))
        {
            return BadRequest("El correo electrónico no es válido");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
        user.password = passwordHash;

        var result = await _userRepository.RegisterUser(user);

        if (result)
        {
            // Calcular la fecha de expiración del token
            var expirationDate = DateTime.Now.AddMinutes(0.5);

            var authResponse = new AuthResponse() { Expiracion = expirationDate, IsAuthenticated = true, Role = "Dummy Role", Token = _tokenManager.GenerateToken(user) };
            return Ok(authResponse);
        }
        else if (!result)
        {
            return Conflict("El usuario ya existe");
        }
        else
        {
            return BadRequest("Error al registrar el usuario");
        }
    }

    private bool IsValidEmail(string email)
    {
        // Expresión regular para validar direcciones de correo electrónico
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        Regex regex = new Regex(pattern);

        return regex.IsMatch(email);
    }

    [HttpPost("LoginUser")]
    public ActionResult<AuthResponse> GetDetails(User user)
    {
        var authUser = _userRepository.CheckCredentials(user);
        if (authUser == null)
        {
            return NotFound();
        }

        // Calcular la fecha de expiración del token
        var expirationDate = DateTime.Now.AddMinutes(1);

        var authResponse = new AuthResponse()
        {
            IsAuthenticated = true,
            Role = "Dummy Role",
            Expiracion = expirationDate,
            Token = _tokenManager.GenerateToken(authUser)
        };

        return Ok(authResponse);
    }

    [HttpPost("ExtendSession")]
    public async Task<ActionResult> ExtendSession(string userEmail)
    {
        // Aquí debes verificar si el userEmail es válido y corresponde a un usuario en tu sistema
        // Puedes hacer esto consultando tu base de datos u otro medio de almacenamiento seguro

        // Por simplicidad, aquí asumiré que el userEmail es válido y se ha asociado con un usuario
        var user = await _userRepository.GetUserByEmail(userEmail);

        if (user == null)
        {
            return BadRequest("Correo electrónico inválido");
        }

        // Generar un nuevo token de sesión para el usuario
        var newSessionToken = _tokenManager.GenerateToken(user);

        // Devolver el nuevo token de sesión
        return Ok(new { SessionToken = newSessionToken });
    }
}