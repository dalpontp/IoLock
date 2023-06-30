namespace iolock_api.Models
{
    public class UserEntity
    {
        public int? Id { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string PreferredUsername { get; set; }
        public bool EmailVerified { get; set; }

        //public User(string givenName, string familyName, string email, string preferredUsername, bool emailVerified)
        //{
        //    GivenName = givenName;
        //    FamilyName = familyName;
        //    Email = email;
        //    PreferredUsername = preferredUsername;
        //    EmailVerified = emailVerified;
        //}
    }
}
