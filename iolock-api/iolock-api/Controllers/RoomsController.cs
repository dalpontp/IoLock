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
    public class RoomsController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public RoomsController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [AllowAnonymous]
        [HttpGet("{Email}", Name = "GetUserAvailableRooms")]
        //public async Task<IEnumerable<User>> GetUsers(string bearer)
        public async Task<IActionResult> getAvailableRooms(string bearer, string email)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "realm_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["roles"];

            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("app-admin"))
            {
                var avaiableRooms = await _dataAccess.getUserAvailablesRoomsAsync(email);

                return Ok(avaiableRooms);
            }

            return BadRequest();
        }
        
        [AllowAnonymous]
        [HttpDelete("{room}/Building/{building}")]
        public async Task<IActionResult> RevokeUserPermission(string bearer, string email, string room, string building)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "realm_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["roles"];

            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("app-admin"))
            {
                var permissionRevoked = await _dataAccess.RevokeUserPermission(email, room, building);
                if (permissionRevoked == 0) return NotFound();
                return Ok(permissionRevoked);
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpPost("{room}/Building/{building}")]
        public async Task<IActionResult> GiveUserPermission(string bearer, string email, string room, string building)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "realm_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["roles"];

            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("app-admin"))
            {
                var permissionRevoked = await _dataAccess.GiveUserPermission(email, room, building);
                if (permissionRevoked == 0) return BadRequest();
                return Ok(permissionRevoked);
            }

            return BadRequest();
        }
    }
}
