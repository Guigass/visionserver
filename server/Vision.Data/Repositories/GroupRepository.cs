using Vision.Data.Context;
using Vision.Data.Interfaces;
using Vision.Data.Models;

namespace Vision.Data.Repositories;
public class GroupRepository : BaseRepository<Group>, IGroupRepository
{
    public GroupRepository(VisionContext context) : base(context)
    {
        
    }

    public async Task AddCameraToGroup(Guid groupId, Guid cameraId)
    {
        var group = await GetByIdAsync(groupId);
        var camera = await _context.Cameras.FindAsync(cameraId);

        if (group == null || camera == null)
            return;

        group.CameraGroup.Add(new CameraGroup { Camera = camera });

        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Camera>> GetCamerasFromGroup(Guid groupId)
    {
        var group = await GetByIdAsync(groupId);

        return group.CameraGroup.Select(cg => cg.Camera)!;
    }

    public async Task RemoveCameraFromGroup(Guid groupId, Guid cameraId)
    {
        var group = await GetByIdAsync(groupId);
        var camera = await _context.Cameras.FindAsync(cameraId);

        if (group == null || camera == null)
            return;

        var cameraGroup = group.CameraGroup.FirstOrDefault(cg => cg.CameraId == cameraId);

        if (cameraGroup != null)
        {
            group.CameraGroup.Remove(cameraGroup);
            await _context.SaveChangesAsync();
        }
    }
}
