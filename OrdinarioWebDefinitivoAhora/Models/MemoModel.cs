using System.ComponentModel.DataAnnotations;

namespace OrdinarioWebDefinitivoAhora.Models
{
    public class MemoModel
    {
        public int Id { get; set; }

        // Foreign Key obligatoria
        public int FolioId { get; set; }
        public FolioModel Folio { get; set; } = null!;

        // Datos del memorándum
        [Required(ErrorMessage = "El campo 'De' es obligatorio")]
        [StringLength(100)]
        public string De { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo 'Para' es obligatorio")]
        [StringLength(100)]
        public string Para { get; set; } = string.Empty;

        [Required]
        public string Asunto { get; set; } = string.Empty;

        [Required]
        [StringLength(5000)]
        public string Contenido { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; }
        public string? Estatus { get; set; }
        public string? UsuarioRegistro { get; set; }

        public ICollection<CancelacionModel>? cancelados { get; set; }
    }
}
