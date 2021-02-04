namespace API.Helper
{
    public class MessageParams : PaginationParams
    {
        public string email { get; set; }

        public string Container { get; set; } = "Unread";
    }
}