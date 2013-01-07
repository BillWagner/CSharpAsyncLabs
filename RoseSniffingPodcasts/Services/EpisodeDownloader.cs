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
        public async Task SaveUrlAsync(StorageFolder folder, HttpClient client, string path)
        {
            var destName = path.Split('/').Last();
            var content = await client.GetByteArrayAsync(path);
            var destFile = await folder.CreateFileAsync(destName, CreationCollisionOption.GenerateUniqueName);
            using (var destStream = await destFile.OpenStreamForWriteAsync())
            {
                destStream.Write(content, 0, content.Length);
                await destStream.FlushAsync();
            }
        }

        public async Task<StorageFolder> VerifyFolderCreation()
        {
            var library = Windows.Storage.KnownFolders.MusicLibrary;
            return await library.CreateFolderAsync("RoseSniffingPodcasts", 
                CreationCollisionOption.OpenIfExists);
        }
    }
}
