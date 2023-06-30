using iolock_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;

namespace iolock_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public UserController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [AllowAnonymous]
        [HttpGet]
        //public async Task<IEnumerable<User>> GetUsers(string bearer)
        public async Task<IActionResult> GetUsers(string bearer)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "resource_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["iolock"]["roles"];

            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("admin"))
            {
                var users = await _dataAccess.GetUsersAsync();
                return Ok(users);
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("{Email}", Name = "GetUserByEmail")]
        //public async Task<IEnumerable<User>> GetUsers(string bearer)
        public async Task<IActionResult> GetUserByEmail(string bearer, String email)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "resource_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["iolock"]["roles"];

            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("admin"))
            {
                var users = await _dataAccess.GetUserByEmailAsync(email);
                return Ok(users);
            }

            return BadRequest();
        }
    }
}
