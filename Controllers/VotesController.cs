using Microsoft.AspNetCore.Mvc;
using Carlile_Cookie_Competition.Models;
using Carlile_Cookie_Competition.Data;
using Carlile_Cookie_Competition.Dtos;

[ApiController]
[Route("api/[controller]")]
public class VotesController : ControllerBase
{
    private readonly AppDbContext _context;
    public VotesController(AppDbContext context) => _context = context;

    [HttpPost]
    public IActionResult SubmitVotes([FromBody] SubmitVotesRequest request)
    {
        foreach (var vote in request.Votes)
        {
            if (!_context.Votes.Any(v => v.VoterId == vote.VoterId && v.CookieId == vote.CookieId))
            {
                _context.Votes.Add(new Vote
                {
                    VoterId = vote.VoterId,
                    CookieId = vote.CookieId,
                    //IsBakerVote = vote.IsBakerVote
                });
            }
        }
        _context.SaveChanges();
        return Ok();
    }
}