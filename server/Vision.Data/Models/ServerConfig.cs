using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vision.Data.Models;
public class ServerConfig : Entity
{
    public bool OnlyProcessWhenIsRequested { get; set; } = true;
    public int IdleTimeToStopProcess { get; set; } = 30;
}
