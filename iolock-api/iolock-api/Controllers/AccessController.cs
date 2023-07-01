using iolock_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.IdentityModel.Tokens;

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

        [AllowAnonymous]
        [HttpGet]
        public async void Get(string bearer)
        {
            var jwtEncodedString = bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorEmail = token.Claims.First(c => c.Type == "email").Value;

            var requestorIsRegistered = await _dataAccess.GetUserByEmailAsync(requestorEmail);

            if (requestorIsRegistered == null)
            {
                var userRequestor = new UserEntity { 
                    GivenName = token.Claims.First(c => c.Type == "given_name").Value,
                    FamilyName = token.Claims.First(c => c.Type == "family_name").Value,
                    Email = token.Claims.First(c => c.Type == "email").Value,
                    PreferredUsername = token.Claims.First(c => c.Type == "preferred_username").Value,
                    EmailVerified = Convert.ToBoolean(token.Claims.First(c => c.Type == "email_verified").Value)
                };
                
                await _dataAccess.CreateUserAsync(userRequestor);
            }
            // dovrei controllare se l'utente è stato modificato e in caso modificarlo anche sul db
            
            //return requestorUsername;
        }

        [AllowAnonymous]
        [HttpPost]
        public Task<string> Post(AccessRequest accessRequest)
        {
            var jwtEncodedString = accessRequest.Bearer;

            var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
            string requestorEmail = token.Claims.First(c => c.Type == "email").Value;

            var picPassword = _dataAccess.GetAccessPasswordAsync(requestorEmail, accessRequest.Code);

            return picPassword;
        }
    }
}
