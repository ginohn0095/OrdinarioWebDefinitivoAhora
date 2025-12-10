using System.ComponentModel.DataAnnotations;

namespace OrdinarioWebDefinitivoAhora.Models
{
    
    public enum FolioEstado
    {
        Disponible,
        Usado,
        Ocupado,
        Bloqueado
    }

    public class FolioModel
    {
        public int Id { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Number { get; set; } // 1..500

        [Required]
        public FolioEstado Estado { get; set; } = FolioEstado.Disponible;

        // Relación uno a uno (principal)
        public MemoModel? Memo { get; set; }
    }
}
