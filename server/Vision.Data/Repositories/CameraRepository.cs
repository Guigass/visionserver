using Vision.Data.Context;
using Vision.Data.Interfaces;
using Vision.Data.Models;

namespace Vision.Data.Repositories;
public class CameraRepository : BaseRepository<Camera>, ICameraRepository
{
    public CameraRepository(VisionContext context) : base(context)
    {
    }
}
