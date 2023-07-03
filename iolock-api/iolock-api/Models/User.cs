namespace iolock_api.Models
{
    public class User
    {
        public int? Id { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string Email { get; set; }
        public string PreferredUsername { get; set; }
        public bool EmailVerified { get; set; }
    }
}
