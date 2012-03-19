using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace IremEczOtomasyonu
{
    class Utilities
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
    }
}
