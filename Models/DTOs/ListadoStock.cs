namespace ControlStock.Models.DTOs
{
    public class ListadoStock
    {
        public int ArticuloId { get; set; }
        public string Articulo { get; set; }
        public string Marca { get; set; }
        public string Rubro { get; set; }
        public int Cantidad { get; set; }    
    }
}
