namespace RoseSniffingPodcasts.Services
{
    using global::System.Threading.Tasks;

    public interface IFeedDownloader
    {
        Task<Series> RetrieveFeed(string feedAddr);
    }
}
