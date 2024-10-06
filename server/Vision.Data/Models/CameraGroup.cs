using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Models;
public class CameraGroup : Entity
{
    public Guid CameraId { get; set; }
    public virtual Camera? Camera { get; set; }

    public Guid GroupId { get; set; }
    public virtual Group? Group { get; set; }
}
