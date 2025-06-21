// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ControlStock.Data;
using ControlStock.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using ControlStock.Models.DTOs;
using Microsoft.Data.SqlClient;


namespace PruebaIdentity.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Administrador, Coordinador")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<MyUser> _signInManager;
        private readonly UserManager<MyUser> _userManager;
        private readonly IUserStore<MyUser> _userStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly MyDbContext _context;


        public RegisterModel(
            UserManager<MyUser> userManager,
            IUserStore<MyUser> userStore,
            SignInManager<MyUser> signInManager,
            MyDbContext context,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public List<UserList> Usuarios { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]

        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>      


        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
            [Display(Name = "Nombre")]
            public string UserName { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "El nombre y apellido es obligatorio.")]
            [Display(Name = "Nombre y Apellido")]
            public string Nombre { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "La contraseña es obligatoria")]
            [StringLength(100, ErrorMessage = "La {0} debe tener como minimo {2} y maximo {1} caracteres de largo.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "La contraseña y la confirmación no coinciden.")]
            public string ConfirmPassword { get; set; }
        }



        public async Task OnGetAsync(string returnUrl = null)
        {
            var thisUser = await _userManager.GetUserAsync(User);

            var roles = await _userManager.GetRolesAsync(thisUser);

            if (!roles.Contains("Administrador") && !roles.Contains("Coordinador"))
            {
                RedirectToPage("/");
            }

            bool TotalScopes = await _context.UserPermissions
                                   .AnyAsync(up => up.UserId == thisUser.Id && up.ScopeId == 1);
            bool TotalSections = await _context.UserPermissions
                                    .AnyAsync(up=>up.UserId == thisUser.Id && up.SectionId == 1);
            if (TotalScopes && TotalSections)
            {
                Usuarios = await _userManager.Users                 
                           .Where(user => user.Id != thisUser.Id)
                           .Select(user => new UserList
                           {
                               UserId = user.Id,
                               UserName = user.NormalizedUserName,
                               Nombre = user.Nombre,
                               IsLockedOut = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now // Indica si está bloqueado
                           })
                           .ToListAsync();
            }
            else if(TotalScopes)
            {
                    int userSectionId = _context.UserPermissions
                        .Where(up => up.UserId == thisUser.Id)
                        .Select(up => up.SectionId)
                        .FirstOrDefault();

                    Usuarios = await _userManager.Users
                        .Where(anu => anu.Id != thisUser.Id && // Excluye el UserId específico
                                      _context.UserPermissions
                                          .Any(up => up.UserId == anu.Id && up.SectionId == userSectionId)
                                       )
                        .Select(anu => new UserList
                        {
                            UserId = anu.Id,
                            UserName = anu.UserName,
                            Nombre = anu.Nombre,
                            IsLockedOut = anu.LockoutEnd.HasValue && anu.LockoutEnd > DateTimeOffset.Now // Indica si está bloqueado
                        })
                        .ToListAsync();
            }
            else if (TotalSections)
            {
                int userScopeId = _context.UserPermissions
                    .Where(up => up.UserId == thisUser.Id)
                    .Select(up => up.ScopeId)
                    .FirstOrDefault();

                Usuarios = await _userManager.Users
                    .Where(anu => anu.Id != thisUser.Id && // Excluye el UserId específico
                                  _context.UserPermissions
                                      .Any(up => up.UserId == anu.Id && up.ScopeId == userScopeId)
                                   )
                    .Select(anu => new UserList
                    {
                        UserId = anu.Id,
                        UserName = anu.UserName,
                        Nombre = anu.Nombre,
                        IsLockedOut = anu.LockoutEnd.HasValue && anu.LockoutEnd > DateTimeOffset.Now // Indica si está bloqueado
                    })
                    .ToListAsync();
            }
            else
            {
                var sql = @"
                           SELECT DISTINCT 
                                	u.Id,
                                    u.Nombre, 
                                    u.NormalizedUserName, 
                                    u.LockoutEnd 
                            FROM 
                                AspNetUsers u
                                INNER JOIN UserPermissions up ON up.UserId = u.Id
                            WHERE 
                                up.ScopeId IN (SELECT ScopeId FROM UserPermissions WHERE UserId = @UserId)
                                AND up.SectionId IN (SELECT SectionId FROM UserPermissions WHERE UserId = @UserId)
                                AND u.Id != @UserId
                                AND (u.LockoutEnd IS NULL OR u.LockoutEnd > GETDATE())
                            ";

                var userIdParam = new SqlParameter("@UserId", thisUser.Id);

                Usuarios = await _context.Users
                            .FromSqlRaw(sql, userIdParam)
                            .Select(anu => new UserList
                            {                                 
                                UserId = anu.Id,
                                UserName = anu.NormalizedUserName,
                                Nombre = anu.Nombre,
                                IsLockedOut = anu.LockoutEnd.HasValue && anu.LockoutEnd > DateTimeOffset.Now // Indica si está bloqueado
                           
                            })
                            .OrderBy(a => a.Nombre)
                            .ToListAsync();

            }
            ReturnUrl = returnUrl;            
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Nombre = Input.Nombre.ToUpper();

                await _userStore.SetUserNameAsync(user, Input.UserName,  CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    return RedirectToPage("/Account/Details", new { id = userId });

                }
                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError("Input.UserName", "El nombre de usuario ya está en uso.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
           
            return Page();
        }

        private MyUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<MyUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(MyUser)}'. " +
                    $"Ensure that '{nameof(MyUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        public class UserList()
        {
            public string UserId { get; set; }
            public string UserName { get; set; } = string.Empty;               
            public string Nombre {  get; set; } = string.Empty;
            public bool IsLockedOut { get; set; }
        }
    }
}
