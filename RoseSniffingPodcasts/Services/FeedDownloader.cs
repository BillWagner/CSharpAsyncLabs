namespace RoseSniffingPodcasts.Services
{
    using global::System.Collections.Generic;
    using global::System.Net.Http;
    using global::System.Threading.Tasks;
    using global::System.Xml.Linq;

    public class FeedDownloader : IFeedDownloader 
    {
        private const string itunesXMLNS = "http://www.itunes.com/dtds/podcast-1.0.dtd";
        private const string mediaXMLNS = "http://search.yahoo.com/mrss/";
        private const string ImageElementName = "image";
        private const string ChannelElementName = "channel";
        private const string LinkElementName = "link";
        private const string TitleElementName = "title";
        private const string HrefElementName = "href";
        private const string UrlElementName = "url";
        private const string DescriptionElementName = "description";
        private const string ItemElementName = "item";
        private const string ContentElementName = "content";
        private const string GuidElementName = "guid";
        private const string PubDateElementName = "pubDate";
        private const string EnclosureElementName = "enclosure";

        private static readonly XName ImageXName = XName.Get(ImageElementName, itunesXMLNS);
        private static readonly XName MediaXName = XName.Get(ContentElementName, mediaXMLNS);

        public async Task<Series> RetrieveFeed(string feedAddr)
        {
            string content = string.Empty;
            using (HttpClient client = new HttpClient())
                content = await client.GetStringAsync(feedAddr);

            var feed = XElement.Parse(content);
            var id = feed.Element(ChannelElementName).Element(LinkElementName).Value;
            var title = feed.Element(ChannelElementName).Element(TitleElementName).Value;
            var imagePath = feed.Element(ChannelElementName).Element(ImageXName).Attribute(HrefElementName).Value;
            var imageElement = feed.Element(ChannelElementName).Element(ImageElementName);
            if (imageElement != null)
                imagePath = imageElement.Element(UrlElementName).Value;
            var description = feed.Element(ChannelElementName).Element(DescriptionElementName).Value;
            var rVal = new Series
                {
                    Id = id,
                    Title = title,
                    ImagePath = imagePath,
                    Description = description
                };
            var items = feed.Element(ChannelElementName).Elements(ItemElementName);

            await rVal.SetEpisodes(Task.Run(() => ParseEpisodes(items)));
            return rVal;
        }

        private IEnumerable<Episode> ParseEpisodes(IEnumerable<XElement> items)
        {
            foreach (var item in items)
            {
                var itemID = item.Element(GuidElementName).Value;
                var itemTitle = item.Element(TitleElementName).Value;
                var itemSubtitle = item.Element(PubDateElementName).Value;
                var itemDescription = item.Element(DescriptionElementName).Value;
                var media = item.Element(MediaXName);
                if (media == null)
                    media = item.Element(EnclosureElementName);
                var download = (media != null) ? media.Attribute(UrlElementName).Value : string.Empty;
                var episode = new Episode
                {
                    ItemID = itemID,
                    ItemTitle = itemTitle,
                    Subtitle = itemSubtitle,
                    DownloadURL = download,
                    Description = itemDescription
                };
                yield return episode;
            }
        }
    }
}
