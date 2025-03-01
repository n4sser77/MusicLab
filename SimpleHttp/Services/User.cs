using System.ComponentModel.DataAnnotations;

namespace SimpleHttp.Services
{
    public class User
    {


        public int Id { get; set; }

        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; } = "";
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string Password { get; internal set; }
    }

    public class LogInModel
    {
        [Required]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}