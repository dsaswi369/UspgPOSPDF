using Microsoft.AspNetCore.Identity;

namespace UspgPOS.Models
{
    public class Usuario: IdentityUser
    {
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }
}
