using Android.Content;
using Android.Views;
using Veldrid.Tests.Android.Forms.Utilities;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views.View;

[assembly: ExportRenderer(typeof(AutomationTextCell), typeof(AutomationTextCellRenderer))]

namespace Veldrid.Tests.Android.Forms.Utilities
{
    public class AutomationTextCellRenderer : TextCellRenderer
    {
        protected override AView GetCellCore(Cell item, AView convertView, ViewGroup parent, Context context)
        {
            AView view = base.GetCellCore(item, convertView, parent, context);
            view.ContentDescription = Cell.AutomationId;
            return view;
        }
    }
}
