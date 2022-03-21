namespace Fanex.NCB.Main.ViewModel.TreeSelect
{
    using System.Collections.Generic;

    public class TreeNodeItem
    {
        public string Value { get; set; }

        public string Title { get; set; }

        public int RealValue { get; set; }

        public IEnumerable<TreeNodeItem> Children { get; set; }
    }
}