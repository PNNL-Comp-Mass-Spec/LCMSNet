using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;

namespace EMSL.DocumentGenerator.Core.Model
{
 
    public class ImageContent
        : DocumentContent
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

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
            Bitmap image = new Bitmap(path);
            SourceImage = ConvertFromImage(image);
            CaptionPlacement = CaptionPlacement.Bottom;
            MaxWidth = image.Width;
            MaxHeight = image.Height;
        }

        public ImageContent(System.Drawing.Bitmap image, string caption = null)
        {
            CaptionText = caption;
            ImageContent content = new ImageContent();
            SourceImage = ConvertFromImage(image);
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

        #region Methods
        private static BitmapSource ConvertFromImage(System.Drawing.Bitmap image)
        {
            // Unmanaged object, MUST be deleted manually, done in finally block.
            IntPtr hBitmap = image.GetHbitmap();
            BitmapSource source = null;
            try
            {
                source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                
                DeleteObject(hBitmap);
            }
            return source;
        }
        #endregion
    }
}
