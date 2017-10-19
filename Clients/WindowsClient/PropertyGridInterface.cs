using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms;

namespace ILNET.Client.Windows
{
    interface IPropertyGridInterface
    {
        event NotifyEvent OnSelectionChanged;
        event NotifyEvent OnDocumentModified;

        Object SelectedObject { get; }
    }
}
