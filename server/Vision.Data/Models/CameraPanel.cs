using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Models;
public class CameraPanel : Entity
{
    public Guid CameraId { get; set; }
    public virtual Camera? Camera { get; set; }

    public Guid PanelId { get; set; }
    public virtual Panel? Panel { get; set; }
}
