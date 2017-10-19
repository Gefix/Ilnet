using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using vbAccelerator.Components.ListBarControl;

namespace ILNET.Client.Windows
{
    public partial class Toolbox : ToolWindow
    {
        public Toolbox()
        {
            InitializeComponent();

            List<ListBarItem> items = new List<ListBarItem>();

            items.Add(new ListBarItem("Pointer"));
            listBar.Groups.Add(new ListBarGroup("General",items.ToArray()));
            items.Clear();
        }
    }
}