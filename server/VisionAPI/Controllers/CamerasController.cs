using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using VisionAPI.Notifications;

namespace VisionAPI.Controllers;

[Authorize]
public class CamerasController : MainController
{
    private readonly ICameraRepository _cameraRepository;
    private readonly IUser _user;

    public CamerasController(ICameraRepository cameraRepository, 
                             INotificador notificador, 
                             IUser user) : base(notificador, user)
    {
        _cameraRepository = cameraRepository;
        _user = user;
    }

    // GET: Cameras
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Camera>>> GetCameras()
    {
        try
        {
            var cameras = await _cameraRepository.GetAllAsync();

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
        {
            return NotFound();
        }

        return camera;
    }

    // POST: Cameras
    [HttpPost]
    public async Task<ActionResult<Camera>> PostCamera(Camera camera)
    {
        if (!ModelState.IsValid)
        {
            return CustomResponse(ModelState);
        }

        await _cameraRepository.CreateAsync(camera);

        return CustomResponse(camera);
    }

    // PUT: Cameras/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutCamera(Guid id, Camera camera)
    {
        if (id != camera.Id) return BadRequest();

        if (!ModelState.IsValid) return CustomResponse(ModelState);

        await _cameraRepository.UpdateAsync(camera);

        return CustomResponse(camera);
    }

    // DELETE: Cameras/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCamera(Guid id)
    {
        var camera = await _cameraRepository.GetByIdAsync(id);

        if (camera == null) return NotFound();

        await _cameraRepository.DeleteAsync(camera);

        return CustomResponse(camera);
    }
}
