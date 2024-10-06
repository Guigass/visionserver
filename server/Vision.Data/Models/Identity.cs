using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Models;
public class Identity
{
    public class AppRole : IdentityRole<Guid>
    {
    }
    public class AppRoleClaim : IdentityRoleClaim<Guid>
    {
    }
    public class AppUserClaim : IdentityUserClaim<Guid>
    {
    }
    public class AppUserLogin : IdentityUserLogin<Guid>
    {
    }
    public class AppUserRole : IdentityUserRole<Guid>
    {
    }
    public class AppUserToken : IdentityUserToken<Guid>
    {
    }
}
