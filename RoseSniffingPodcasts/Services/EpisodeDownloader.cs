namespace RoseSniffingPodcasts.Services
{
    using global::System;
    using global::System.Linq;
    using global::System.Net.Http;
    using global::System.Threading.Tasks;
    using global::Windows.Storage;
    using global::System.IO;

    public class EpisodeDownloader : IEpisodeDownloader
    {
        public void SaveUrlAsync(StorageFolder folder, HttpClient client, string path)
        {
            var destName = path.Split('/').Last();
            var content = client.GetByteArrayAsync(path).Result;
            var destFile = folder.CreateFileAsync(destName, CreationCollisionOption.GenerateUniqueName).GetResults();
            using (var destStream = destFile.OpenStreamForWriteAsync().Result)
            {
                destStream.Write(content, 0, content.Length);
                destStream.FlushAsync().Wait();
            }
        }

        public StorageFolder VerifyFolderCreation()
        {
            var library = Windows.Storage.KnownFolders.MusicLibrary;
            return library.CreateFolderAsync("RoseSniffingPodcasts", 
                CreationCollisionOption.OpenIfExists).GetResults();
        }
    }
}
