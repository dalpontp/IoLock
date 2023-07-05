using iolock_api.Models;
using iolock_api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace iolock_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public UsersController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Authorize(Roles = "app-user")]
        [HttpPost("NewUserCheck")]
        public async Task InsertIfNewUser()
        {
            var user = User.Identity;
            var username = User.Identity.Name;

            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            if (userEmail != null)
            {
                var webappUser = await _dataAccess.GetUserByEmailAsync(userEmail);
                if (webappUser == null)
                {
                    var newUser = new Models.User
                    {
                        GivenName = User.FindFirstValue(ClaimTypes.GivenName),
                        FamilyName = User.FindFirstValue(ClaimTypes.Surname),
                        Email = userEmail,
                        EmailVerified = Convert.ToBoolean(User.FindFirstValue("email_verified")),
                        PreferredUsername = username
                    };
                    await _dataAccess.InsertUserAsync(newUser);
                }
            }
        }

        [Authorize(Roles = "app-admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _dataAccess.GetUsersAsync();

            return result != null ? Ok(result) : BadRequest();
        }

        [Authorize(Roles = "app-admin")]
        [HttpGet("{Email}")]
        public async Task<IActionResult> GetUserByEmail(String email)
        {
            var result = await _dataAccess.GetUserByEmailAsync(email);

            return result != null ? Ok(result) : BadRequest();
        }

        [Authorize(Roles = "app-admin")]
        [HttpGet("{Email}/Rooms", Name = "GetUserAvailableRooms")]
        public async Task<IActionResult> GetAvailableRooms(string email)
        {
            var result = await _dataAccess.GetUserAvailablesRoomsAsync(email);

            return result != null ? Ok(result) : BadRequest();
        }

        [Authorize(Roles = "app-admin")]
        [HttpDelete("{Email}/Rooms/{room}/Building/{building}")]
        public async Task<IActionResult> RevokeUserPermission(string email, string room, string building)
        {
            var permissionRevoked = await _dataAccess.RevokeUserPermission(email, room, building);
            return permissionRevoked != 0 ? Ok(permissionRevoked) : NotFound();
        }

        [Authorize(Roles = "app-admin")]
        [HttpPost("{Email}/Rooms/{room}/Building/{building}")]
        public async Task<IActionResult> GiveUserPermission(string email, string room, string building)
        {
            var permissionGived = await _dataAccess.GiveUserPermission(email, room, building);
            return permissionGived != 0 ? Ok(permissionGived) : NotFound();
        }
    }
}
