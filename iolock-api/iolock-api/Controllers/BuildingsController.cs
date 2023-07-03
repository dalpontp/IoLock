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
    public class BuildingsController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public BuildingsController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Authorize(Roles = "app-user")]
        [HttpGet]
        public async Task<IActionResult> GetBuildings()
        {
            var result = await _dataAccess.GetBuildings();

            return result != null ? Ok(result) : BadRequest();
        }

        [Authorize(Roles = "app-user")]
        [HttpGet("{Building}/Rooms")]
        public async Task<IActionResult> GetBuildingRooms(string building)
        {
            var result = await _dataAccess.GetBuildingRooms(building);

            return result != null ? Ok(result) : BadRequest();
        }
    }
}
