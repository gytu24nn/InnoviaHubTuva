using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BackEnd.Hubs;

[Authorize]
public class NotificationHub : Hub
{

}
