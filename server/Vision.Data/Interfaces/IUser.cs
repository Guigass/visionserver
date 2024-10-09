using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Interfaces;
public interface IUser
{
    string Name { get; }
    Guid? GetUserId();
    string GetUserEmail();
    string GetUserNome();
    bool IsAuthenticated();
    bool IsInRole(string role);
    bool HasClaim(string claimType, string claimValue)
    IEnumerable<Claim> GetClaimsIdentity();
}
