using ControlStock.Data;
using ControlStock.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlStock.Controllers
{
    public class StockService
    {
        private readonly MyDbContext _context;

        public StockService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AgregarStockAsync(int depositoId, int articuloId, int loteId, int cantidad)
        {
            var depositoArticuloLote = await _context.DepositoArticuloLotes
                .FirstOrDefaultAsync(dal => dal.ScopeId == depositoId && dal.ArticuloId == articuloId && dal.LoteId == loteId);

            if (depositoArticuloLote != null)
            {
                depositoArticuloLote.Cantidad += cantidad;
            }
            else
            {
                depositoArticuloLote = new DepositoArticuloLote
                {
                    ScopeId = depositoId,
                    ArticuloId = articuloId,
                    LoteId = loteId,
                    Cantidad = cantidad
                };
                _context.DepositoArticuloLotes.Add(depositoArticuloLote);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DescontarStockAsync(int depositoId, int articuloId, int loteId, int cantidad)
        {
            var depositoArticuloLote = await _context.DepositoArticuloLotes
                .FirstOrDefaultAsync(dal => dal.ScopeId == depositoId && dal.ArticuloId == articuloId && dal.LoteId == loteId);

            if (depositoArticuloLote == null || depositoArticuloLote.Cantidad < cantidad)
            {
                return false; // No hay suficiente stock para descontar
            }

            depositoArticuloLote.Cantidad -= cantidad;

            if (depositoArticuloLote.Cantidad == 0)
            {
                _context.DepositoArticuloLotes.Remove(depositoArticuloLote);
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }

}
