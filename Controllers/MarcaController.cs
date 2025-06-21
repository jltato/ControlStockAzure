using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlStock.Data;
using ControlStock.Models;
using Microsoft.AspNetCore.Identity;
using System.Linq.Dynamic.Core;

namespace ControlStock.Controllers
{
    public class MarcaController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<MyUser> _userManager;
        private readonly ILogger<MarcaController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public MarcaController(MyDbContext context, UserManager<MyUser> user, ILogger<MarcaController> logger, RoleManager<IdentityRole> roleManager )
        {
            _context = context;
            _userManager = user;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: Marca
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var marcas = await _context.Marcas
                         .Include(m => m.Rubro)
                         .Where(m => _context.UserPermissions
                             .Where(up => up.UserId == user.Id)
                             .Select(up => up.SectionId)
                             .Contains(m.Rubro.SectionId) && m.EliminadoLogico == false)
                             .ToListAsync();

            return View(marcas);
        }

        // GET: Marca/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas
                .Include(m => m.Rubro)
                .FirstOrDefaultAsync(m => m.IdMarca == id);
            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }

        // GET: Marca/Create
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
                var rubros = await _context.Rubros
                        .Where(rubro => _context.UserPermissions
                            .Where(up => up.UserId == user.Id)
                            .Select(up => up.SectionId)
                            .Contains(rubro.SectionId) &&
                                rubro.EliminadoLogico == false)
                        .ToListAsync();

                ViewData["UserId"] = user.Id;
            ViewData["IdRubro"] = new SelectList(rubros, "IdRubro", "Name");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Marca/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMarca,MarcaName,IdRubro,UserId")] Marca marca, string returnUrl = null!)
        {
            returnUrl = returnUrl ?? Url.Content("~/marca/index");
            try
            {
                if (ModelState.IsValid)
                {                    
                    marca.MarcaName = marca.MarcaName.ToUpper();
                    _context.Add(marca);
                    await _context.SaveChangesAsync();
                    return Redirect(returnUrl);
                }
                //ViewData["IdRubro"] = new SelectList(_context.Rubros, "IdRubro", "Name", marca.IdRubro);
                return View(marca);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return View();
            }           
        }

        // GET: Marca/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
            {
                return NotFound();
            }

            var rubros = await _context.Rubros
                .Include(a => a.Section)
                .ThenInclude(a => a.UserPermissions
                    .Where(up => up.UserId == user.Id))
                .Where(up => up.EliminadoLogico == false)
                .ToListAsync();

            ViewData["UserId"] = user.Id;
            ViewData["IdRubro"] = new SelectList(rubros, "IdRubro", "Name", marca.IdRubro);
            return View(marca);
        }

        // POST: Marca/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMarca,MarcaName,IdRubro,UserId")] Marca marca)
        {
            if (id != marca.IdMarca)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    marca.MarcaName = marca.MarcaName.ToUpper();
                    _context.Update(marca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcaExists(marca.IdMarca))
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
            ViewData["IdRubro"] = new SelectList(_context.Rubros, "IdRubro", "Name", marca.IdRubro);
            return View(marca);
        }

        // GET: Marca/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas   
                .Include(m => m.Rubro)
                .FirstOrDefaultAsync(m => m.IdMarca == id);
            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }

        // POST: Marca/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var marca = await _context.Marcas.FindAsync(id);
            if (marca != null)
            {
                marca.UserId = user.Id;
                marca.EliminadoLogico = true;
                //_context.Marcas.Remove(marca);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarcaExists(int id)
        {
            return _context.Marcas.Any(e => e.IdMarca == id);
        }
    }
}
