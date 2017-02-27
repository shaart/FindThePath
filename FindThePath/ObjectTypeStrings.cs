using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindThePath
{
    public static class ObjectTypeStrings
    {
        public static Dictionary<string, ObjectType> Dictionary = new Dictionary<string, ObjectType>()
        {
            // No object (None)
            { "", ObjectType.None },
            // StartPoint
            { "a", ObjectType.StartPoint },
            { "A", ObjectType.StartPoint },
            // EndPoint
            { "b", ObjectType.EndPoint},
            { "B", ObjectType.EndPoint},
            // Block
            { "x", ObjectType.Block},
            { "X", ObjectType.Block}
        };

        /// <summary>
        /// Search in static dictionary ObjectType by received key and returns found ObjectType. If not found: returns ObjectType.None
        /// </summary>
        /// <param name="keyString">ObjectType string key</param>
        /// <returns>ObjectType by key</returns>
        public static ObjectType GetFromString(string keyString)
        {
            ObjectType resultType;
            if (!ObjectTypeStrings.Dictionary.TryGetValue(keyString, out resultType))
            {
                //throw new ArgumentException("ObjectType with this key not found");
                resultType = ObjectType.None;
            }
            return resultType;
        }
    }
}
