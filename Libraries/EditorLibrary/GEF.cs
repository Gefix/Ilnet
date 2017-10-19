using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ILNET;
using ILNET.Elements;
using System.Drawing;
using System.Globalization;

namespace ILNET.Editor
{
    public class GEF
    {
        public static CultureInfo CultureInfo = new System.Globalization.CultureInfo("en-GB");

        Project m_Project;
        
        public GEF(Project project)
        {
            m_Project = project;
        }
    }
}
