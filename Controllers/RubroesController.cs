using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlStock.Data;
using ControlStock.Models;
using Microsoft.AspNetCore.Identity;

namespace ControlStock.Controllers
{
    public class RubroesController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<MyUser> _userManager;
        private readonly ILogger<ArticuloController> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RubroesController(MyDbContext context, UserManager<MyUser> userManager, ILogger<ArticuloController> logger, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
            _roleManager = roleManager;
        }

        // GET: Rubroes
        public async Task<IActionResult> Index()
        {
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
                return View(rubros);
            }
            return View();
        }

        // GET: Rubroes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubro = await _context.Rubros
                .FirstOrDefaultAsync(m => m.IdRubro == id);
            if (rubro == null)
            {
                return NotFound();
            }

            return View(rubro);
        }


        // GET: Rubroes/Create
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
                var sections = await _context.Sections
                    .Where(sections => _context.UserPermissions
                        .Where(up => up.UserId == user.Id)
                        .Select(up => up.SectionId)
                        .Contains(sections.SectionId))
                    .ToListAsync();
                ViewBag.Sections = new SelectList(sections, "SectionId", "Name");
                return View(); 
            }
            return View();
        }

        // POST: Rubroes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdRubro,Name,SectionId")] Rubro rubro, string? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var user = await _userManager.GetUserAsync(User);
            rubro.UserId = user.Id;
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                rubro.Name = rubro.Name.ToUpper();
                _context.Add(rubro);
                await _context.SaveChangesAsync();
                return Redirect(returnUrl);
            }
            return View(rubro);
        }

        // GET: Rubroes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubro = await _context.Rubros.FindAsync(id);
            if (rubro == null)
            {
                return NotFound();
            }
            return View(rubro);
        }

        // POST: Rubroes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdRubro,Name, SectionId")] Rubro rubro)
        {
            ModelState.Remove("UserId");
            if (id != rubro.IdRubro)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            rubro.UserId = user.Id;

            if (ModelState.IsValid)
            {
                try
                {
                    rubro.Name = rubro.Name.ToUpper();
                    _context.Update(rubro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RubroExists(rubro.IdRubro))
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
            return View(rubro);
        }

        // GET: Rubroes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rubro = await _context.Rubros
                .FirstOrDefaultAsync(m => m.IdRubro == id);
            if (rubro == null)
            {
                return NotFound();
            }

            return View(rubro);
        }

        // POST: Rubroes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var user = await _userManager.GetUserAsync(User);
            var rubro = await _context.Rubros.FindAsync(id);
            if (rubro != null)
            {
                rubro.UserId = user.Id;
                rubro.EliminadoLogico = true;
                _context.Update(rubro);
                await _context.SaveChangesAsync();               
            }           
            return RedirectToAction(nameof(Index));
        }

        private bool RubroExists(int id)
        {
            return _context.Rubros.Any(e => e.IdRubro == id);
        }
    }
}
