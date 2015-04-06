using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace XBMCRemoteRT.Helpers
{
    public class TileHelper
    {
        public static void UpdateTile(string tileId)
        {
            XmlDocument wideTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150PeekImageAndText01);
            XmlNodeList tileTextAttributes = wideTileXml.GetElementsByTagName("text");
            tileTextAttributes[0].InnerText = "Notification Text";
            tileTextAttributes[1].InnerText = "Line1";
            tileTextAttributes[2].InnerText = "Line2";
            tileTextAttributes[3].InnerText = "Line3";

            XmlNodeList tileImageAttributes = wideTileXml.GetElementsByTagName("image");
            (tileImageAttributes[0] as XmlElement).SetAttribute("src", "http://www.olsug.org/wiki/images/9/95/Tux-small.png");

            //XmlDocument squareTileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquare150x150Text04);

            //XmlNodeList squareTileTextAttributes = squareTileXml.GetElementsByTagName("text");
            //squareTileTextAttributes[0].AppendChild(squareTileXml.CreateTextNode("This text was delivered through a notification"));

            //IXmlNode node = wideTileXml.ImportNode(squareTileXml.GetElementsByTagName("binding").Item(0), true);
            //wideTileXml.GetElementsByTagName("visual").Item(0).AppendChild(node);

            TileNotification tileNotification = new TileNotification(wideTileXml);

            TileUpdater secondaryTileUpdater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId);
            secondaryTileUpdater.Update(tileNotification);

        }
    }
}
