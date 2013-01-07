namespace RoseSniffingPodcasts.Data
{
    using global::System;
    using global::Windows.UI.Xaml.Media;
    using global::Windows.UI.Xaml.Media.Imaging;
    using global::System.Threading.Tasks;
    using global::System.Windows.Input;
    using global::Windows.UI.Popups;

    /// <summary>
    /// Podcast item data model.
    /// </summary>
    public class PodcastEpisode : CommonDataElement
    {
        private static IPlayback playbackControl;

        private class PlayPauseHandler : ICommand
        {
            private PodcastEpisode owner;
            public PlayPauseHandler(PodcastEpisode owner)
            {
                this.owner = owner;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                PodcastEpisode.playbackControl.setSource(new Uri(owner.Description, UriKind.Absolute));
                PodcastEpisode.playbackControl.PlayPause();
            }
        }

        public PodcastEpisode(String uniqueId, String title, String subtitle, String imagePath,
            String description, String content, PodcastSeries group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
            this.playCommand = new PlayPauseHandler(this);
        }

        private ICommand playCommand;
        public ICommand Play { get { return playCommand; } }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private PodcastSeries _group;
        public PodcastSeries Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }

        private ImageSource _imageLrg = null;
        public ImageSource ImageLarge
        {
            get { 
                if (_imageLrg == null)
                    _imageLrg = new BitmapImage(new Uri(CommonDataElement._baseUri, "Assets/podCast_160x160.png")); 
                return this._imageLrg;
            }
        }

        private ImageSource _imageSmall = null;
        public ImageSource ImageSmall
        {
            get { 
                if (_imageSmall == null)
                    _imageSmall = new BitmapImage(new Uri(CommonDataElement._baseUri, "Assets/podCast_40x40.png")); 
                return this._imageSmall;
            }
        }

        public void SetPlaybackControl(IPlayback playback)
        {
            playbackControl = playback; ;
        }
    }
}
