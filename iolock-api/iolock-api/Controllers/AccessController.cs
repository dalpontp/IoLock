using iolock_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;
using iolock_api.Services;

namespace iolock_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IDataAccess _dataAccess;

        public AccessController(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        [Authorize(Roles = "app-user")]
        [HttpGet]
        public async void Get(string bearer)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorEmail = token.Claims.First(c => c.Type == "email").Value;

            var requestorIsRegistered = await _dataAccess.GetUserByEmailAsync(requestorEmail);

            if (requestorIsRegistered == null)
            {
                var userRequestor = new User { 
                    GivenName = token.Claims.First(c => c.Type == "given_name").Value,
                    FamilyName = token.Claims.First(c => c.Type == "family_name").Value,
                    Email = token.Claims.First(c => c.Type == "email").Value,
                    PreferredUsername = token.Claims.First(c => c.Type == "preferred_username").Value,
                    EmailVerified = Convert.ToBoolean(token.Claims.First(c => c.Type == "email_verified").Value)
                };
                
                await _dataAccess.CreateUserAsync(userRequestor);
            }
            // dovrei controllare se l'utente è stato modificato e in caso modificarlo anche sul db
            UserService.Update(requestorIsRegistered, _dataAccess);
            //return requestorUsername;
        }

        [Authorize(Roles = "app-user")]
        [HttpPost]
        public async Task<IActionResult> Post(AccessRequest accessRequest)
        {
            //UserService.Update(accessRequest.Email, _dataAccess);

            var result = await _dataAccess.GetAccessPasswordAsync(accessRequest.Email, accessRequest.Code);

            return result != null ? Ok(result) : BadRequest();
        }
    }
}
