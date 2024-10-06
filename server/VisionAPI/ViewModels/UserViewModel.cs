using System.ComponentModel.DataAnnotations;

namespace VisionAPI.ViewModels;

public class RegisterUserViewModel
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 5)]
    public string Password { get; set; } = string.Empty;

    [Compare("Password", ErrorMessage = "As senhas não conferem")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(200, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(200, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
    public string Lastname { get; set; } = string.Empty;
}

public class LoginUserViewModel
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [EmailAddress(ErrorMessage = "O campo {0} está em formato inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 5)]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponseViewModel
{
    public string AccessToken { get; set; } = string.Empty;
    public double ExpiresIn { get; set; }
    public UserTokenViewModel User { get; set; } = new UserTokenViewModel();
}

public class UserTokenViewModel
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public IEnumerable<ClaimViewModel> Claims { get; set; } = new List<ClaimViewModel>();
}

public class ClaimViewModel
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
