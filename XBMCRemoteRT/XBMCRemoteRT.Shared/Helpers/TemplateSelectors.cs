using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XBMCRemoteRT.Models.Files;

namespace XBMCRemoteRT.TemplateSelectors
{
    public class FileDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileTemplate { get; set; }
        public DataTemplate DirectoryTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            File f = (File)item;

            switch (f.FileType)
            {
                case "file":
                    return FileTemplate;
                case "directory":
                    return DirectoryTemplate;
                default:
                    throw new NotSupportedException();
            }
        }

    }
}