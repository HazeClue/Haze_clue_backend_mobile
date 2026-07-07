using System.Threading.Tasks;

namespace HazeClue.Core.Domain.Contracts
{
    public interface ILLMService
    {
        Task<string> GeneratePersonalizedTipAsync(string assessmentData, string smartwatchSummary, string focusSummary);
    }
}
