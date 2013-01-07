using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RoseSniffingPodcasts.Data;
using System.Threading.Tasks;
using RoseSniffingPodcasts.Services;
using Windows.Storage;

namespace PodcastStoreAppTesting
{

    public class EpisodeDownloaderStub : IEpisodeDownloader
    {
        private Task task;
        public void SetReturnValue(Task downloadTask)
        {
            this.task = downloadTask;
        }
        public Task SaveUrlAsync(Windows.Storage.StorageFolder folder, System.Net.Http.HttpClient client, string path)
        {
            return task;
        }
        public Task<StorageFolder> VerifyFolderCreation()
        {
            return Task.FromResult(default(StorageFolder));
        }
    }

    [TestClass]
    public class DownloadTests
    {
        [TestMethod]
        public async Task TestFastDownload()
        {
            int executeChanged = 0;
            var downloader = new EpisodeDownloaderStub();
            var task = new Task(() => {});
            task.Start();
            downloader.SetReturnValue(task);
            var underTest = new PodcastSeries("1", "testCollection", string.Empty, "A test collection", downloader);
            underTest.DownloadHander.CanExecuteChanged +=(_, __) => executeChanged++;
            PodcastEpisode episode = new PodcastEpisode("2", "title", "subtitle", string.Empty, "http://nowhere.com/episode1.mp3", "This is a podcast", underTest);
            underTest.Items.Add(episode);
            underTest.SelectedEpisodes.Add(episode);
            underTest.DownloadHander.Execute(null);

            // want to await the ownded task.
            // check the result
            await underTest.ActiveDownload;

            Assert.AreEqual(2, executeChanged);
            Assert.AreEqual(0, underTest.SelectedEpisodes.Count);
        }

        [TestMethod]
        public async Task TestSlowDownload()
        {
            int executeChanged = 0;
            var downloader = new EpisodeDownloaderStub();
            var task = new Task(() => { });
            downloader.SetReturnValue(task);
            var underTest = new PodcastSeries("1", "testCollection", string.Empty, "A test collection", downloader);
            underTest.DownloadHander.CanExecuteChanged += (_, __) => executeChanged++;
            PodcastEpisode episode = new PodcastEpisode("2", "title", "subtitle", string.Empty, "http://nowhere.com/episode1.mp3", "This is a podcast", underTest);
            underTest.Items.Add(episode);
            underTest.SelectedEpisodes.Add(episode);
            underTest.DownloadHander.Execute(null);

            Assert.AreEqual(1, executeChanged);
            Assert.AreEqual(1, underTest.SelectedEpisodes.Count);

            task.Start();

            // want to await the ownded task.
            // check the result
            await underTest.ActiveDownload;


            Assert.AreEqual(2, executeChanged);
            Assert.AreEqual(0, underTest.SelectedEpisodes.Count);
        }

        [TestMethod]
        public async Task TestErrorDownload()
        {
            int executeChanged = 0;
            var downloader = new EpisodeDownloaderStub();
            var task = new Task(() => { throw new InvalidOperationException("Well, that failed"); });
            downloader.SetReturnValue(task);
            var underTest = new PodcastSeries("1", "testCollection", string.Empty, "A test collection", downloader);
            underTest.DownloadHander.CanExecuteChanged += (_, __) => executeChanged++;
            PodcastEpisode episode = new PodcastEpisode("2", "title", "subtitle", string.Empty, "http://nowhere.com/episode1.mp3", "This is a podcast", underTest);
            underTest.Items.Add(episode);
            underTest.SelectedEpisodes.Add(episode);
            underTest.DownloadHander.Execute(null);

            Assert.AreEqual(1, executeChanged);
            Assert.AreEqual(1, underTest.SelectedEpisodes.Count);

            task.Start();

            // want to await the ownded task.
            // check the result
            try
            {
                await underTest.ActiveDownload;
                Assert.Fail("Exception not generated");
            }
            catch (InvalidOperationException)
            {
                return;
            }
        }
    
    }
}
