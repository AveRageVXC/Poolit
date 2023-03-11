namespace Poolit.Models;

public class UserFile
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public int Size { get; set; }
}
