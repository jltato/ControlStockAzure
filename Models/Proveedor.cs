using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlStock.Models;

public partial class Proveedor
{
    public int ProveedorId { get; set; }

    [Required(ErrorMessage ="El Nombre del Proveedor es Obligatorio")]
    public string ProveedorName { get; set; } = null!;

    public string? ProveedorAdress { get; set; }

    public string? ProveedorPhone { get; set; }

    public string? ProveedorMail { get; set; }

    public bool EliminadoLogico { get; set; } = false; 

    public string UserId { get; set; } = string.Empty;

    public DateTime? FechaAlta { get; set; } = DateTime.Now;

    public virtual ICollection<Ingresos>? Ingresos { get; set; } = new List<Ingresos>();

    public virtual ICollection<SectionProveedor>? SectionProveedores { get; set; } = new List<SectionProveedor>();
}
