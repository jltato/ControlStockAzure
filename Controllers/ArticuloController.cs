using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlStock.Data;
using ControlStock.Models;
using Microsoft.AspNetCore.Identity;

namespace ControlStock.Controllers
{
    public class ArticuloController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<MyUser> _userManager;
        private readonly ILogger<ArticuloController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ArticuloController(MyDbContext context, UserManager<MyUser> userManager, ILogger<ArticuloController> logger, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Articulo
        public async Task<IActionResult> Index()
        {

            // Obtener el usuario logueado
            var user = await _userManager.GetUserAsync(User);
            var sections = await _context.UserPermissions
                .Where(a => a.UserId == user.Id)
                .Select(a => a.SectionId)
                .ToListAsync();
                
            var articulos = await _context.Articulos
            .Include(a => a.Marca)
            .ThenInclude(m => m.Rubro)
            .Where(a => sections.Contains(a.Marca.Rubro.SectionId))
            .ToListAsync();

            return View(articulos);
        }

        // GET: Articulo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articulo = await _context.Articulos
                .Include(a => a.Marca)
                .ThenInclude(m => m.Rubro)
                .FirstOrDefaultAsync(m => m.IdArticulo == id);
            if (articulo == null)
            {
                return NotFound();
            }

            return View(articulo);
        }

        // GET: Articulo/Create
        public async Task<IActionResult> Create(string returnUrl=null!)
        {
            // Capturar la URL de la página anterior
            string refererUrl = Request.Headers.Referer.ToString();

            // Validar que refererUrl no sea nulo o vacío y asignarlo a returnUrl si es una URL válida
            if (!string.IsNullOrEmpty(refererUrl) && Uri.IsWellFormedUriString(refererUrl, UriKind.Absolute))
            {
                returnUrl = new Uri(refererUrl).AbsolutePath;
            }

            // Asignar la URL predeterminada si returnUrl sigue siendo nulo
            returnUrl ??= Url.Content("~/");

            var user = await _userManager.GetUserAsync(User);

            var rubros = await _context.Rubros
                        .Where(rubro => _context.UserPermissions
                            .Where(up => up.UserId == user.Id)
                            .Select(up => up.SectionId)
                            .Contains(rubro.SectionId) &&
                                rubro.EliminadoLogico == false)
                        .ToListAsync();

            ViewData["UserId"] = user.Id;
            ViewData["Rubro"] = new SelectList(rubros, "IdRubro", "Name");
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Articulo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdArticulo,Nombre,Descripcion,Activo,IdMarca,StockMin,UserId,Observaciones")] Articulo articulo, String? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Articulo/index");
            var user = articulo.UserId;
            if (ModelState.IsValid)
            {
                articulo.Nombre = articulo.Nombre.ToUpper();
                articulo.Descripcion = articulo.Descripcion?.ToUpper();
                articulo.Observaciones = articulo.Observaciones?.ToUpper();
                _context.Add(articulo);
                await _context.SaveChangesAsync();
                return Redirect(returnUrl);
            }
            var rubros = await _context.Rubros
                .Include(a => a.Section)
                .ThenInclude(a => a.UserPermissions)
                .Where(up => up.UserId == user && up.EliminadoLogico == false)
                .ToListAsync();

            ViewData["UserId"] = user;
            ViewData["Rubro"] = new SelectList(rubros, "IdRubro", "Name");
            return View(articulo);
        }

        // GET: Articulo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);                   

            if (id == null)
            {
                return NotFound();
            }

            var articulo = await _context.Articulos
                    .Include(a => a.Marca)   // Incluir Marca
                    .ThenInclude(m => m.Rubro) // Incluir Rubro a través de Marca
                    .FirstOrDefaultAsync(m => m.IdArticulo == id);

            if (articulo == null)
            {
                return NotFound();
            }
            var rubros = await _context.Rubros
                        .Where(rubro => _context.UserPermissions
                            .Where(up => up.UserId == user.Id)
                            .Select(up => up.SectionId)
                            .Contains(rubro.SectionId) &&
                                rubro.EliminadoLogico == false)
                        .ToListAsync();

            var marcas = await _context.Marcas
                .Where(up => up.IdRubro == articulo.Marca.IdRubro)
                .ToListAsync();

            ViewData["UserId"] = user.Id;
            ViewData["IdRubro"] = new SelectList(rubros, "IdRubro", "Name",articulo.Marca.IdRubro);
            ViewData["IdMarca"] = new SelectList(marcas, "IdMarca", "MarcaName", articulo.IdMarca);
            return View(articulo);
        }

        // POST: Articulo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdArticulo,Nombre,Descripcion,Activo,IdMarca,StockMin,UserId,Observaciones")] Articulo articulo)
        {
            if (id != articulo.IdArticulo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    articulo.Nombre = articulo.Nombre.ToUpper();
                    articulo.Descripcion = articulo.Descripcion?.ToUpper();
                    articulo.Observaciones = articulo.Observaciones?.ToUpper();
                    _context.Update(articulo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticuloExists(articulo.IdArticulo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdMarca"] = new SelectList(_context.Marcas, "IdMarca", "MarcaName", articulo.IdMarca);
            ViewData["IdRubro"] = new SelectList(_context.Rubros, "IdRubro", "Name", articulo.Marca?.Rubro?.IdRubro);
            return View(articulo);
        }

        // GET: Articulo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articulo = await _context.Articulos
                .Include(a => a.Marca)
                .ThenInclude(m => m.Rubro)
                .FirstOrDefaultAsync(m => m.IdArticulo == id);
            if (articulo == null)
            {
                return NotFound();
            }

            return View(articulo);
        }

        // POST: Articulo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articulo = await _context.Articulos.FindAsync(id);
            if (articulo != null)
            {
                _context.Articulos.Remove(articulo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticuloExists(int id)
        {
            return _context.Articulos.Any(e => e.IdArticulo == id);
        }

        // GET: Articulo/GetMarcasPorRubro
        public async Task<IActionResult> GetMarcasPorRubro(int idRubro)
        {

            var marcas = await _context.Marcas
                .Where(m => m.IdRubro == idRubro && m.EliminadoLogico == false) 
                .OrderBy(a => a.MarcaName)
                .ToListAsync();

            var articulos = await _context.Articulos
                .Where(a => a.EliminadoLogico == false && a.Marca.IdRubro == idRubro)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
            var result = new
            {
                Marcas = marcas,
                Articulos = articulos
            };
            return Json(result);
        }

        // GET: Articulo/GetArticulosPorMarca
        public async Task<IActionResult> GetArticulosPorMarca(int idMarca)
        {

            var articulo = await _context.Articulos
                .Where(m => m.IdMarca == idMarca && m.EliminadoLogico == false)
                .OrderBy(m => m.Nombre)
                .ToListAsync();

            return Json(articulo);
        }

        // GET: Articulo/GetLotesPorArticulo
        public async Task<IActionResult> GetLotesPorArticulo(int idArticulo)
        {
            try
            {
                var lotes = await _context.Lotes
                        .Include(l => l.DepositoArticuloLotes)
                        .Where(m => m.DepositoArticuloLotes
                            .Any(dal => dal.ArticuloId == idArticulo && dal.Cantidad != 0))
                         .Select(m => new
                         {
                             m.NumeroLote,
                             m.LoteId,
                         })
                        .ToListAsync();

                var marca = await _context.Articulos
                            .Where(a => a.IdArticulo == idArticulo)
                            .Select(a => a.IdMarca)
                            .FirstOrDefaultAsync();

                var result = new
                {
                    Lotes = lotes,
                    Marca = marca
                };
                return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }           
        }

        //GET: Articulo/GetCantidadPorLote
        public async Task<IActionResult> GetCantidadPorLote(int idLote, int idDeposito, int idArticulo)
        {
            
            try
            {
             int cantidad = await _context.DepositoArticuloLotes
             .Where(a => (idLote == 0 ? a.LoteId == null : a.LoteId == idLote) && a.ArticuloId == idArticulo && a.ScopeId == idDeposito)
             .Select(c => c.Cantidad)
             .FirstOrDefaultAsync();

                return Json(new { success = true, cantidad });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la cantidad por lote.");
                // Retornar respuesta JSON de error
                return Json(new { success = false, message = "Error al obtener la cantidad." });
            }
        }
    }
}
