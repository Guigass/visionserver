using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using VisionAPI.Extensions;
using VisionAPI.Notifications;

namespace VisionAPI.Controllers;

[Authorize]
public class CamerasController : MainController
{
    private readonly ICameraRepository _cameraRepository;
    private readonly IUser _user;
    private readonly UserManager<User> _userManager;

    public CamerasController(ICameraRepository cameraRepository,
                             INotificador notificador,
                             IUser user,
                             UserManager<User> userManager) : base(notificador, user)
    {
        _cameraRepository = cameraRepository;
        _user = user;
        _userManager = userManager;
    }

    // GET: Cameras
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Camera>>> GetCameras()
    {
        try
        {
            IEnumerable<Camera> cameras;

            // Se o usuário for Admin, retorna todas as câmeras
            if (_user.IsInRole("Admin"))
                cameras = await _cameraRepository.GetAllAsync();
            else
            {
                // Retorna apenas as câmeras que o usuário tem a claim de acesso
                var userClaims = _user.GetClaimsIdentity();
                var cameraIds = userClaims
                    .Where(c => c.Type == "CameraAccess")
                    .Select(c => Guid.Parse(c.Value))
                    .ToList();

                cameras = await _cameraRepository.GetByIds(cameraIds);
            }

            return CustomResponse(cameras);
        }
        catch (Exception)
        {
            NotificarErro("Ocorreu um erro, contate o suporte.");
            return CustomResponse();
        }
    }

    // GET: Cameras/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Camera>> GetCamera(Guid id)
    {
        var camera = await _cameraRepository.GetByIdAsync(id);

        if (camera == null)
            return NotFound();

        if (!_user.IsInRole("Admin") && !_user.HasClaim("CameraAccess", id.ToString()))
            return Forbid();

        return CustomResponse(camera);
    }

    // POST: Cameras
    [HttpPost]
    [RoleAuthorize("Admin")]
    public async Task<ActionResult<Camera>> PostCamera(Camera camera)
    {
        if (!ModelState.IsValid)
            return CustomResponse(ModelState);

        await _cameraRepository.CreateAsync(camera);

        var user = await _userManager.FindByIdAsync(_user.GetUserId().ToString()!);
        await _userManager.AddClaimAsync(user!, new Claim("CameraAccess", camera.Id.ToString()));

        return CustomResponse(camera);
    }

    // PUT: Cameras/5
    [HttpPut("{id}")]
    [RoleAuthorize("Admin")]
    public async Task<IActionResult> PutCamera(Guid id, Camera camera)
    {
        if (id != camera.Id) return BadRequest();

        if (!ModelState.IsValid) return CustomResponse(ModelState);

        await _cameraRepository.UpdateAsync(camera);

        return CustomResponse(camera);
    }

    // DELETE: Cameras/5
    [HttpDelete("{id}")]
    [RoleAuthorize("Admin")]
    public async Task<IActionResult> DeleteCamera(Guid id)
    {
        var camera = await _cameraRepository.GetByIdAsync(id);

        if (camera == null) return NotFound();

        await _cameraRepository.DeleteAsync(camera);

        var users = _userManager.Users.ToList();

        foreach (var user in users)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var cameraClaim = claims.FirstOrDefault(c => c.Type == "CameraAccess" && c.Value == id.ToString());

            if (cameraClaim != null)
                await _userManager.RemoveClaimAsync(user, cameraClaim);
        }

        return CustomResponse(camera);
    }

    // POST: Cameras/5/AssignClaim/{userId}
    [HttpPost("{id}/AssignClaim/{userId}")]
    [RoleAuthorize("Admin")]
    public async Task<IActionResult> AssignCameraClaim(Guid id, Guid userId)
    {
        var camera = await _cameraRepository.GetByIdAsync(id);
        if (camera == null)
            return NotFound("Câmera não encontrada.");

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound("Usuário não encontrado.");

        var claim = new Claim("CameraAccess", id.ToString());
        var result = await _userManager.AddClaimAsync(user, claim);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                NotificarErro(error.Description);
            }
            return CustomResponse();
        }

        return Ok();
    }

}
