using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindThePath
{
    public struct ObjectPoint
    {
        public int X, Y;
        public ObjectType Obj;

        public ObjectPoint(int x = 0, int y = 0, ObjectType obj = ObjectType.None)
        {
            X = x;
            Y = y;
            Obj = obj;
        }

        public override string ToString()
        {
            return string.Format("{0},{1} (Type: {2})", X, Y, Obj.ToString());
        }
    }
}
