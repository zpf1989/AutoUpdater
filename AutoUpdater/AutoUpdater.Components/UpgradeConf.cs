using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdater.Components
{
    public class UpgradeConf
    {
        public string path { get; set; }
        public ItemType type { get; set; }
        public OptType opt { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", opt, type, path);
        }
    }

    public enum OptType
    {
        add,
        update,
        delete
    }

    public enum ItemType
    {
        file,
        folder
    }
}
