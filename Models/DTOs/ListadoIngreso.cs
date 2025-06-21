namespace ControlStock.Models.DTOs
{
    public class ListadoIngreso
    {
        public int ArticuloId { get; set; }
        public string Articulo { get; set; } 
        public string Marca { get; set; }
        public int? LoteId { get; set; }
        public string? Lote { get; set; } 
        public int Cantidad { get; set; }
    }
}
