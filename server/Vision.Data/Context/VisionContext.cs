using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vision.Data.Models;
using static Vision.Data.Models.Identity;

namespace Vision.Data.Context;

public class VisionContext : IdentityDbContext<User, AppRole, Guid, AppUserClaim, AppUserRole, AppUserLogin, AppRoleClaim, AppUserToken>
{
    public VisionContext(DbContextOptions<VisionContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Camera> Cameras { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<CameraGroup> CamerasGroups { get; set; }
    public DbSet<Panel> Panels { get; set; }
    public DbSet<CameraPanel> CamerasPanels { get; set; }
    public DbSet<ServerConfig> ServerConfigs { get; set; }
}