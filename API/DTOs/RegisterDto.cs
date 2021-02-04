using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class RegisterDto
    {
       
        [Required]
        public string userFirstName {get; set;}

        [Required]
        public string userLastName {get; set;}
        
        [Required]
        [EmailAddress]
        public string email {get; set;}


    }
}