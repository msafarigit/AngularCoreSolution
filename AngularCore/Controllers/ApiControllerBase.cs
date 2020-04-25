using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AngularCore.Controllers
{
    /*
     
     ApiController attribute:
     The [ApiController] attribute can be applied to a controller class to enable the following opinionated, API-specific behaviors:
       - Attribute routing requirement
       - Automatic HTTP 400 responses
       - Binding source parameter inference
       - Multipart/form-data request inference
       - Problem details for error status codes

     The Problem details for error status codes feature requires a compatibility version of 2.2 or later. The other features require a compatibility version of 2.1 or later.
     Don't create a web API controller by deriving from the Controller class.
     Controller derives from ControllerBase and adds support for views, so it's for handling web pages, not web API requests.
    */

    /*
     The [ApiController] attribute makes attribute routing a requirement.
     Actions are inaccessible via conventional routes defined by UseEndpoints, UseMvc, or UseMvcWithDefaultRoute in Startup.Configure.
    */
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/[controller]/{id}")]
    [Route("api/[controller]/[action]")]
    public class ApiControllerBase : ControllerBase
    {
        //[HttpPost]
        //[Consumes("application/json")]
        //public IActionResult PostJson(IEnumerable<int> values) =>
        //Ok(new { Consumes = "application/json", Values = values });

        //[HttpPost]
        //[Consumes("application/x-www-form-urlencoded")]
        //public IActionResult PostForm([FromForm] IEnumerable<int> values) =>
        //    Ok(new { Consumes = "application/x-www-form-urlencoded", Values = values });
    }
}