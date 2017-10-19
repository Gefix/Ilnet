using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILNET.Client.Windows
{
    public delegate void NotifyEvent(object sender);

    public delegate bool RenameEvent(object sender, string newName);

}
