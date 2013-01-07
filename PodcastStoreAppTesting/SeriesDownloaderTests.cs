using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RoseSniffingPodcasts.Data;
using RoseSniffingPodcasts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PodcastStoreAppTesting
{
    public class FeedDownloaderStub : IFeedDownloader
    {
        private Task<Series> result;

        public Task<Series> RetrieveFeed(string feedAddr)
        {
            return result;
        }

        // Change this to take an array of 7 tasks. 
        // Return each task in order.
        // cycle if the array is comlpeted.
        public void SetResult(Task<Series> result)
        {
            this.result = result;
        }
    }

    [TestClass]
    public class SeriesDownloaderTests
    {
        [TestMethod]
        public void NullDownloaderFails()
        {
            var undertest = new PodcastCollection(default(IFeedDownloader));
            Assert.AreEqual(0, undertest.AllGroups.Count);
        }

        [TestMethod]
        public async Task FastDownloadWorks()
        {
            var downloader = new FeedDownloaderStub();
            var series = new Series();
            await series.SetEpisodes(Task.FromResult(new List<Episode>().AsEnumerable()));
            downloader.SetResult(Task.FromResult(series));
            var underTest = new PodcastCollection(downloader);

            Assert.AreEqual(7, underTest.AllGroups.Count);
        }

        [TestMethod]
        public async Task SlowDownloadWorks()
        {
            var downloader = new FeedDownloaderStub();
            var series = new Series();

            var episodeTCS = new TaskCompletionSource<IEnumerable<Episode>>();
            var downloadTCS = new TaskCompletionSource<Series>();

            var parseTask = series.SetEpisodes(episodeTCS.Task);
            downloader.SetResult(downloadTCS.Task);

            var underTest = new PodcastCollection(downloader);

            episodeTCS.TrySetResult(new List<Episode>().AsEnumerable());
            downloadTCS.TrySetResult(series);
            await parseTask;

            Assert.AreEqual(7, underTest.AllGroups.Count);
        }

        [TestMethod]
        public async Task ExcpetionsCreateEmptySeries()
        {
            var downloader = new FeedDownloaderStub();
            var series = new Series();

            var episodeTCS = new TaskCompletionSource<IEnumerable<Episode>>();
            var downloadTCS = new TaskCompletionSource<Series>();

            var parseTask = series.SetEpisodes(episodeTCS.Task);
            downloader.SetResult(downloadTCS.Task);

            var underTest = new PodcastCollection(downloader);

            episodeTCS.TrySetResult(new List<Episode>().AsEnumerable());
            downloadTCS.TrySetException(new NullReferenceException());
            await parseTask;

            Assert.AreEqual(0, underTest.AllGroups.Count);
        }



    }
}
