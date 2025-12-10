using OrdinarioWebDefinitivoAhora.Models;
using System.ComponentModel.DataAnnotations;

namespace OrdinarioWebDefinitivoAhora.ViewModels
{
    public class MemoViewModel
    {
        public int Id { get; set; }
        public int Folio { get; set; }
        public int Año { get; set; }
        public string? De { get; set; }
        public string? Para { get; set; }
        public string? Asunto { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
        public string? Estatus { get; set; } // Activo o Cancelado
        public string? UsuarioRegistro { get; set; }
        public ICollection<CancelacionModel>? Cancelaciones { get; set; }
    }
}
