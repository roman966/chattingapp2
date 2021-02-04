using System.Collections.Generic;



namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
    
        public string email { get; set; }
        public string userFirstName { get; set; }

        public string userLastName { get; set; }

        

        public ICollection<Message> MessagesSent { get; set; }

        public ICollection<Message> MessagesReceived { get; set; }


    }
} 