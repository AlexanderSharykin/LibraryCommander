using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels
{
    /// <summary>
    /// Disk-drive descriptor vm
    /// </summary>
    public class PartitionVm
    {
        public string Name { get; set; }
        
        public int Index { get; set; }

        public string Hotkey
        {
            get { return "Control+D" + Index; }
        }
    }
}
