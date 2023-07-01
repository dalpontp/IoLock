using iolock_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetLogs(string bearer)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "realm_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["roles"];

            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("app-admin"))
            {
                var buildings = await _dataAccess.GetLogs();
                return Ok(buildings);
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("Users/{Email}")]
        public async Task<IActionResult> GetUserLogs(string bearer, string email)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "realm_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["roles"];

            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("app-admin"))
            {
                var buildings = await _dataAccess.GetUserLogs(email);
                return Ok(buildings);
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("Users/{Email}")]
        public async Task<IActionResult> PostUserLog(string bearer, string email, string room, string building)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "realm_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["roles"];

            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("app-admin"))
            {
                var buildings = await _dataAccess.InsertUserLogs(email, room, building);
                return Ok(buildings);
            }

            return BadRequest();
        }
    }
}
