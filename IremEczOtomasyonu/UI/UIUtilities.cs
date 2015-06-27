using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace IremEczOtomasyonu.UI
{
    class UIUtilities
    {
        /// <summary>
        /// Opens the decade mode for easy selection of the expiration date.
        /// </summary>
        /// <param name="datePicker"></param>
        public static void DatePickerSelectDecade(DatePicker datePicker)
        {
            if (datePicker == null)
            {
                return;
            }
            var popup = datePicker.Template.FindName("PART_Popup", datePicker) as Popup;
            if (popup != null && popup.Child is Calendar)
            {
                ((Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
            }
        }

        public static bool HasDataGridErrors(DataGrid dataGrid)
        {
            bool hasDataGridErrors = false;
            for (int i = 0; i < dataGrid.Items.Count; i++)
            {
                DataGridRow row = GetRow(dataGrid, i);
                if (row != null && Validation.GetHasError(row))
                {
                    hasDataGridErrors = true;
                    break;
                }
            }
            return hasDataGridErrors;
        }

        private static DataGridRow GetRow(DataGrid dataGrid, int index)
        {
            DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row == null)
            {
                // May be virtualized, bring into view and try again.
                dataGrid.UpdateLayout();
                dataGrid.ScrollIntoView(dataGrid.Items[index]);
                row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(index);
            }
            return row;
        }
    }
}
