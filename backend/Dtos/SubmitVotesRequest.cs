// Dtos/SubmitVotesRequest.cs
namespace Carlile_Cookie_Competition.Dtos
{
    public record SubmitVotesRequest(int Year, List<int> CookieIds, string? VoterId);
}