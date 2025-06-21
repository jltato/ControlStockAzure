namespace ControlStock.Models
{
    public class DetalleEgreso
    {
        public int DetalleId { get; set; }

        public int? EgresoId { get; set; }

        public int ArticuloId { get; set; }

        public int? LoteId { get; set; } = null;

        public int Cantidad { get; set; }

        public virtual Articulo? Articulo { get; set; } = null!;

        public virtual Egresos? Egreso { get; set; } = null!;

        public virtual Lote? Lote { get; set; }
    }
}