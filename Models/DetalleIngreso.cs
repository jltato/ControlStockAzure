using System;
using System.Collections.Generic;

namespace ControlStock.Models;

public partial class DetalleIngreso
{
    public int DetalleId { get; set; }

    public int? IngresoId { get; set; }

    public int ArticuloId { get; set; }

    public int? LoteId { get; set; } = null;

    public int Cantidad { get; set; }

    public virtual Articulo? Articulo { get; set; } = null!;

    public virtual Ingresos? Ingresos { get; set; } = null!;

    public virtual Lote? Lote { get; set; }

 
}
