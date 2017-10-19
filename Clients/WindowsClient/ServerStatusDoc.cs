using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using System.IO;

using ILNET;
using ILNET.Elements;
using ILNET.Editor;
using System.Drawing.Drawing2D;

namespace ILNET.Client.Windows
{
    public partial class ServerStatusDoc : DockContent, IToolstripInterface
    {
        public ServerStatusDoc()
        {
            InitializeComponent();
        }

        #region IToolstripInterface Members

        public List<ToolStrip> ToolStirps
        {
            get
            {
                List<ToolStrip> list = new List<ToolStrip>();
                list.Add(this.tschildServer);
                return list;
            }
        }

        #endregion
        
        private string m_fileName = "project.xml";

		public string FileName
		{
			get	{	return m_fileName;	}
			set
			{
				if (value != string.Empty)
				{
				}

				//m_fileName = value;
				this.ToolTipText = value;
			}
		}

		protected override string GetPersistString()
		{
            // Add extra information into the persist string for this document
            // so that it is available when deserialized.
			return GetType().ToString() + "," + FileName + "," + Text;
		}

        private void ServerStatusDoc_Load(object sender, EventArgs e)
        {
        }
    }
}