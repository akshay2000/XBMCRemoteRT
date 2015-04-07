using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using XBMCRemoteRT.Models.Video;
using XBMCRemoteRT.RPCWrappers;
using Newtonsoft.Json.Linq;
using XBMCRemoteRT.Models.Common;
using System.Threading.Tasks;

namespace XBMCRemoteRT.Helpers
{
    public class TileHelper
    {
        private static List<TVShow> tvShows;
        public static async void UpdateAllTiles()
        {
            tvShows = await VideoLibrary.GetTVShows();
            IReadOnlyList<SecondaryTile> tiles = await SecondaryTile.FindAllAsync();
            foreach (var tile in tiles)
            {
                await UpdateTile(tile);
            }
        }

        public static async Task UpdateTile(SecondaryTile tile)
        {
            var currentShow = tvShows.FirstOrDefault(s => s.Title == tile.Arguments.Split(new char[]{'_'})[1]);
            JObject sort = new JObject(
                new JProperty("order", "ascending"),
                new JProperty("method", "playcount"));

            Limits limits = new Limits(0, 3);

            List<Episode> episodes = await VideoLibrary.GetEpisodes(limits:limits, sort: sort, tvShowID: currentShow.TvShowId);

            ////Square tile
            //XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);
            //XmlNodeList tileTextAttributes = tileXml.GetElementsByTagName("text");
            //tileTextAttributes[0].InnerText = "Episodes";

            //for (int i = 0; i < episodes.Count; i++)
            //{
            //    tileTextAttributes[i + 1].InnerText = episodes[i].Label;
            //}

            //XmlNodeList tileImageAttributes = tileXml.GetElementsByTagName("image");
            //string uriString = DownloadHelper.GetRemoteUri(currentShow.Thumbnail);
            //(tileImageAttributes[0] as XmlElement).SetAttribute("src", uriString);

            //Wide tile
            XmlDocument wideTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWide310x150PeekImage02);
            XmlNodeList tileTextWideAttributes = wideTileXml.GetElementsByTagName("text");
            tileTextWideAttributes[0].InnerText = "Episodes";

            for (int i = 0; i < episodes.Count; i++)
            {
                tileTextWideAttributes[i + 1].InnerText = episodes[i].Label;
            }

            tileTextWideAttributes[4].InnerText = "Hi";

            XmlNodeList tileImageWideAttributes = wideTileXml.GetElementsByTagName("image");
            string uriWideString = await DownloadHelper.DownloadImageForTile(currentShow.Thumbnail);
            (tileImageWideAttributes[0] as XmlElement).SetAttribute("src", uriWideString);
            
            ////Packaging
            //IXmlNode node = wideTileXml.ImportNode(tileXml.GetElementsByTagName("binding").Item(0), true);
            //wideTileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(wideTileXml);

            TileUpdater secondaryTileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);
            secondaryTileUpdater.Update(tileNotification);

        }
    }
}
