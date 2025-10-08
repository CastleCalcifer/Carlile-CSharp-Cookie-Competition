namespace Carlile_Cookie_Competition.Dtos
{
    public class SubmitVotesRequest
    {
        public List<VoteDto> Votes { get; set; } = new();
    }

    public class VoteDto
    {
        public required string UserId { get; set; }
        public int CookieId { get; set; }
        public bool IsBakerVote { get; set; }
        public string? VoterId { get; internal set; }
    }
}