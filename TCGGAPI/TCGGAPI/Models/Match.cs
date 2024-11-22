namespace TCGGAPI.Models;

public class Match
{
    public int Id { get; set; }
    public Board Board { get; set; }
    public string Status { get; set; }
}