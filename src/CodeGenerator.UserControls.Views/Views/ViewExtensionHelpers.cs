using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator.UserControls.Views
{
    public static class ViewExtensionHelpers
    {
        public static void ClearDataBindings(this System.Windows.Forms.Control control)
        {
            control.DataBindings.Clear();
            foreach (System.Windows.Forms.Control child in control.Controls)
            {
                child.ClearDataBindings();
            }
        }

        /// <summary>
        /// Make sure the control is not overlapped by the label
        /// </summary>
        public static void EnsureLabelVisible(this System.Windows.Forms.Label label, Control control, Action<int>? updateOtherControls = null)
        {
            EnsureLabelVisible(label, control, null, updateOtherControls);
        }

        /// <summary>
        /// Make sure the control is not overlapped by the label (error messagelabel is also moved)
        /// </summary>
        public static void EnsureLabelVisible(this System.Windows.Forms.Label label, Control control, Label? errorMessage = null, Action<int>? updateOtherControls = null)
        {
            label.SizeChanged += (s, e) =>
            {
                if (label.Right > control.Left)
                {
                    var overlap = label.Right - control.Left + 5;
                    control.Width -= overlap;
                    control.Left += overlap;
                    if(errorMessage!=null)
                        errorMessage.Left = control.Left;
                    updateOtherControls?.Invoke(control.Left);
                }

            };
        }
    }
}
