using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;
using Vision.Data.Interfaces;
using VisionAPI.Notifications;

namespace VisionAPI.Controllers;

[ApiController]
[Route("[controller]")]
public abstract class MainController : Controller
{
    private readonly INotificador _notificador;
    public readonly IUser AppUser;

    protected Guid UsuarioId { get; set; }

    protected bool UsuarioAutenticado { get; set; }

    public MainController(INotificador notificador, IUser appUser)
    {
        _notificador = notificador;
        AppUser = appUser;

        if (AppUser.IsAuthenticated())
        {
            UsuarioId = AppUser.GetUserId()!.Value;
            UsuarioAutenticado = true;
        }
    }

    protected bool OperacaoValida()
    {
        return !_notificador.TemNotificacao();
    }

    // Validação de notificações de erro
    protected ActionResult CustomResponse(object? result = null)
    {
        if (OperacaoValida())
        {
            return Ok(new
            {
                success = true,
                data = result
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = _notificador.ObterNotificacoes().Select(p => p.Mensagem)
        });
    }

    protected ActionResult CustomResponse(ModelStateDictionary modelState)
    {
        if (!modelState.IsValid) NotificarErroModelInvalida(modelState);

        return CustomResponse();
    }

    protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
    {
        var erros = modelState.Values.SelectMany(p => p.Errors);
        foreach (var erro in erros)
        {
            var errorMessage = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
            NotificarErro(errorMessage);
        }
    }

    protected void NotificarErro(string mensagem)
    {
        _notificador.Handle(new Notificacao(mensagem));
    }
}
