namespace RoseSniffingPodcasts.Data
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Collections.ObjectModel;
    using global::System.Collections.Specialized;
    using global::System.Threading.Tasks;
    using global::System.Windows.Input;
    using global::Windows.UI.Popups;
    using global::RoseSniffingPodcasts.Services;

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class PodcastSeries : CommonDataElement
    {
        private const string DownloadSelectedEpisodes = "Download Selected Episodes";

        private class DownloadCommandHandler : ICommand
        {
            private PodcastSeries owner;
            private readonly IEpisodeDownloader downloader;
            public DownloadCommandHandler(PodcastSeries owner, IEpisodeDownloader downloader)
            {
                this.owner = owner;
                this.downloader = downloader;
                owner.SelectedEpisodes.CollectionChanged += (_, __) => CanExecuteChanged(this, EventArgs.Empty);
            }

            public bool CanExecute(object parameter)
            {
                return owner.selectedEpisodes.Count > 0;
            }

            public event EventHandler CanExecuteChanged;

            public async void Execute(object parameter)
            {
                var result = Task.FromResult(default(IUICommand));
                owner.DownloadVisible = false;
                owner.ProgressVisible = true;
                var folder = await downloader.VerifyFolderCreation();
                using (var client = new System.Net.Http.HttpClient())
                {
                    // find all selected episodes.
                    try
                    {
                        foreach (var episode in owner.selectedEpisodes)
                        {
                            var path = episode.Description;
                            await downloader.SaveUrlAsync(folder, client, path);
                        }
                    }
                    catch (Exception)
                    {
                        // Umm, some download failed.
                        var errMsg = new MessageDialog("One or more downloads failed");
                        result = errMsg.ShowAsync().AsTask();
                    }
                    await result;
                }
                owner.DownloadVisible = true;
                owner.ProgressVisible = false;
                owner.selectedEpisodes.Clear();
            }
        }

        public PodcastSeries(String uniqueId, String title, String imagePath, String description)
            : this(uniqueId, title, imagePath, description, new EpisodeDownloader())
        {
        }

        // constructor for testing
        public PodcastSeries(String uniqueId, String title, String imagePath, String description, IEpisodeDownloader downloader)
            : base(uniqueId, title, DownloadSelectedEpisodes, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
            this.downloadHander = new DownloadCommandHandler(this, downloader);
        }

        public Task ActiveDownload { get; set; }

        private bool downloadVisible = true;
        public bool DownloadVisible
        {
            get { return downloadVisible; }
            set { base.SetProperty(ref downloadVisible, value); }
        }

        private bool progressVisible;
        public bool ProgressVisible
        {
            get { return progressVisible; }
            set { base.SetProperty(ref progressVisible, value); }
        }

        public ICommand DownloadHander { get { return this.downloadHander; } }
        private DownloadCommandHandler downloadHander;

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<PodcastEpisode> _items = new ObservableCollection<PodcastEpisode>();
        public ObservableCollection<PodcastEpisode> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<PodcastEpisode> _topItem = new ObservableCollection<PodcastEpisode>();
        public ObservableCollection<PodcastEpisode> TopItems
        {
            get { return this._topItem; }
        }

        private ObservableCollection<PodcastEpisode> selectedEpisodes = new ObservableCollection<PodcastEpisode>();
        public ObservableCollection<PodcastEpisode> SelectedEpisodes
        {
            get { return this.selectedEpisodes; }
        }
    }
}
