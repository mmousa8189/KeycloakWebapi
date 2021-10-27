using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KeycloakWebapi.Controllers
{
    //[ApiControlle]
    [Route("test")]
    public class TestController : Controller
    {
        //anonymous
        [HttpGet("anonymous")]
        public IActionResult GetAnonymous() => Ok("Hello Anonymous");

        [Authorize("User")]
        [HttpGet("user")]
        public IActionResult GetUser() => Ok("Hello User");
        [HttpGet("admin")]
        public IActionResult GetAdmin() => Ok("Hello Admin");
        [HttpGet("all-user")]
        public IActionResult GetAllUser() => Ok("Hello All User");
    }
}