using System;
using System.Collections.Generic;

namespace ControlStock.Models;

public partial class Section
{
    public int SectionId { get; set; }

    public string Name { get; set; } = null!;

    public string Abreviatura { get; set; } = null!;

    public virtual ICollection<Rubro>? Rubros { get; set; } = new List<Rubro>();

    public virtual ICollection<UserPermission>? UserPermissions { get; set; } = new List<UserPermission>();

    public virtual ICollection<SectionProveedor>? SectionProveedores { get; set; } = new List<SectionProveedor>();

    public virtual ICollection<Egresos>? Egresos { get; set; } = new List<Egresos>();
}
