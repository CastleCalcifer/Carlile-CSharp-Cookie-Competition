namespace Carlile_Cookie_Competition.Models;
public class Cookie
{
    public int Id { get; set; }
    public int Year { get; set; }
    public string CookieName { get; set; } = "";
    public string ImageUrl { get; set; } = ""; // store relative path like "/images/cookie1.jpg"
    public int Score { get; set; } // optional denormalized score

    public string BakerId { get; set; } = "";
    public List<Vote> Votes { get; set; } = new();
}
