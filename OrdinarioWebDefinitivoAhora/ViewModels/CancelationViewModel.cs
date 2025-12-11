using OrdinarioWebDefinitivoAhora.Models;
using System.ComponentModel.DataAnnotations;

namespace OrdinarioWebDefinitivoAhora.ViewModels
{
    public class CancelationViewModel
    {

        public int Id { get; set; }
        public int MemoId { get; set; }           
       
        public string UsuarioCancelo { get; set; } = string.Empty;
        public string MotivoCancelacion { get; set; } = string.Empty;
        public DateTime FechaCancelacion { get; set; }
    }
}
