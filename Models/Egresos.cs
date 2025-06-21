using System.ComponentModel.DataAnnotations;

namespace ControlStock.Models
{
    public class Egresos
    {
        public int EgresoId { get; set; }
        [Required(ErrorMessage ="La fecha es obligatoria")]
        public DateTime FechaEgreso { get; set; }

        [Required(ErrorMessage ="El destino es obligatorio")]
        [Range(1,9999,ErrorMessage ="Seleccione una opción")]
        public int? Destino { get; set; }
        public string? Observaciones { get; set; }
        public int ScopeId { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string UserId { get; set; } = string.Empty;
        public bool EliminadoLogico { get; set; } = false;

        public int SectionId { get; set; }

        public Section? Section { get; set; }

        public Scope? Scope { get; set; } // Relación de navegación para ScopeId

        public Scope? DestinoScope { get; set; } // Relación de navegación para Destino
        public IEnumerable<DetalleEgreso>? DetalleEgresos { get; set; }
    }
}
