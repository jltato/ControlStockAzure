using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlStock.Models;

public partial class DepositoArticuloLote
{
    public int DepositoArticuloLoteId { get; set; }

    public int? LoteId { get; set; }

    [Required(ErrorMessage ="La cantidad es obligatoria")]
    public int Cantidad { get; set; }

    public int ArticuloId { get; set; }

    public int? ScopeId { get; set; }

    public virtual Articulo? Articulo { get; set; } = null!;

    public virtual Lote? Lote { get; set; }

    public virtual Scope? Scope { get; set; }
}
