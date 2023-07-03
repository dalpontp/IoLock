using iolock_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using iolock_api.Services;

namespace iolock_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public LogsController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Authorize(Roles = "app-user")]
        [HttpGet]
        public async Task<IActionResult> GetLogs()
        {
            var result = await _dataAccess.GetLogs();

            return result != null ? Ok(result) : BadRequest();
        }

        [Authorize(Roles = "app-user")]
        [HttpGet("Users/{Email}")]
        public async Task<IActionResult> GetUserLogs(string email)
        {
            var result = await _dataAccess.GetUserLogs(email);

            return result != null ? Ok(result) : BadRequest();
        }

        [Authorize(Roles = "app-user")]
        [HttpPost("Users/{Email}")]
        public async Task<IActionResult> PostUserLog(string email, string room, string building)
        {
            var result = await _dataAccess.InsertUserLogs(email, room, building);

            return result != null ? Ok(result) : BadRequest();
        }
    }
}
