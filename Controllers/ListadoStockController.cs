using ControlStock.Data;
using ControlStock.Models;
using ControlStock.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ControlStock.Controllers
{
    public class ListadoStockController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<MyUser> _userManager;
        private readonly ILogger<ListadoStockController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
       

        public ListadoStockController(MyDbContext context, UserManager<MyUser> userManager, ILogger<ListadoStockController> logger, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
          
        }

        // GET: ListadoStockController
        public async Task<ActionResult> Index()
        {

            var user = await _userManager.GetUserAsync(User);
            var depositos = await _context.Scopes
                        .Where(s => _context.UserPermissions
                            .Any(up => up.UserId == user.Id && up.ScopeId == s.ScopeId && up.Scope.EliminadoLogico == false))
                        .ToListAsync();
            ViewBag.depositos = new SelectList(depositos, "ScopeId", "ScopeName");

            return View();
        }

        //POST: ListadoStock/DatosTable
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

                var sql = @"
                           SELECT 
                                a.IdArticulo AS ArticuloId, 
                                a.Nombre AS Articulo, 
                                m.MarcaName AS Marca, 
                                r.Name AS Rubro, 
                                SUM(dal.Cantidad) as Cantidad
                            FROM 
                                Articulo a
                            INNER JOIN 
                                Marca m ON m.IdMarca = a.IdMarca
                            INNER JOIN 
                                Rubro r ON r.IdRubro = m.IdRubro
                            INNER JOIN 
                                DepositoArticuloLotes dal ON dal.ArticuloId = a.IdArticulo
                            WHERE 
                                dal.ScopeId in (SELECT ScopeId FROM UserPermissions WHERE UserId = @UserId)		
                                AND
                                r.SectionId in (SELECT SectionId FROM UserPermissions WHERE UserId = @UserId)		
                                AND a.EliminadoLogico = 'false' 
                                AND (dal.Cantidad > 0 
                                     OR (dal.Cantidad <= 0 AND a.Activo = 'true'))
	                            AND DAL.ScopeId = @ScopeId
                            GROUP BY a.IdArticulo, a.Nombre, m.MarcaName, r.Name
                            ORDER BY r.Name, a.Nombre
                            ";

                var userIdParam = new SqlParameter("@UserId", user.Id);
                var scopeIdParam = new SqlParameter("@ScopeId", ScopeId);

                var allData = await _context.Database.SqlQueryRaw<ListadoStock>(sql, userIdParam, scopeIdParam)
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
                        i.ArticuloId.ToString().Contains(searchValue) ||
                        (i.Articulo != null && i.Articulo.ToLower().Contains(searchValue)) ||
                        i.Marca != null && i.Marca.ToLower().Contains(searchValue) ||
                        i.Rubro != null && i.Rubro.ToLower().Contains(searchValue) 
                    ).ToList();
                }

                // Contar registros filtrados
                var totalRecordsFiltered = allData.Count;

                // Aplicar ordenación en memoria
                switch (orderColumnIndex)
                {
                    case 0: // Suponiendo que la primera columna es IngresoId
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.ArticuloId).ToList()
                            : allData.OrderByDescending(i => i.ArticuloId).ToList();
                        break;
                    case 1: // Suponiendo que la segunda columna es ProveedorName
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.Articulo).ToList()
                            : allData.OrderByDescending(i => i.Articulo).ToList();
                        break;
                    case 2: // Suponiendo que la tercera columna es FechaIngreso
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.Rubro).ToList()
                            : allData.OrderByDescending(i => i.Rubro).ToList();
                        break;
                    case 3: // Suponiendo que la tercera columna es FechaIngreso
                        allData = orderDirection == "OrderBy"
                            ? allData.OrderBy(i => i.Marca).ToList()
                            : allData.OrderByDescending(i => i.Marca).ToList();
                        break;
                    default:
                        allData = allData.OrderByDescending(e => e.Articulo).ToList(); // Orden por defecto
                        break;
                }

                if (length == -1)
                {
                    length = totalRecordsFiltered;
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





        // GET: ListadoStockController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var detalle = await _context.Articulos
                .Include(a => a.Marca)
                .ThenInclude(b => b.Rubro)
                .Where(c => c.IdArticulo == id)
                .FirstOrDefaultAsync();

            var lotes = await _context.DepositoArticuloLotes
                        .Include(dal => dal.Lote) 
                        .Include(dal => dal.Scope) 
                        .Where(dal => dal.ArticuloId == id && dal.Cantidad > 0 &&
                                      _context.UserPermissions.Any(up => up.ScopeId == dal.ScopeId && up.UserId == user.Id))
                        .Select(dal => new {
                            dal.DepositoArticuloLoteId,
                            dal.Lote,
                            dal.Cantidad
                        })
                        .ToListAsync();

            ViewBag.lotes = lotes;
            return View(detalle);
        }

        // GET: ListadoStockController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ListadoStockController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ListadoStockController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var lote = await _context.DepositoArticuloLotes
                    .Include(a => a.Lote)
                    .Where(a => a.DepositoArticuloLoteId == id)
                    .FirstOrDefaultAsync();
            return View(lote);
        }

        // POST: ListadoStockController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, DepositoArticuloLote depositoArticuloLote)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(depositoArticuloLote);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Details), new { id = depositoArticuloLote.ArticuloId });
                }
                else
                {
                    var lote = await _context.DepositoArticuloLotes
                    .Include(a => a.Lote)
                    .Where(a => a.DepositoArticuloLoteId == id)
                    .FirstOrDefaultAsync();
                    return View(lote);
                }   
                
            }
            catch
            {
                var lote = await _context.DepositoArticuloLotes
                    .Include(a => a.Lote)
                    .Where(a => a.DepositoArticuloLoteId == id)
                    .FirstOrDefaultAsync();
                return View(lote);
            }
        }

        // GET: ListadoStockController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ListadoStockController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
