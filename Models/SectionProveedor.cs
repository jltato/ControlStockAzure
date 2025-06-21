using ControlStock.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlStock.Models
{
    public class SectionProveedor
    {
        public int ProveedorId { get; set; }
        public Proveedor? Proveedor { get; set; }

        public int SectionId { get; set; }
        public Section? Section { get; set; }
    }
}
