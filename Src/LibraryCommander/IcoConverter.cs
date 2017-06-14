using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LibraryCommander
{
    /// <summary>
    /// Class loads icons for folders and different types of files
    /// </summary>
    public class IcoConverter: IValueConverter
    {
        private Dictionary<string, ImageSource> _imgCache = new Dictionary<string, ImageSource>();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            if (path == null)
                return null;

            // path is Directory
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return Directory.Exists(path) 
                    ? new Uri("pack://application:,,,/Resources/folder.png")
                    : new Uri("pack://application:,,,/Resources/folderWarning.png");
            }

            if (false == File.Exists(path))
                return new Uri("pack://application:,,,/Resources/fileWarning.png");

            ImageSource src = null;            

            string ext = Path.GetExtension(path);
            if (_imgCache.TryGetValue(ext, out src))
                return src;

            // https://stackoverflow.com/questions/2969821/display-icon-in-wpf-image
            // answer by Pablo Caballero

            Icon icon = Icon.ExtractAssociatedIcon(path);

            if (icon != null)
                using (Bitmap bmp = icon.ToBitmap())
                {
                    var stream = new MemoryStream();
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    src = BitmapFrame.Create(stream);
                }
            _imgCache.Add(ext, src);

            return src;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
