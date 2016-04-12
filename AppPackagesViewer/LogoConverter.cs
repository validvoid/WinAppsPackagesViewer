using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using Windows.ApplicationModel;
using Windows.Storage;
using Windows.Storage.BulkAccess;

namespace ProjMarduk.AppPackagesViewer
{
    class LogoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            try
            {
                Package pkg = value as Package;

                if (pkg?.InstalledLocation != null)
                {
                    var pngstandard =
                            Directory.GetFiles(pkg.InstalledLocation.Path, "*.png", SearchOption.AllDirectories)
                                .Where(
                                    s =>
                                        s.IndexOf("44x44", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        s.IndexOf("150x150", StringComparison.OrdinalIgnoreCase) >= 0);
                    var pngStandard = pngstandard as IList<string> ?? pngstandard.ToList();
                    if (pngStandard.Any())
                    {
                        return pngStandard.First();
                    }

                    var pngothers =
                            Directory.GetFiles(pkg.InstalledLocation.Path, "*.png", SearchOption.AllDirectories)
                                .Where(
                                    s =>
                                        s.IndexOf("logo", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                        s.IndexOf("tile", StringComparison.OrdinalIgnoreCase) >= 0);

                    var pngOthers = pngothers as IList<string> ?? pngothers.ToList();
                    if (pngOthers.Any())
                    {
                        return pngOthers.Last();
                    }
                }

                return "";
            }
            catch
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
