// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ControlStock.Models;
using System.ComponentModel.DataAnnotations;
using ControlStock.Data;
using Microsoft.VisualBasic;

namespace SUAP_PortalOficios.Areas.Identity.Pages.Account
{
    public class DetailsModel : PageModel
    {

        private readonly UserManager<MyUser> _userManager;
        private readonly MyDbContext _context;
        private readonly ILogger<DetailsModel> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;    

        public DetailsModel (
             UserManager<MyUser> userManager
            , MyDbContext context
            , ILogger<DetailsModel> logger,
             RoleManager<IdentityRole> roleManager
             )
        {

            _userManager = userManager;
            _context = context;
            _logger = logger;
            _roleManager = roleManager;

        }

        [BindProperty]
        public InputModel Input { get; set; }
       
        [BindProperty]
        public RolModel Rol { get; set; }

        [BindProperty(SupportsGet = true)]
        public string UserId { get; set; }

        public string ReturnUrl { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<Section> Sections { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<Scope> Scopes { get; set; }

        [BindProperty(SupportsGet = true)]
        public List<IdentityRole> Roles { get; set; }

        [BindProperty (SupportsGet = true)]
        public string UserRole { get; set; }

        public string UserName { get; set; }
        public bool IsLocked { get; set; }
        public List<roleList> Alcance { get; set; }

       
        public string StatusMessage { get; set; }

        public async Task<ActionResult> OnGetAsync(string id)
        {
            UserId = id;

            await inicializador(UserId);
         
            return Page();
        }

        public async Task<ActionResult> OnPostForm1(string id)
        {
            UserId = id;
            var user = await _userManager.FindByIdAsync(id);
                   
            if (user != null)
            { 
                ModelState.Clear();
                TryValidateModel(Input, nameof(Input));
                if (ModelState.IsValid)
                {
                    var userPermission = new UserPermission
                    {
                        UserId = user.Id,
                        SectionId = Input.SelectedSection,
                        ScopeId = Input.SelectedScope
                    };
                    _context.UserPermissions.Add(userPermission);
               
                    try
                    {
                        var result = await _context.SaveChangesAsync();
                        if (result > 0)
                        {
                            _logger.LogInformation("User modify the {0} account with succsess.", user.NormalizedUserName);
                            return RedirectToPage("/Account/Details", new { id = user.Id });
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Ha ocurrido un error, intente nuevamente");
                            return Page();
                        }
                    }
                    catch (Exception ex) {
                        if (ex.InnerException.HResult == -2146232060)
                        {
                            ModelState.AddModelError(string.Empty, "Este Usuario ya posee este alcance. Por favor elija otro");
                            await inicializador(UserId);
                            return Page();
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, ex.Message);
                            await inicializador(UserId);
                            return Page();
                        }                        
                    }
                }
            }
            await inicializador(UserId);
            return Page();
        }

        public async Task<ActionResult> OnPostForm3(string id)
            {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                var rolename = await _userManager.GetRolesAsync(user);

                var nombreRol = await _roleManager.FindByIdAsync(Rol.SelectedRolId);
                if (rolename.Count == 0)
                {

                    var res = await _userManager.AddToRoleAsync(user, nombreRol.Name);
                    if (res.Succeeded)
                    {
                        _logger.LogInformation("User modify the {0} account with succsess.", user.NormalizedUserName);
                        return RedirectToPage("/Account/Details", new { id = user.Id });
                    }
                }
                else
                {
                    var rol = await _roleManager.FindByNameAsync(rolename.FirstOrDefault());
                    if (rol.Id != Rol.SelectedRolId)
                    {
                        var resultRemove = await _userManager.RemoveFromRolesAsync(user, rolename);
                        if (resultRemove.Succeeded)
                        {
                            var resultAdd = await _userManager.AddToRoleAsync(user, nombreRol.Name);

                            if (resultAdd.Succeeded)
                            {
                                _logger.LogInformation("User modify the {0} account with succsess.", user.NormalizedUserName);
                                return RedirectToPage("/Account/Details", new { id = user.Id });
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation("User modify the {0} account with succsess.", user.NormalizedUserName);
                        return RedirectToPage("/Account/Details", new { id = user.Id });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            var userRole = await _context.UserRoles
                               .Where(ur => ur.UserId == UserId)
                               .Select(ur => ur.RoleId)
                               .FirstOrDefaultAsync();
           if (userRole != null)
           {
                UserRole = userRole;
           }
           return Page();         
        }


        public async Task<ActionResult> OnPostDeleteRole(string userId, int scopeId, int sectionId)
        {
            try
            {
                var itemToDelete = await _context.UserPermissions.FirstOrDefaultAsync(u => u.UserId == userId && u.ScopeId == scopeId && u.SectionId == sectionId);
                if (itemToDelete != null)
                {
                    _context.UserPermissions.Remove(itemToDelete);
                    await _context.SaveChangesAsync();
                    return RedirectToPage("/Account/Details", new { id = userId });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await inicializador(userId);
                return Page();
            }
            return RedirectToPage("/Account/Details", new { id = userId });
        }
       

        public async Task<IActionResult> OnPostDeleteUserAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId is required.");
            }
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return NotFound("User not found.");
                }

                //var res = await _userManager.DeleteAsync(user); // Eliminar el Usuario definitivamente (ojo!! Eliminado en casada de todas las tablas)

                if (user.LockoutEnd > DateTime.Now)
                {
                    user.LockoutEnd = null;
                }
                else { 
                    user.LockoutEnd = DateTimeOffset.MaxValue; // Bloquear indefinidamente (no genera problemas de eliminado en cascada)
                }
                var res = await _userManager.UpdateAsync(user);

                if (res.Succeeded)
                {
                    return RedirectToPage("/Account/Register");
                    
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                await inicializador(userId);
                return Page();
            }
            await inicializador(userId);
            return Page();
        }


        public async Task inicializador(string id )
        {
           
            var thisUser = await _userManager.GetUserAsync(User);
            var thisUserId = thisUser.Id;
           
            UserId = id;
         
            var sections = await _context.Sections
                .Where(section => _context.UserPermissions
                    .Any(up => up.SectionId == section.SectionId && up.UserId == thisUserId))
                .ToListAsync();         
          
            bool hasSpecificScope = await _context.UserPermissions
                                    .AnyAsync(up => up.UserId == thisUserId && up.ScopeId == 1);
            if (hasSpecificScope)
            {
                Scopes = await _context.Scopes.ToListAsync();
            }
            else
            {
                Scopes = await _context.UserPermissions
                            .Where(up => up.UserId == thisUserId)
                            .Join(_context.Scopes,
                                   up => up.ScopeId,
                                   sec => sec.ScopeId,
                                   (up, sec) => sec)
                            .Distinct() 
                            .ToListAsync();
            }

            if (User.IsInRole("Administrador"))
            {
                Roles = await _context.Roles.ToListAsync();
                Sections = await _context.Sections.ToListAsync();
            }
            else
            {
                Sections = sections;
                Roles = await _context.Roles
                        .Where(r => r.Name != "Administrador")
                        .ToListAsync();
            }

            var userRole = await _context.UserRoles
                          .Where(ur => ur.UserId == UserId)
                          .Select(ur => ur.RoleId)
                          .FirstOrDefaultAsync();
            if (userRole != null)
            {
                UserRole = userRole;
            }
            Alcance = _context.UserPermissions
                        .Where(up => up.UserId == UserId)
                        .Join(_context.Sections,
                              up => up.SectionId,
                              sec => sec.SectionId,
                              (up, sec) => new { up, sec })
                        .Join(_context.Scopes,
                              upSecJoint => upSecJoint.up.ScopeId,
                              sco => sco.ScopeId,
                              (upSecJoint, sco) => new roleList
                              {
                                  ScopeName = sco.ScopeName,
                                  SectionName = upSecJoint.sec.Name,
                                  UserId = UserId,
                                  ScopeId = sco.ScopeId,
                                  SectionId = upSecJoint.sec.SectionId
                              })
                        .ToList();

            var res = _context.Users
                   .Where(us => UserId == null || us.Id == UserId)
                   .FirstOrDefault();
            UserName = res.Nombre;
            IsLocked = res.LockoutEnd > DateTime.Now;
        }

        public class InputModel
        {           
            [Required(ErrorMessage = "La secci�n es obligatoria.")]
            [Display(Name = "Secci�n")]
            public int SelectedSection { get; set; }

            [Required(ErrorMessage = "El �mbito es obligatorio.")]
            [Display(Name = "�mbito")]
            public int SelectedScope { get; set; }        
        }

        public class RolModel
        {
            [Required(ErrorMessage = "El Rol es obligatorio.")]
            [Display(Name = "Rol")]
            public string SelectedRolId { get; set; }
        } 
    }

    public class roleList
    {
        public string UserId { get; set; }
        public int? SectionId { get; set; }
        public int? ScopeId { get; set; }
        public string ScopeName { get; set; }
        public string SectionName {  get; set; }  
    }
}
