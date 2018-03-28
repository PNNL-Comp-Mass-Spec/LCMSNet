using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows;
using LcmsNet.Method;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LcmsNetSDK.Logging;
using LcmsNetSDK.Method;

namespace LcmsNet.Reporting
{
    /// <summary>
    /// Class that builds a directory of error reporting documents for easy reporting of problems.
    /// </summary>
    public class ErrorReportBuilder
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ErrorReportBuilder()
        {
            BasePath = "ErrorReports";
        }

        /// <summary>
        /// Gets or sets the path of the directory where errors are created.
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Creates a report zip file for error reporting.
        /// </summary>
        /// <param name="errorControls"></param>
        /// <param name="methods"></param>
        /// <param name="logPath"></param>
        public string CreateReport(List<ContentControl> errorControls,
            List<LCMethod> methods,
            string logPath,
            string hardwarePath)
        {
            var now = DateTime.Now;
            var newPath = string.Format("report_{0:0000}{1:00}{2:00}_{3:00}{4:00}{5:00}", now.Year,
                now.Day,
                now.Month,
                now.Hour,
                now.Minute,
                now.Second);

            var didCreate = CreateAppropiatePaths(BasePath, newPath);
            if (!didCreate)
            {
                return null;
            }

            newPath = Path.Combine(BasePath, newPath);
            CreateScreenShots(newPath, errorControls);
            SaveMethods(newPath, methods);

            try
            {
                File.Copy(logPath, Path.Combine(newPath, Path.GetFileName(logPath)));
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not copy the current log file.", ex);
            }

            try
            {
                File.Copy(hardwarePath, Path.Combine(newPath, Path.GetFileName(hardwarePath)));
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not copy the current hardware configuration file.", ex);
            }

            var zipPath = newPath + ".zip";
            try
            {
                CreateZip(newPath, zipPath);
                Directory.Delete(newPath, true);
            }
            catch (Exception ex)
            {
                zipPath = null;
                ApplicationLogger.LogError(0, "Could not create the error report zip file.", ex);
            }

            return zipPath;
        }

        /// <summary>
        /// Copies the zip file report to the server path provided.
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="cart"></param>
        /// <param name="errorPath"></param>
        public static void CopyReportToServer(string zipPath, string cart, string errorPath)
        {
            if (zipPath != null)
            {
                try
                {
                    var cartPath = Path.Combine(errorPath, cart);
                    var exists = Directory.Exists(cartPath);

                    if (!exists)
                    {
                        Directory.CreateDirectory(cartPath);
                    }
                    File.Copy(zipPath, Path.Combine(cartPath, Path.GetFileName(zipPath)));
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0, "Could not copy the error report file to the server.", ex);
                }
            }
        }

        #region

        /// <summary>
        /// Creates an image from a control as a screenshot.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="minWidth"></param>
        /// <param name="minHeight"></param>
        /// <returns></returns>
        private BitmapSource CreateImage(ContentControl control, double minWidth, double minHeight)
        {
            return CaptureScreen(control, minWidth, minHeight, 96, 96);
        }

        private static BitmapSource CaptureScreen(ContentControl target, double minWidth, double minHeight, double dpiX, double dpiY)
        {
            if (target == null)
            {
                return null;
            }
            var bounds = VisualTreeHelper.GetDescendantBounds(target);
            if (bounds.IsEmpty)
            {
                bounds = new Rect(new System.Windows.Point(0, 0), new System.Windows.Size(300, 300));
            }
            bounds.Width = Math.Max(bounds.Width, minWidth);
            bounds.Height = Math.Max(bounds.Height, minHeight);
            var rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0), (int)(bounds.Height * dpiY / 96.0), dpiX, dpiY, PixelFormats.Pbgra32);
            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen())
            {
                // Draw a solid background...
                ctx.DrawRectangle(System.Windows.Media.Brushes.White, null, new Rect(new System.Windows.Point(), bounds.Size));
                var vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new System.Windows.Point(), bounds.Size));
            }
            rtb.Render(dv);
            return rtb;
        }

        /// <summary>
        /// Creates a screenshot for all of the provided controls.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="controls"></param>
        private void CreateScreenShots(string path, List<ContentControl> controls)
        {
            try
            {
                var width = 800d;
                var height = 600d;

                foreach (var control in controls)
                {
                    width = Math.Max(width, Math.Max(control.ActualWidth, double.IsNaN(control.Width) ? 0 : control.Width));
                    height = Math.Max(height, Math.Max(control.ActualHeight, double.IsNaN(control.Height) ? 0 : control.Height));
                }

                foreach (var control in controls)
                {
                    var name = control.GetType().Name;
                    var imagePath = Path.Combine(path, name + ".png");

                    var image = CreateImage(control, width, height);
                    using (var pngFile = new FileStream(imagePath, FileMode.Create))
                    {
                        var encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(image));
                        encoder.Save(pngFile);
                    }
                }
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0, "Could not create control screenshots.", ex);
            }
        }

        /// <summary>
        /// Creates the appropiate directories.
        /// </summary>
        /// <returns></returns>
        private bool CreateAppropiatePaths(string basePath, string newPath)
        {
            if (!Directory.Exists(basePath))
            {
                try
                {
                    Directory.CreateDirectory(basePath);
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0,
                        string.Format("Could not create the root error reporting directory {0}.", basePath), ex);
                    return false;
                }
            }

            try
            {
                Directory.CreateDirectory(Path.Combine(basePath, newPath));
            }
            catch (Exception ex)
            {
                ApplicationLogger.LogError(0,
                    string.Format("Could not create a error reporting directory {0}.", basePath), ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Saves a list of LC-methods to the path provided.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="methods"></param>
        private void SaveMethods(string path, List<LCMethod> methods)
        {
            var writer = new LCMethodWriter();
            foreach (var method in methods)
            {
                try
                {
                    writer.WriteMethod(Path.Combine(path, method.Name) + ".xml", method);
                }
                catch (Exception ex)
                {
                    ApplicationLogger.LogError(0,
                        string.Format("Could not save the LC method {0} for error reporting.", method.Name), ex);
                }
            }
        }

        private void CreateZip(string sourcePath, string zipPath)
        {
            ZipFile.CreateFromDirectory(sourcePath, zipPath);
        }

        #endregion
    }
}