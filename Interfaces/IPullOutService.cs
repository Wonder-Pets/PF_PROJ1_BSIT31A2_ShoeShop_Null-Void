using WebApplication5.DTO;


namespace WebApplication5.Interfaces
{
    public interface IPullOutService
    {
        PullOutRequestDto CreatePullOut(CreatePullOutDto dto);
        void ApprovePullOut(Guid id, string approvedBy);
        IEnumerable<PullOutRequestDto> GetAllPullOuts();
    }
}