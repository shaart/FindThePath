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
        public ObjectType Obj;

        public WaveObject(int? value = null, ObjectType obj = ObjectType.None)
        {
            Value = value;
            Obj = obj;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Obj.ToString(), Value);
        }
    }
}
