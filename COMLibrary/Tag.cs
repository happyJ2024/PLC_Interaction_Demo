using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KepwareClient
{
    public class Tag
    {
        public string TagName;
        public string TagDataType;

        public Tag(string name, string type)
        {
            TagName = name;
            TagDataType = type;
        }
    }
}
