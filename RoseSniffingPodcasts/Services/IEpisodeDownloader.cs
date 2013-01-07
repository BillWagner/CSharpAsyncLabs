namespace RoseSniffingPodcasts.Services
{
    using global::System;
    using global::System.Net.Http;
    using global::System.Threading.Tasks;
    using global::Windows.Storage;

    public interface IEpisodeDownloader
    {
        void SaveUrlAsync(StorageFolder folder, HttpClient client, string path);
        StorageFolder VerifyFolderCreation();
    }
}
