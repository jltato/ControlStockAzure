using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlStock.Models;

public partial class Lote
{
    public int LoteId { get; set; }

    public DateTime? FechaElaboracion { get; set; }

    public DateTime? FechaVencimiento { get; set; }

    [Required(ErrorMessage ="El Numero de Lote es obligatorio")]
    public string NumeroLote { get; set; } = null!;

    public virtual ICollection<DepositoArticuloLote>? DepositoArticuloLotes { get; set; } = new List<DepositoArticuloLote>();

    public virtual ICollection<DetalleIngreso> DetalleIngresos { get; set; } = new List<DetalleIngreso>();
}
