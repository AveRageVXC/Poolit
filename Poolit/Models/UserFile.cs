namespace Poolit.Models
{
    public class UserFile
    {
        public ulong Id { get; set; }
        public ulong AuthorId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public ulong Size { get; set; }
    }
}
