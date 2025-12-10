using System.ComponentModel.DataAnnotations;

namespace OrdinarioWebDefinitivoAhora.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        public string NumEmpleado { get; set; } = string.Empty; // identificador único (ej: 1001)

        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User"; // "Admin" o "User"

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // contraseña hasheada

        // campos opcionales
        public bool IsActive { get; set; } = true;
    }
}
