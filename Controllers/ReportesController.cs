using ControlStock.Data;
using ControlStock.Models;
using ControlStock.Models.DTOs;
using jsreport.AspNetCore;
using jsreport.Types;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static System.Formats.Asn1.AsnWriter;

namespace ControlStock.Controllers
{
    public class ReportesController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<MyUser> _userManager;
        private readonly ILogger<ReportesController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ReportesController(ILogger<ReportesController> logger, MyDbContext context, UserManager<MyUser> UserManager, RoleManager<IdentityRole> RoleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = UserManager;
            _roleManager = RoleManager;
        }
        //GET /reports/PrintReport
        public async Task<ActionResult> PrintReportAsync(int Id)
        {
            var user = await _userManager.GetUserAsync(User);
            var egreso = await _context.Egresos
                     .Include(a => a.Scope)
                     .Include(b => b.DestinoScope)
                     .Include(e => e.DetalleEgresos)
                     .ThenInclude(de => de.Articulo)
                     .FirstOrDefaultAsync(e => e.EgresoId == Id);

            if (egreso == null)
            {
                return NotFound();
            }
            var seccion = _context.Sections
                .Include(a => a.UserPermissions)
                 .Where(s => s.UserPermissions.Any(up => up.UserId == user.Id))
                .FirstOrDefault();
            // También puedes pasar datos adicionales usando ViewBag
            ViewBag.seccion = seccion.Name;

            // Pasar el modelo a la vista

            return View("Index", egreso);
        }




        // GET: ReportsController
        [MiddlewareFilter(typeof(JsReportPipeline))]
        public async Task<ActionResult> IndexAsync(int Id)
        {
            var user = await _userManager.GetUserAsync(User);
            var egreso = await _context.Egresos
                    .Include(a => a.Scope)
                    .Include(b => b.DestinoScope)
                    .Include(e => e.DetalleEgresos)
                    .ThenInclude(de => de.Articulo)
                    .FirstOrDefaultAsync(e => e.EgresoId == Id);

            if (egreso == null)
            {
                return NotFound();
            }

            var seccion = _context.Sections
                .Include(a => a.UserPermissions)
                 .Where(s => s.UserPermissions.Any(up => up.UserId == user.Id))
                .FirstOrDefault();

            // También puedes pasar datos adicionales usando ViewBag
            ViewBag.seccion = seccion.Name;

            //// Pasar el modelo a la vista
            HttpContext.JsReportFeature()
                .DebugLogsToResponse()
                .Recipe(Recipe.ChromePdf);
            return View(egreso);

        }

        // GET: ReportsController/Stock
        public async Task<ActionResult> Stock(int deposito)
        {
            try
            { 
                var user = await _userManager.GetUserAsync(User);
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
                            "
                ;
                var scope = await _context.Scopes
                                .Where(a => a.ScopeId == deposito)
                                .FirstOrDefaultAsync();
                var userIdParam = new SqlParameter("@UserId", user.Id);
                var scopeIdParam = new SqlParameter("@ScopeId", deposito);

                var allData = await _context.Database.SqlQueryRaw<ListadoStock>(sql, userIdParam, scopeIdParam)
                    .ToListAsync();

                ViewBag.Deposito = scope?.ScopeName;
                return View(allData);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }
    }
}
