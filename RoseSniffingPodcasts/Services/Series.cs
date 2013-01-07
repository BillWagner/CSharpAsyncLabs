namespace RoseSniffingPodcasts.Services
{
    using global::System.Collections.Generic;
    using global::System.Threading.Tasks;

    public class Series
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ImagePath { get; set; }
        public string Description { get; set; }

        public IEnumerable<Episode> Episodes { get { return episodes; } }
        public void SetEpisodes(Task<IEnumerable<Episode>> parseTask)
        {
            episodes = parseTask.Result;
        }
        private IEnumerable<Episode> episodes;
    }
}
