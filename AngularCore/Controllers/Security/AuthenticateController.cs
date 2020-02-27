using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AngularCore.Controllers.Security
{
    public class AuthenticateController : ApiControllerBase
    {
        [HttpGet]
        public IActionResult LogOut()
        {
            return Ok(new { status = StatusCodes.Status200OK , body = new { result = "test" } });
        }
    }

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ApiControllerBase : ControllerBase
    {
    }

}