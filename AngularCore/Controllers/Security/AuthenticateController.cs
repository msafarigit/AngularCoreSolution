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
        //IActionResult type
        //The IActionResult return type is appropriate when multiple ActionResult return types are possible in an action.
        //The ActionResult types represent various HTTP status codes.
        
        [HttpGet]
        public IActionResult LogOut()
        {
            return Ok(new { status = StatusCodes.Status200OK , body = new { result = "test" } });
        }
    }
}