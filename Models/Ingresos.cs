using ControlStock.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ControlStock.Models
{
    public class Ingresos
    {
        [Key]
        public int IngresoId { get; set; }
        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime FechaIngreso { get; set; }
        [Required(ErrorMessage = "El proveedor es obligatorio")]
        [ForeignKey(nameof(ProveedorId))]
        public int? ProveedorId { get; set; }
        public string? Comprobante { get; set; } = string.Empty;
        public string? Observaciones { get; set; } = string.Empty;
        public int ScopeId { get; set; }
        public string? UserId { get; set; }
        public bool EliminadoLogico { get; set; } = false;
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public Proveedor? Proveedor { get; set; }
        public virtual Scope? Scope { get; set; } = null!;
        public IEnumerable<DetalleIngreso>? DetalleIngresos { get; set; } 
    }
}
