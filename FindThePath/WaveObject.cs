using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindThePath
{
    public struct WaveObject
    {
        public int? Value;
        public ObjectType Type;

        public WaveObject(int? value = null, ObjectType type = ObjectType.None)
        {
            Value = value;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Type.ToString(), Value);
        }
    }
}
