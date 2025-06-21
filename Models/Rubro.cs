using ControlStock.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlStock.Models;

public partial class Rubro
{
    public int IdRubro { get; set; }

    [Required(ErrorMessage ="El Nombre es obligatorio")]
    public string Name { get; set; } = null!;

    public int SectionId { get; set; }

    public bool EliminadoLogico { get; set; } = false;

    public string UserId { get; set; } = null!;

    public MyUser? User { get; set; } = null!;

    public virtual ICollection<Marca> Marcas { get; set; } = new List<Marca>();

    public virtual Section? Section { get; set; } = null!;

}
