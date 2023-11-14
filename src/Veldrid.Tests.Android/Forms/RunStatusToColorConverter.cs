using System;
using System.Globalization;
using Veldrid.Tests.Android.ViewModels;
using Xamarin.Forms;

namespace Veldrid.Tests.Android.Forms
{
    class RunStatusToColorConverter : IValueConverter
    {
        internal static readonly Color NoTestColor = Color.FromHex("#ff7f00");
        internal static readonly Color SkippedColor = Color.FromHex("#ff7700");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RunStatus status)
            {
                return status switch
                {
                    RunStatus.Ok => Color.Green,
                    RunStatus.Failed => Color.Red,
                    RunStatus.NoTests => NoTestColor,
                    RunStatus.Skipped => SkippedColor,
                    RunStatus.NotRun => (object)Color.DarkGray,
                    _ => throw new ArgumentOutOfRangeException(nameof(value)),
                };
            }
            return Color.Red;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
