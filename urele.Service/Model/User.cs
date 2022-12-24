namespace urele.Service.Model
{
    public class User
    {
        public string username { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string surname { get; set; } = String.Empty;
        public string email { get; set; } = String.Empty;
        public string password { get; set; } = String.Empty;
        public DateTime? tokenExpiresOn { get; set; }
    }

    public class EditUser
    {
        public string username { get; set; } = String.Empty;
        public string token { get; set; } = String.Empty;
        public string name { get; set; } = String.Empty;
        public string surname { get; set; } = String.Empty;
        public string email { get; set; } = String.Empty;
        public string password { get; set; } = String.Empty;
        public DateTime? tokenExpiresOn { get; set; }
    }
}
