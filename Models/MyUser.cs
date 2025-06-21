using Microsoft.AspNetCore.Identity;

namespace ControlStock.Data
{
    public class MyUser : IdentityUser
    {
        public bool PaswordChange { get; set; } = true;
        public string Nombre { get; set; } = string.Empty;
    }
}
