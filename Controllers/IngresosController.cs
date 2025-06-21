using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlStock.Data;
using ControlStock.Models;
using Microsoft.AspNetCore.Identity;
using ControlStock.Models.DTOs;
using Microsoft.Data.SqlClient;

namespace ControlStock.Controllers
{
    public class IngresosController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ILogger<IngresosController> _logger;
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IngresosController(MyDbContext context,  ILogger<IngresosController> logger, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Ingresos
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

        // GET: Ingresos/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
          
            var ingreso = await _context.Ingresos
                .Include(i => i.Proveedor)
                .Include(i => i.DetalleIngresos)
                    .ThenInclude(di => di.Articulo)
                .Include(i => i.DetalleIngresos)
                    .ThenInclude(di => di.Lote)
                .FirstOrDefaultAsync(m => m.IngresoId == id);

            if (ingreso == null)
            {
                return NotFound();
            }

            // Pasar la lista de productos a ViewBag
            ViewBag.Productos = ingreso.DetalleIngresos;
            

            return View(ingreso);
        }


        //POST: Ingresos/DatosTable
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

                var allData = await _context.Ingresos
                                    .Include(a => a.Scope)
                                    .Where(i => i.EliminadoLogico == false &&
                                    i.Scope.ScopeId == ScopeId &&
                                    i.Proveedor.SectionProveedores
                                    .Any(sp => _context.UserPermissions
                                        .Where(up => up.UserId == user.Id)
                                        .Select(up => up.SectionId)
                                        .Contains(sp.SectionId)))
                                     .Select(i => new
                                     {
                                         Ingreso = i,
                                         Proveedor = i.Proveedor
                                     })     
                                     
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
                        i.Ingreso.IngresoId.ToString().Contains(searchValue) ||
                        (i.Proveedor != null && i.Proveedor.ProveedorName.ToLower().Contains(searchValue)) ||
                        (i.Ingreso.Comprobante != null && i.Ingreso.Comprobante.ToLower().Contains(searchValue)) ||
                        i.Ingreso.FechaIngreso.ToString("dd/MM/yyyy").Contains(searchValue) // Ajusta el formato de fecha según necesites
                    ).ToList();
                }

                // Contar registros filtrados
                var totalRecordsFiltered = allData.Count;

                // Aplicar ordenación en memoria
                switch (orderColumnIndex)
                {
                    case 0: 
                        allData = allData.OrderByDescending(e => e.Ingreso.FechaIngreso).ToList();
                        break;
                    case 1: //  ProveedorName
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.Proveedor?.ProveedorName).ToList()
                            : allData.OrderByDescending(i => i.Proveedor?.ProveedorName).ToList();
                        break;
                    case 2: //  FechaIngreso
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.Ingreso.FechaIngreso).ToList()
                            : allData.OrderByDescending(i => i.Ingreso.FechaIngreso).ToList();
                        break;
                    case 3: //  Comprobante
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.Ingreso.Comprobante).ToList()
                            : allData.OrderByDescending(i => i.Ingreso.Comprobante).ToList();
                        break;
                    case 4: //  es IngresoId
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.Ingreso.IngresoId).ToList()
                            : allData.OrderByDescending(i => i.Ingreso.IngresoId).ToList();
                        break;
                    default:
                        allData = allData.OrderByDescending(e => e.Ingreso.FechaIngreso).ToList(); // Orden por defecto
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

        // GET: Ingresos/Create
        public async Task<IActionResult> Create()
        {

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await SetViewBagData(user);             
            }
       
            return View();
        }

        // POST: Ingresos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ingresos ingreso, List<ListadoIngreso> detalleIngresos)
        {
            var user = await _userManager.GetUserAsync(User);            
            try
            {                
                if (ModelState.IsValid && user != null)
                {                    
                    // Añadir el nuevo ingreso al contexto
                    _context.Add(ingreso);
                    // Guardar los cambios en la base de datos para obtener el IngresoId
                    await _context.SaveChangesAsync();

                    foreach (var detalle in detalleIngresos)
                    {
                        // Buscar el artículo en DepositoArticuloLote
                        var depositoArticuloLote = _context.DepositoArticuloLotes
                            .FirstOrDefault(dal => dal.ArticuloId == detalle.ArticuloId
                                                    && dal.ScopeId == ingreso.ScopeId
                                                    && dal.LoteId == detalle.LoteId);

                        if (depositoArticuloLote != null)
                        {
                            // Si el artículo ya existe en el depósito y lote, actualizar la cantidad
                            depositoArticuloLote.Cantidad += detalle.Cantidad;
                        }
                        else
                        {
                            // Si no existe, crear un nuevo registro en DepositoArticuloLote
                            depositoArticuloLote = new DepositoArticuloLote
                            {
                                ScopeId = ingreso.ScopeId,
                                LoteId = detalle.LoteId,
                                ArticuloId = detalle.ArticuloId,
                                Cantidad = detalle.Cantidad
                            };

                            _context.DepositoArticuloLotes.Add(depositoArticuloLote);
                        }

                        // Añadir el detalle al contexto y establecer la relación con el ingreso
                        //detalle.IngresoId = ingreso.IngresoId;
                        //_context.DetalleIngreso.Add(detalle);
                    }

                    // Guardar los cambios en la base de datos
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                // Manejo del caso en que el modelo no es válido
                await SetViewBagData(user);
                ViewBag.DetalleIngreso = detalleIngresos;
                return View(ingreso);
            }
                catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el ingreso con ID: {IngresoId}", ingreso.IngresoId);
                await SetViewBagData(user);
                ViewBag.DetalleIngreso = detalleIngresos;
                return View(ingreso);
            }
        }


        // GET: Ingresos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null || user == null)
            {
                return NotFound();
            }

            var ingresos = await _context.Ingresos.FindAsync(id);
            if (ingresos == null)
            {
                return NotFound();
            }

            var provedores = await _context.Proveedors
              .Include(p => p.SectionProveedores)
              .ThenInclude(sp => sp.Section)
              .Where(p => p.SectionProveedores
                  .Any(sp => sp.Section.UserPermissions
                      .Any(up => up.UserId == user.Id)) && p.EliminadoLogico == false)
              .ToListAsync();

            ViewData["UserId"] = user.Id;    
            ViewData["ProveedorId"] = new SelectList(provedores, "ProveedorId", "ProveedorName", ingresos.ProveedorId);
            return View(ingresos);
        }

        // POST: Ingresos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IngresoId,FechaIngreso,ProveedorId,Observaciones,UserId,ScopeId,Comprobante")] Ingresos ingresos)
        {
            if (id != ingresos.IngresoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingresos);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngresosExists(ingresos.IngresoId))
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
            ViewData["ProveedorId"] = new SelectList(_context.Proveedors, "ProveedorId", "ProveedorName", ingresos.ProveedorId);
            return View(ingresos);
        }

        // GET: Ingresos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ingresos = await _context.Ingresos
                .Include(i => i.Proveedor)
                .FirstOrDefaultAsync(m => m.IngresoId == id);
            if (ingresos == null)
            {
                return NotFound();
            }

            return View(ingresos);
        }

        // POST: Ingresos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ingresos = await _context.Ingresos.FindAsync(id);
            if (ingresos != null)
            {
                ingresos.EliminadoLogico = true;
                _context.Update(ingresos);                
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngresosExists(int id)
        {
            return _context.Ingresos.Any(e => e.IngresoId == id);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLote(Lote lote)
        {
            if (ModelState.IsValid)
            {
                _context.Lotes.Add(lote);
                await _context.SaveChangesAsync();

                // Redireccionar o retornar el resultado adecuado
                return Json(new { success = true, loteId = lote.LoteId, NumeroLote = lote.NumeroLote });
            }

            // Manejo de errores
            return Json(new { success = false });
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

            var provedores = await _context.Proveedors
                .Include(p => p.SectionProveedores)
                .ThenInclude(sp => sp.Section)
                .Where(p => p.SectionProveedores
                    .Any(sp => sp.Section.UserPermissions
                        .Any(up => up.UserId == user.Id)) && p.EliminadoLogico == false)
                .ToListAsync();

            ViewBag.Proveedores = new SelectList(provedores, "ProveedorId", "ProveedorName");

            var depositos = await _context.Scopes
                .Where(s => _context.UserPermissions
                    .Any(up => up.UserId == user.Id && up.ScopeId == s.ScopeId && up.Scope.EliminadoLogico == false))
                .ToListAsync();

            ViewBag.Depositos = new SelectList(depositos, "ScopeId", "ScopeName");
        }

    }
}
