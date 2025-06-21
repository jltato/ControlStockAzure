using ControlStock.Data;
using System.ComponentModel.DataAnnotations;

namespace ControlStock.Models;

public partial class Marca
{
    public int IdMarca { get; set; }

    public string MarcaName { get; set; } = null!;

    [Range(1, int.MaxValue, ErrorMessage = "El Rubro es obligatorio.")]
    public int IdRubro { get; set; }

    public bool EliminadoLogico { get; set; } = false;

    public string UserId { get; set; } = null!;
    public MyUser? User { get; set; } = null!;

    public virtual ICollection<Articulo> Articulos { get; set; } = new List<Articulo>();

    public virtual Rubro? Rubro { get; set; } = null!;
}
