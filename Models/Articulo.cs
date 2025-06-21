using System.ComponentModel.DataAnnotations;

namespace ControlStock.Models;

public partial class Articulo
{
    public int IdArticulo { get; set; }

    [Required(ErrorMessage = "El Nombre es Obligatorio")]
    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool Activo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La Marca es obligatoria.")]
    public int IdMarca { get; set; }

    public int? StockMin { get; set; }

    public bool EliminadoLogico { get; set; } = false;
    public string UserId { get; set; } = null!;

    public string? Observaciones { get; set; } = string.Empty;

    public virtual ICollection<DepositoArticuloLote> DepositoArticuloLotes { get; set; } = new List<DepositoArticuloLote>();

    public virtual ICollection<DetalleIngreso> DetalleIngresos { get; set; } = new List<DetalleIngreso>();

    public virtual Marca? Marca { get; set; } = null!;
}
