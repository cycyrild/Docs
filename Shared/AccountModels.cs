namespace DocsWASM.Shared
{
    public class AccountModels
    {
        public enum UserType { Admin = 0, Student = 1, Teacher = 2}
        public class User
        {
            public uint Id { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public DateTime? LastLogin { get; set; }
            public string Bio { get; set; }
            public bool FullNamePrivacy { get; set; }
            public string? CreatedIp { get; set; }
            public string? LastIp   { get; set; }
            public UserType TypeOfUser { get; set; }
        }
    }
}
