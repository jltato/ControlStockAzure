using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlStock.Models;
using ControlStock.Data;
using Microsoft.AspNetCore.Identity;
using ControlStock.Models.DTOs;
using Newtonsoft.Json.Linq;

namespace ControlStock.Controllers
{
    public class EgresosController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ILogger<IngresosController> _logger;
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public EgresosController(MyDbContext context, ILogger<IngresosController> logger, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Egresos
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var depositos = await _context.Scopes                        
                        .Where(s => _context.UserPermissions
                            .Any(up => up.UserId == user.Id && up.ScopeId == s.ScopeId && up.Scope.EliminadoLogico == false))
                        .ToListAsync();

            ViewBag.depositos = new SelectList(depositos, "ScopeId", "ScopeName");
            return View();
        }

        // GET: Egresos/Details/5
        public async Task<IActionResult> Details(int? id)
        {                              
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var egresos = await _context.Egresos
                    .Include(e => e.Scope)
                    .Include(a => a.DestinoScope)
                    .Include(d => d.DetalleEgresos)
                        .ThenInclude(de => de.Articulo)
                    .Include(i => i.DetalleEgresos)
                        .ThenInclude(di => di.Lote)
                    .FirstOrDefaultAsync(m => m.EgresoId == id);

                if (egresos == null)
                {
                    return NotFound();
                }
                // Pasar la lista de productos a ViewBag
                ViewBag.Productos = egresos.DetalleEgresos;
                return View(egresos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return View();
        }
        [HttpGet]
        // GET: Egresos/Create
        public async Task<IActionResult> Create()

        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await SetViewBagData(user);
               
                          
            }
;
          
            return View();
        }

        // POST: Egresos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Egresos egresos, List<ListadoIngreso> detalleEgresos)
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {                
                if (ModelState.IsValid)
                {
                    // Añadir el nuevo ingreso al contexto
                    _context.Add(egresos);
                    // Guardar los cambios en la base de datos para obtener el IngresoId
                    await _context.SaveChangesAsync();

                    foreach (var detalle in detalleEgresos)
                    {
                        // Buscar el artículo en DepositoArticuloLote
                        var depositoArticuloLote = _context.DepositoArticuloLotes
                            .FirstOrDefault(dal => dal.ArticuloId == detalle.ArticuloId
                                                    && dal.ScopeId == egresos.ScopeId
                                                    && dal.LoteId == detalle.LoteId);

                        if (depositoArticuloLote != null)
                        {
                            // Si el artículo  existe en el depósito y lote, actualizar la cantidad
                            depositoArticuloLote.Cantidad -= detalle.Cantidad;
                        }


                        // Añadir el detalle al contexto y establecer la relación con el ingreso
                        //detalle.IngresoId = ingreso.IngresoId;
                        //_context.DetalleIngreso.Add(detalle);
                    }

                    // Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    var printUrl = Url.Action("index", "Reportes", new { id = egresos.EgresoId }, protocol: Request.Scheme);
                    TempData["PrintUrl"] = printUrl;

                    // Redirigir a la acción Index
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Manejo del caso en que el modelo no es válido
                   
                    await SetViewBagData(user);
                    ViewBag.DetalleEgreso = detalleEgresos;
                    return View(egresos);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el ingreso con ID: {IngresoId}", egresos.EgresoId);
                await SetViewBagData(user);
                ViewBag.DetalleEgreso = detalleEgresos;
                return View(egresos);
            }
        }

        // GET: Egresos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egresos = await _context.Egresos.FindAsync(id);
            if (egresos == null)
            {
                return NotFound();
            }
            ViewData["ScopeId"] = new SelectList(_context.Scopes, "ScopeId", "ScopeId", egresos.ScopeId);
            return View(egresos);
        }

        // POST: Egresos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EgresoId,FechaEgreso,ScopeId,Observaciones,Timestamp,UserId,EliminadoLogico")] Egresos egresos)
        {
            if (id != egresos.EgresoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(egresos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EgresosExists(egresos.EgresoId))
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
            ViewData["ScopeId"] = new SelectList(_context.Scopes, "ScopeId", "ScopeId", egresos.ScopeId);
            return View(egresos);
        }

        // GET: Egresos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var egresos = await _context.Egresos
                .Include(e => e.Scope)
                .Include(a => a.DestinoScope)
                .FirstOrDefaultAsync(m => m.EgresoId == id);
            if (egresos == null)
            {
                return NotFound();
            }

            return View(egresos);
        }

        // POST: Egresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var egresos = await _context.Egresos.FindAsync(id);
            if (egresos != null)
            {
                egresos.EliminadoLogico = true;
                _context.Update(egresos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EgresosExists(int id)
        {
            return _context.Egresos.Any(e => e.EgresoId == id);
        }

        private async Task SetViewBagData(MyUser user)
        {
            var rubros = await _context.Rubros
                    .Where(rubro => _context.UserPermissions
                        .Where(up => up.UserId == user.Id)
                        .Select(up => up.SectionId)
                        .Contains(rubro.SectionId) &&
                            rubro.EliminadoLogico == false)
                    .ToListAsync();

            ViewBag.UserID = user.Id;
            ViewBag.Rubros = new SelectList(rubros, "IdRubro", "Name");

            var depositos = await _context.Scopes
                .Where(s => _context.UserPermissions
                    .Any(up => up.UserId == user.Id && up.ScopeId == s.ScopeId && up.Scope.EliminadoLogico == false))
                .ToListAsync();

            var destinos = await _context.Scopes
                            .Where(a => a.ScopeId != 1)
                            .ToListAsync();

            var section = await _context.UserPermissions                  
                   .Where(b => b.UserId == user.Id)
                   .FirstOrDefaultAsync();

            ViewData["section"] = section?.SectionId;
            ViewData["depositos"] = new SelectList(depositos, "ScopeId", "ScopeName");
            ViewData["destinos"] = new SelectList(destinos, "ScopeId", "ScopeName");
        }


            //POST: Egresos/DatosTable
            [HttpPost]
        public async Task<IActionResult> DatosTable(int ScopeId)
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                // Obtener parámetros de la solicitud
                var searchValue = Request.Form["search[value]"].FirstOrDefault() ?? "";
                var start = int.Parse(Request.Form["start"].FirstOrDefault() ?? "0");
                var length = int.Parse(Request.Form["length"].FirstOrDefault() ?? "0");
                var orderColumnIndex = int.Parse(Request.Form["order[0][column]"].FirstOrDefault() ?? "0");
                var orderDirection = Request.Form["order[0][dir]"] == "asc" ? "OrderBy" : "OrderByDescending";
                var draw = int.Parse(Request.Form["draw"].FirstOrDefault() ?? "0");

                // Query original
                var allData = await _context.Egresos
                            .Where(egreso => !egreso.EliminadoLogico &&
                                             _context.UserPermissions.Any(up => up.ScopeId == egreso.ScopeId && up.UserId == user.Id))
                            .Where(egreso => _context.UserPermissions.Any(up => up.SectionId == egreso.SectionId && up.UserId == user.Id))
                            .Include(e => e.Scope) // Cargar Scope por el ScopeId
                            .Include(e => e.DestinoScope) // Cargar Scope por el Destino
                            .Include(e => e.DetalleEgresos) // Cargar detalles de egreso
                             .Where(s => s.Scope.ScopeId == ScopeId)
                            .ToListAsync();


                // Contar registros totales (sin paginación)
                var totalRecords = allData.Count();

                //// Obtener todos los datos antes de aplicar el filtrado
                //var allData = await query.ToListAsync();

                // Aplicar filtrado en memoria
                if (!string.IsNullOrEmpty(searchValue))
                {
                    searchValue = searchValue.ToLower(); // Convertir a minúsculas para una búsqueda sin distinción de mayúsculas/minúsculas
                    allData = allData.Where(i =>
                        i.EgresoId.ToString().Contains(searchValue) ||
                        (i.Scope != null && i.DestinoScope.ScopeName.ToLower().Contains(searchValue)) ||
                        i.FechaEgreso.ToString("dd/MM/yyyy").Contains(searchValue) // Ajusta el formato de fecha según necesites
                    ).ToList();
                }

                // Contar registros filtrados
                var totalRecordsFiltered = allData.Count;

                // Aplicar ordenación en memoria
                switch (orderColumnIndex)
                {
                    case 0:
                        allData = allData.OrderByDescending(e => e.FechaEgreso).ToList();
                        break;
                    case 1: //  ProveedorName
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.DestinoScope?.ScopeName).ToList()
                            : allData.OrderByDescending(i => i.DestinoScope?.ScopeName).ToList();
                        break;                    
                    case 2: //  Comprobante
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.EgresoId).ToList()
                            : allData.OrderByDescending(i => i.EgresoId).ToList();
                        break;                    
                    default:
                        allData = allData.OrderByDescending(e => e.FechaEgreso).ToList(); // Orden por defecto
                        break;
                }

                // Paginación
                var paginatedResult = allData
                    .Skip(start)
                    .Take(length)
                    .ToList();

                // Devolver datos en el formato esperado por DataTables
                var json = Json(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecordsFiltered,
                    data = paginatedResult
                });

                Console.Write(json);

                return json;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        //GET: Egresos/GetDestinos
        public async Task<IActionResult> GetDestinos(int ScopeId)
        {
            var destinos = await _context.Scopes
                           .Where(a => a.ScopeId != 1 && a.ScopeId != ScopeId && a.EliminadoLogico == false)
                           .ToListAsync();

            return Json(destinos);
        }
    }
}
