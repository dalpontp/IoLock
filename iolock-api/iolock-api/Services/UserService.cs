using iolock_api.Models;
using Newtonsoft.Json.Linq;

namespace iolock_api.Services
{
    public class UserService
    {
        public static async void Update(User keycloakUser, IDataAccess dataAccess)
        {
            Dictionary<string, object> diffs = new Dictionary<string, object>();

            User appUser = await dataAccess.GetUserByEmailAsync(keycloakUser.Email);

            if (appUser.GivenName != keycloakUser.GivenName) diffs["GivenName"] = keycloakUser.GivenName;
            if (appUser.FamilyName != keycloakUser.FamilyName) diffs["FamilyName"] = keycloakUser.FamilyName;
            if (appUser.PreferredUsername != keycloakUser.PreferredUsername) diffs["PreferredUsername"] = keycloakUser.PreferredUsername;
            //if (appUser.Email != keycloakUser.Email) diffs["Email"] = keycloakUser.Email;
            if (appUser.EmailVerified != keycloakUser.EmailVerified) diffs["EmailVerified"] = keycloakUser.EmailVerified;

            if (diffs.Count() > 0) await dataAccess.UpdateUserAsync(diffs, keycloakUser.Email);
        }
    }
}
