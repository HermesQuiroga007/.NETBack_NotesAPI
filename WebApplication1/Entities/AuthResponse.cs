namespace WebApplication1.Entities
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Role { get; set; }

    }
}
