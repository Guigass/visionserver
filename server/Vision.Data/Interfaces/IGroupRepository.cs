using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Data.Models;

namespace Vision.Data.Interfaces;
public interface IGroupRepository : IRepositoryBase<Group>
{
    Task AddCameraToGroup(Guid groupId, Guid cameraId);

    Task RemoveCameraFromGroup(Guid groupId, Guid cameraId);

    Task<IEnumerable<Camera>> GetCamerasFromGroup(Guid groupId);
}
