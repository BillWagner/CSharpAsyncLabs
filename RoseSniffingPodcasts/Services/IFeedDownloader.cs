namespace RoseSniffingPodcasts.Services
{
    using global::System.Threading.Tasks;

    public interface IFeedDownloader
    {
        Series RetrieveFeed(string feedAddr);
    }
}
