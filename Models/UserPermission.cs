using ControlStock.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlStock.Models;

public partial class UserPermission
{

    public String UserId { get; set; } = string.Empty;
    
    public int SectionId { get; set; }

    public int ScopeId { get; set; }

    public virtual MyUser User { get; set; } = null!;

    public virtual Scope Scope { get; set; } = null!;

    public virtual Section Section { get; set; } = null!;
}
