using System.ComponentModel.DataAnnotations;

namespace DocsWASM.Pages.LoginRegister
{
    public class LoginRegisterModels
    {
        public struct LoginView
        {
            public bool badCredentials;
            public string registrationError;
        }

        public struct LoginResult
        {
            public bool success;
            public ulong id;
            public string errorMessage;
            public string hash;
        }

        public struct RegisterView
        {
            public string ipAdress;
            public string userAgent;
        }

        public struct RegisterResult
        {
            public bool success;
            public string message;
        }

        public class RegisterForm
        {
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress]
            [StringLength(50, ErrorMessage = "Must be less than 50 characters.")]
            public string Email { get; set; }

            [Required]
            [Range(typeof(bool), "false", "true", ErrorMessage = "This form disallows unapproved ships.")]
            public bool IsPrivateFullName { get; set; }

            [Required(ErrorMessage = "First name is required")]
            [StringLength(100, MinimumLength = 5, ErrorMessage = "Must be between 5 and 100 characters")]
            public string FirstName { get; set; }


            [Required(ErrorMessage = "Last name is required")]
            [StringLength(100, MinimumLength = 5, ErrorMessage = "Must be between 5 and 100 characters")]
            public string LastName { get; set; }


            [Required(ErrorMessage = "Username is required")]
            [StringLength(50, MinimumLength = 5, ErrorMessage = "Must be between 5 and 50 characters.")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [StringLength(50, ErrorMessage = "Must be between 10 and 50 characters.", MinimumLength = 10)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required(ErrorMessage = "Confirm Password is required.")]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords must be identical.")]
            public string ConfirmPassword { get; set; }
        }

    }
}
