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
     Don't create a web API controller by deriving from the Controller class.
     Controller derives from ControllerBase and adds support for views, so it's for handling web pages, not web API requests.
    */
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/[controller]/{id}")]
    [Route("api/[controller]/[action]")]
    public class ApiControllerBase : ControllerBase
    {
    }
}