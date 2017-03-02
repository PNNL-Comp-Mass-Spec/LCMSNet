using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LcmsNetDataClasses.Method;
using System;
using System.IO;
using LcmsNetDataClasses.Logging;
using LcmsNet.Method;
using Shell32;

namespace LcmsNet.Reporting
{
    /// <summary>
    /// Class that builds a directory of error reporting documents for easy reporting of problems.
    /// </summary>
    public class classErrorReportBuilder
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public classErrorReportBuilder()
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
        /// <param name="errorForm"></param>
        /// <param name="methods"></param>
        /// <param name="logPath"></param>
        public string CreateReport(List<Form> errorForms,
            List<classLCMethod> methods,
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
            CreateScreenShots(newPath, errorForms);
            SaveMethods(newPath, methods);

            try
            {
                File.Copy(logPath, Path.Combine(newPath, Path.GetFileName(logPath)));
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not copy the current log file.", ex);
            }

            try
            {
                File.Copy(hardwarePath, Path.Combine(newPath, Path.GetFileName(hardwarePath)));
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not copy the current hardware configuration file.", ex);
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
                classApplicationLogger.LogError(0, "Could not create the error report zip file.", ex);
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
                    classApplicationLogger.LogError(0, "Could not copy the error report file to the server.", ex);
                }
            }
        }

        #region

        /// <summary>
        /// Creates an image from a form as a screenshot.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private Image CreateImage(Form form)
        {
            var image = new Bitmap(form.Width, form.Height);
            using (var graphics = form.CreateGraphics())
            {
                form.DrawToBitmap(image, new Rectangle(0, 0, form.Width, form.Height));
            }
            return image;
        }

        /// <summary>
        /// Creates a screenshot for all of the provided forms.
        /// </summary>
        /// <param name="forms"></param>
        private void CreateScreenShots(string path, List<Form> forms)
        {
            try
            {
                foreach (var form in forms)
                {
                    var name = form.Name;
                    var imagePath = Path.Combine(path, name + ".png");

                    using (var image = CreateImage(form))
                    {
                        image.Save(imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                classApplicationLogger.LogError(0, "Could not create form screenshots.", ex);
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
                    classApplicationLogger.LogError(0,
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
                classApplicationLogger.LogError(0,
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
        private void SaveMethods(string path, List<classLCMethod> methods)
        {
            var writer = new classLCMethodWriter();
            foreach (var method in methods)
            {
                try
                {
                    writer.WriteMethod(Path.Combine(path, method.Name) + ".xml", method);
                }
                catch (Exception ex)
                {
                    classApplicationLogger.LogError(0,
                        string.Format("Could not save the LC method {0} for error reporting.", method.Name), ex);
                }
            }
        }

        private void CreateZip(string sourcePath, string zipPath)
        {
            //Create an empty zip file
            var emptyzip = new byte[]
            {
                80, 75, 5, 6, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };

            var fs = File.Create(zipPath);
            fs.Write(emptyzip, 0, emptyzip.Length);
            fs.Flush();
            fs.Close();
            fs = null;

            //Copy a folder and its contents into the newly created zip file
            var sc = new Shell32.ShellClass();
            var sourceFolder = sc.NameSpace(Path.GetFullPath(sourcePath));
            var destFolder = sc.NameSpace(Path.GetFullPath(zipPath));
            var items = sourceFolder.Items();
            destFolder.CopyHere(items, 20);

            //Ziping a file using the Windows Shell API
            //creates another thread where the zipping is executed.
            //This means that it is possible that this console app
            //would end before the zipping thread
            //starts to execute which would cause the zip to never
            //occur and you will end up with just
            //an empty zip file. So wait a second and give
            //the zipping thread time to get started
            System.Threading.Thread.Sleep(2000);
        }

        #endregion
    }
}