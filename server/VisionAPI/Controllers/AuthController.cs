using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using VisionAPI.Configuration;
using VisionAPI.Notifications;
using VisionAPI.ViewModels;

namespace VisionAPI.Controllers;

public class AuthController : MainController
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly AppSettings _appSettings;
    private readonly ILogger _logger;

    public AuthController(INotificador notificador,
                          UserManager<User> userManager,
                          SignInManager<User> signInManager,
                          IOptions<AppSettings> appSettings,
                          ILogger<AuthController> logger,
                          IUser user) : base(notificador, user)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    [HttpPost("Register")]
    public async Task<ActionResult> Registrar(RegisterUserViewModel registerUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var user = new User
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = false,
            FirstName = registerUser.Name,
            LastName = registerUser.Lastname,
            IsActive = true,
            CreatedAt = DateTime.Now,
            LastLogin = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation($"Usuário {registerUser.Email} criado com sucesso");

            await _signInManager.SignInAsync(user, false);
            return CustomResponse(await GerarJwt(user.Email));
        }

        foreach (var erro in result.Errors)
        {
            NotificarErro(erro.Description);
        }

        return CustomResponse(registerUser);
    }

    [HttpPost("Login")]
    public async Task<ActionResult> Login(LoginUserViewModel loginUser)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

        if (result.Succeeded)
        {
            _logger.LogInformation($"Usuário {loginUser.Email} logado com sucesso");
            return CustomResponse(await GerarJwt(loginUser.Email));
        }

        if (result.IsLockedOut)
        {
            _logger.LogInformation($"Usuário {loginUser.Email} temporariamente bloqueado por tentativas inválidas");

            NotificarErro("Usuário temporariamente bloqueado por tentativas inválidas");
            return CustomResponse(loginUser);
        }

        NotificarErro("Usuário ou senha incorretos");
        return CustomResponse(loginUser);
    }

    private async Task<LoginResponseViewModel> GerarJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        var claims = await _userManager.GetClaimsAsync(user!);
        var userRoles = await _userManager.GetRolesAsync(user!);

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user!.Id.ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email!));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, Tools.Date.ToUnixEpochDate(DateTime.Now).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, Tools.Date.ToUnixEpochDate(DateTime.Now).ToString(), ClaimValueTypes.Integer64));

        foreach (var userRole in userRoles)
            claims.Add(new Claim("role", userRole));

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret!);

        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _appSettings.Issuer,
            Audience = _appSettings.ValidAt,
            Subject = identityClaims,
            Expires = DateTime.Now.AddHours(_appSettings.ExpirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        });

        var encodedToken = tokenHandler.WriteToken(token);

        var claimsExclude = new[] { "sub", "jti", "nbf", "iat" };

        var response = new LoginResponseViewModel
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_appSettings.ExpirationHours).TotalSeconds,
            User = new UserTokenViewModel
            {
                Id = user.Id,
                Email = user.Email!,
                Name = user.FirstName!,
                Lastname = user.LastName!,
                Claims = claims.Where(c => !claimsExclude.Contains(c.Type)).Select(c => new ClaimViewModel { Type = c.Type, Value = c.Value }),
            },
        };

        return response;
    }
}
