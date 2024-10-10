using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Vision.Data.Interfaces;
using Vision.Data.Models;
using VisionAPI.Notifications;

namespace VisionAPI.Controllers;

[Authorize]
public class GroupsController : MainController
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUser _user;
    private readonly UserManager<User> _userManager;

    public GroupsController(IGroupRepository groupRepository,
                             INotificador notificador,
                             IUser user,
                             UserManager<User> userManager) : base(notificador, user)
    {
        _groupRepository = groupRepository;
        _user = user;
        _userManager = userManager;
    }
}
