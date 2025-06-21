using System;
using System.Collections.Generic;

namespace ControlStock.Models;

public partial class Scope
{
    public int ScopeId { get; set; }

    public string ScopeName { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public bool EliminadoLogico { get; set; }

    public string UserId { get; set; } = null!;

    public virtual ICollection<DepositoArticuloLote> DepositoArticuloLotes { get; set; } = new List<DepositoArticuloLote>();

    public virtual ICollection<Ingresos> Ingresos { get; set; } = new List<Ingresos>();
    public virtual ICollection<Egresos> Egresos { get; set; } = new List<Egresos>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}
