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
    public class PlaybackStub : IPlayback
    {

        public void setSource(Uri source)
        {
        }

        public void PlayPause()
        {
        }
    }

    [TestClass]
    public class PlayPauseTests
    {
        [TestMethod]
        public void NullPlaybackControlThrowsException()
        {
            var playback = default(IPlayback);
            var underTest = new PodcastEpisode("1", "episode", "just some talking", string.Empty, "http://www.nowhere.com/episode1.mp3", "talking heads", default(PodcastSeries));
            underTest.SetPlaybackControl(playback);
            Assert.ThrowsException<NullReferenceException>(() => underTest.Play.Execute(null));
        }

        [TestMethod]
        public void SimplePlayback()
        {
            var playback = new PlaybackStub();
            var underTest = new PodcastEpisode("1", "episode", "just some talking", string.Empty, "http://www.nowhere.com/episode1.mp3", "talking heads", default(PodcastSeries));
            underTest.SetPlaybackControl(playback);
            underTest.Play.Execute(null);
        }
    }
}
