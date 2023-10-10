using System;
using System.Windows.Media.Imaging;

namespace PDFGenerator.Core.Model
{

    public class ImageContent
        : DocumentContent
    {
        #region Constructors
        public ImageContent()
        {
            CaptionText         = null;
            MaxWidth            = double.MaxValue;
            MaxHeight           = double.MaxValue;
            CaptionPlacement    = CaptionPlacement.Bottom;
            SourceImage         = null;
        }

        public ImageContent(string path, string caption = null)
        {
            CaptionText = caption;
            var image = new BitmapImage(new Uri(path));
            SourceImage = image;
            CaptionPlacement = CaptionPlacement.Bottom;
            MaxWidth = image.Width;
            MaxHeight = image.Height;
        }

        public ImageContent(BitmapSource image, string caption = null)
        {
            CaptionText = caption;
            SourceImage = image;
            CaptionPlacement = CaptionPlacement.Bottom;
            MaxWidth = image.Width;
            MaxHeight = image.Height;
        }
        #endregion

        #region Properties
        public string           CaptionText         { get; set; }
        public double           MaxWidth            { get; set; }
        public double           MaxHeight           { get; set; }
        public CaptionPlacement CaptionPlacement    { get; set; }
        public BitmapSource     SourceImage         { get; set; }

        public override ItemType ItemType { get { return ItemType.ImageItem; } }

        #endregion
    }
}
