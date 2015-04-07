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
                UpdateTile(tile);
            }
        }

        public static async void UpdateTile(SecondaryTile tile)
        {
            var currentShow = tvShows.FirstOrDefault(s => s.Title == tile.Arguments.Split(new char[]{'_'})[1]);
            XmlDocument wideTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);
            XmlNodeList tileTextAttributes = wideTileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "Notification Text";
            tileTextAttributes[1].InnerText = "Line1";
            tileTextAttributes[2].InnerText = "Line2";
            tileTextAttributes[3].InnerText = "Line3";

            XmlNodeList tileImageAttributes = wideTileXml.GetElementsByTagName("image");
            string uriString = DownloadHelper.GetRemoteUri(currentShow.Thumbnail);
            (tileImageAttributes[0] as XmlElement).SetAttribute("src", uriString);

            //XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);

            //XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            //squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode("This text was delivered through a notification"));

            //IXmlNode node = wideTileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            //wideTileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(wideTileXml);

            TileUpdater secondaryTileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tile.TileId);
            secondaryTileUpdater.Update(tileNotification);

        }
    }
}
