using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using AngularCore.Hubs;

namespace AngularCore.Controllers.Chart
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartController : ControllerBase
    {
        private IHubContext<ChartHub> hubContext;

        public ChartController(IHubContext<ChartHub> hub)
        {
            hubContext = hub;
        }

        public IActionResult Get()
        {
            TimerManager timerManager = new TimerManager(() => hubContext.Clients.All.SendAsync("transferchartdata", DataManager.GetData()));

            return Ok(new { Message = "Request Completed" });
        }
    }
}
