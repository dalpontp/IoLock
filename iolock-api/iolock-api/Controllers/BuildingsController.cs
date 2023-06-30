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
    public class BuildingsController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public BuildingsController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> getBuildings(string bearer)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "resource_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["iolock"]["roles"];
            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("admin"))
            {
                var buildings = await _dataAccess.GetBuildings();
                return Ok(buildings);
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [HttpGet("{Building}/Rooms")]
        public async Task<IActionResult> getBuildingRooms(string bearer, string building)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorUsername = token.Claims.First(c => c.Type == "resource_access").Value;

            var jsonObject = JsonConvert.DeserializeObject<JObject>(requestorUsername);

            var userRoles = jsonObject["iolock"]["roles"];
            string[] jsonStringArray = userRoles.Select(j => j.ToString()).ToArray();


            if (jsonStringArray.Contains("admin"))
            {
                var rooms = await _dataAccess.GetBuildingRooms(building);
                return Ok(rooms);
            }

            return BadRequest();
        }
    }
}
