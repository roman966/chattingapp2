namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
            

        }
        public Connection(string connectionId, string useremail)
        {
            ConnectionId = connectionId;
            Useremail = useremail;

        }
        
        public string ConnectionId { get; set; }

        public string Useremail { get; set; }
    }
}