using ControlStock.Data;
using ControlStock.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ControlStock.Controllers
{
    public class ProveedoresController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<MyUser> _userManager;
        private readonly ILogger<ProveedoresController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
        
        public ProveedoresController(MyDbContext context, UserManager<MyUser> user, ILogger<ProveedoresController> logger, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = user;
            _logger = logger;
            _roleManager = roleManager;
        }

        // GET: Proveedores
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                var provedores = await _context.Proveedors
                                 .Include(p => p.SectionProveedores)
                                 .ThenInclude(sp => sp.Section)
                                 .Where(p => p.SectionProveedores
                                     .Any(sp => sp.Section.UserPermissions
                                         .Any(up => up.UserId == user.Id)) && p.EliminadoLogico == false)
                                 .ToListAsync();
                ViewBag.UserId = user.Id;
                return View(provedores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                
                return View();
            }
           
        }

        // GET: Proveedores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .FirstOrDefaultAsync(m => m.ProveedorId == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // GET: Proveedores/Create
        public async Task<IActionResult> Create(string returnUrl = null!)
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
            
            if (user != null)
            {
                ViewBag.UserId = user.Id;
                var sections = await _context.Sections
                    .Where(sections => _context.UserPermissions
                        .Where(up => up.UserId == user.Id)
                        .Select(up => up.SectionId)
                        .Contains(sections.SectionId))
                    .ToListAsync();
                ViewBag.Sections = new SelectList(sections, "SectionId", "Name");
            }
            
                return View();
        }

        // POST: Proveedores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProveedorId,ProveedorName,ProveedorAdress,ProveedorPhone,UserId,SectionId,ProveedorMail")] Proveedor proveedor, int SectionId, string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            // Validar si el SectionId está presente
            if (SectionId == 0)
            {
                ModelState.AddModelError("SectionId", "Debes seleccionar una sección.");
                return View(proveedor);
            }            
           
            if (ModelState.IsValid)
            {
                proveedor.ProveedorName = proveedor.ProveedorName.ToUpper();
                proveedor.ProveedorAdress = proveedor.ProveedorAdress?.ToUpper();
                _context.Add(proveedor);
                await _context.SaveChangesAsync();

                // Crear una instancia de SectionProveedor
                var sectionProveedor = new SectionProveedor
                {
                    ProveedorId = proveedor.ProveedorId,
                    SectionId = SectionId
                };

                // Agregar la relación a la tabla intermedia
                _context.Add(sectionProveedor);
                await _context.SaveChangesAsync();

                return Redirect(returnUrl);
            }
            return View(proveedor);
        }

        // GET: Proveedores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.UserId = user.Id;
                var sections = await _context.Sections
                    .Where(sections => _context.UserPermissions
                        .Where(up => up.UserId == user.Id)
                        .Select(up => up.SectionId)
                        .Contains(sections.SectionId))
                    .ToListAsync();
                ViewBag.Sections = new SelectList(sections, "SectionId", "Name");
            }
            else
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: Proveedores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProveedorId,ProveedorName,ProveedorAdress,ProveedorPhone,UserId,SectionId,ProveedorMail")] Proveedor proveedor, int SectionId)
        {

            if (id != proveedor.ProveedorId)
            {
                return NotFound();
            }

            // Validar si el SectionId está presente
            if (SectionId == 0)
            {
                ModelState.AddModelError("SectionId", "Debes seleccionar una sección.");
                return View(proveedor);
            }

            //Validar el Modelo Proveedor
            if (ModelState.IsValid)
            {
                try
                {
                    proveedor.ProveedorName = proveedor.ProveedorName.ToUpper();
                    proveedor.ProveedorAdress = proveedor.ProveedorAdress?.ToUpper();
                    _context.Update(proveedor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProveedorExists(proveedor.ProveedorId))
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
            return View(proveedor);
        }

        // GET: Proveedores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedors
                .FirstOrDefaultAsync(m => m.ProveedorId == id);
            if (proveedor == null)
            {
                return NotFound();
            }

            return View(proveedor);
        }

        // POST: Proveedores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proveedor = await _context.Proveedors.FindAsync(id);
            if (proveedor != null)
            {
                proveedor.EliminadoLogico = true;
                _context.Update(proveedor);
                await _context.SaveChangesAsync();
            }

            
            return RedirectToAction(nameof(Index));
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedors.Any(e => e.ProveedorId == id);
        }
    }
}
