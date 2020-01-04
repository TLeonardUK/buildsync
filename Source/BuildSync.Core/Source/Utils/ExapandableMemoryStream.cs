using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class ExpandableMemoryStream : MemoryStream
    {
        private static FieldInfo ExpandableField;
        private static FieldInfo ExposableField;

        public ExpandableMemoryStream(byte[] Input)
            : base(Input)
        {
            // This is super sketchy
            if (ExpandableField == null)
            {
                ExpandableField = typeof(MemoryStream).GetField("_expandable", BindingFlags.NonPublic | BindingFlags.Instance);
                ExposableField = typeof(MemoryStream).GetField("_exposable", BindingFlags.NonPublic | BindingFlags.Instance);
            }

            ExpandableField.SetValue(this, true);
            ExposableField.SetValue(this, true);
        }
    }
}
