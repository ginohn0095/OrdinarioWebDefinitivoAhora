using System.ComponentModel.DataAnnotations;

namespace OrdinarioWebDefinitivoAhora.Models
{
    public class CancelacionModel
    {
        public int Id { get; set; }

        // Memo al que pertenece la cancelación
        public int MemoId { get; set; }
        public MemoModel? Memo { get; set; }

        // Fecha y hora de la cancelación
        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Explicación de por qué se cancela
        [Required]
        public string Motivo { get; set; } = string.Empty;

        // Usuario que canceló
        [Required]
        public string UsuarioCancela { get; set; } = string.Empty;
    }
}
