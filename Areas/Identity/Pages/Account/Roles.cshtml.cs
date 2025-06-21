using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PruebaIdentity.Areas.Identity.Pages.Account;
using ControlStock.Data;
using System.ComponentModel.DataAnnotations;

namespace ControlStock.Areas.Identity.Pages.Account
{

    public class RolesModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _rolManager;
        private readonly MyDbContext _context;
        private readonly ILogger<RegisterModel> _logger;


        public RolesModel(RoleManager<IdentityRole> rolManager, MyDbContext context, ILogger<RegisterModel> logger)
        {
            _rolManager = rolManager;
            _context = context;
            _logger = logger;
        }


        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public List<Sections> Sections { get; set; }
        public List<Scopes> Scopes { get; set; }

        public List<RolePermissionsViewModel> RolePermissions { get; set; }

        public async Task<ActionResult> OnGet()
        {
               RolePermissions = await _context.UserPermissions
              .Include(rp => rp.User)
              .Include(rp => rp.Scope)
              .Include(rp => rp.Section)
              .Select(rp => new RolePermissionsViewModel
              {
                  RoleName = rp.User.UserName,
                  ScopeName = rp.Scope.ScopeName,
                  SectionName = rp.Section.Name
              })
              .ToListAsync();


            Sections = await _context.Sections.ToListAsync();
            Scopes = await _context.Scopes.ToListAsync();

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Identity/Account/Roles");

            if (ModelState.IsValid)
            {
                var newRol = new IdentityRole();
                newRol.Name = Input.RolName;
                newRol.NormalizedName = Input.RolName.ToUpper();

                var res = _rolManager.CreateAsync(newRol);
                if (res.Result.Succeeded)
                {
                    var rolePermission = new UserPermissions
                    {
                        UserId = newRol.Id, // Utiliza el Id del nuevo rol
                        SectionId = Input.SelectedSection,
                        ScopeId = Input.SelectedScope 
                    };

                    _context.RolePermissions.Add(rolePermission);

                    try
                    {
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("User created a new rol.");
                        return LocalRedirect(returnUrl);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        ModelState.AddModelError(string.Empty, "Ocurrió un error inesperado. Intente nuevamente más tarde.");
                    }
                    
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in res.Result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            // If we got this far, something failed, redisplay form
            Sections = await _context.Sections.ToListAsync();
            Scopes = await _context.Scopes.ToListAsync();
            RolePermissions = await _context.RolePermissions
             .Include(rp => rp.Role)
             .Include(rp => rp.Scope)
             .Include(rp => rp.Section)
             .Select(rp => new RolePermissionsViewModel
             {
                 RoleName = rp.Role.NormalizedName,
                 ScopeName = rp.Scope.ScopeName,
                 SectionName = rp.Section.Name
             })
             .ToListAsync();
            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "El nombre de rol es obligatorio.")]
            [Display(Name = "Nombre")]
            public string RolName { get; set; } = string.Empty;

            [Required(ErrorMessage = "La sección es obligatoria.")]
            [Display(Name = "Sección")]
            public int? SelectedSection { get; set; }

            [Required(ErrorMessage = "La sección es obligatoria.")]
            [Display(Name = "Sección")]
            public int? SelectedScope { get; set; } 


        }
    

        public class RolePermissionsViewModel
        {
            [Required]
            public string RoleName { get; set; } 
            public string? ScopeName { get; set; }
            public string? SectionName { get; set; }
        }

    }
}

