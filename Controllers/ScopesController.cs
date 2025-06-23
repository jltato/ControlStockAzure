using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlStock.Models;
using ControlStock.Data;
using Microsoft.AspNetCore.Identity;

namespace ControlStock.Controllers
{
    public class ScopesController : Controller
    {
        private readonly MyDbContext _context;
        private readonly ILogger<ScopesController> _logger;
        private readonly UserManager<MyUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ScopesController(MyDbContext context, ILogger<ScopesController> logger, UserManager<MyUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Scopes
        public async Task<IActionResult> Index()
        {
            var scopes = await _context.Scopes
                .Where(s => s.ScopeId != 1)
                .ToListAsync();
            return View(scopes);
        }

        // GET: Scopes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scopes = await _context.Scopes
                .FirstOrDefaultAsync(m => m.ScopeId == id);
            if (scopes == null)
            {
                return NotFound();
            }

            return View(scopes);
        }

        // GET: Scopes/Create
        public async Task<IActionResult> CreateAsync(string returnUrl = null!)
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

            if (user == null)
            {
                return NotFound();
            }

            ViewBag.UserId = user.Id;
            return View();
        }

        // POST: Scopes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScopeId,ScopeName,UserId")] Scope scopes, String? returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                _context.Add(scopes);
                await _context.SaveChangesAsync();
                return Redirect(returnUrl);
            }
            return View(scopes);
        }

        // GET: Scopes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            
            if (id == null || user == null)
            {
                return NotFound();
            }

            var scopes = await _context.Scopes.FindAsync(id);
            if (scopes == null)
            {
                return NotFound();
            }
            ViewBag.UserId = user.Id;
            return View(scopes);
        }

        // POST: Scopes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScopeId,ScopeName,UserId")] Scope scopes)
        {
            if (id != scopes.ScopeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scopes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScopesExists(scopes.ScopeId))
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
            return View(scopes);
        }

        // GET: Scopes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scopes = await _context.Scopes
                .FirstOrDefaultAsync(m => m.ScopeId == id);
            if (scopes == null)
            {
                return NotFound();
            }

            return View(scopes);
        }

        // POST: Scopes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scopes = await _context.Scopes.FindAsync(id);
            if (scopes != null)
            {
                scopes.EliminadoLogico=true;
                _context.Update(scopes);
                await _context.SaveChangesAsync();                
            }           
            return RedirectToAction(nameof(Index));
        }

        private bool ScopesExists(int id)
        {
            return _context.Scopes.Any(e => e.ScopeId == id);
        }
    }
}
