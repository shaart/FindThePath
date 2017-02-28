using Microsoft.VisualStudio.TestTools.UnitTesting;
using FindThePath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindThePath.Tests
{
    [TestClass()]
    public class WaveAlgLeeTests
    {
        [TestMethod()]
        [TestCategory("WaveAlgLee MakeWay")]
        [ExpectedException(typeof(ArgumentException), "Incorrect 'startPoint' argument: object type must be 'StartPoint'")]
        public void MakeWayTest_ExceptionGeneration_IncorrectStartPointArgument()
        {
            string[,] area = new string[3, 3] {
                // 0   1   2
                { "a", "", "" },    // 0
                { "", "", "" },     // 1
                { "", "", "b" }     // 2
            };
            var incorrectPoint = new ObjectPoint(0, 0, ObjectType.Block);
            var endPoint = new ObjectPoint(2, 2, ObjectType.EndPoint);
            WaveAlgLee.MakeWay(ref area, incorrectPoint, endPoint);
        }

        [TestMethod()]
        [TestCategory("WaveAlgLee MakeWay")]
        [ExpectedException(typeof(ArgumentException), "Incorrect 'endPoint' argument: object type must be 'EndPoint'")]
        public void MakeWayTest_ExceptionGeneration_IncorrectEndPointArgument()
        {
            string[,] area = new string[3, 3] {
                // 0   1   2
                { "a", "", "" },    // 0
                { "", "", "" },     // 1
                { "", "", "b" }     // 2
            };
            var startPoint = new ObjectPoint(0, 0, ObjectType.StartPoint);
            var incorrectPoint = new ObjectPoint(2, 2, ObjectType.Block);
            WaveAlgLee.MakeWay(ref area, startPoint, incorrectPoint);
        }

        [TestMethod()]
        [TestCategory("WaveAlgLee MakeWay")]
        public void MakeWayTest_Common_usage()
        {
            string[,] area = new string[3, 3] {
                // 0   1   2
                { "a", "", "" },     // 0
                { "", "x", "" },     // 1
                { "", "x", "b" }     // 2
            };

            var startPoint = new ObjectPoint(0, 0, ObjectType.StartPoint);
            var endPoint = new ObjectPoint(2, 2, ObjectType.EndPoint);

            List<ObjectPoint> expectedResult = new List<ObjectPoint>()
            {
                new ObjectPoint{ X = 0, Y = 1, Obj = ObjectType.None },
                new ObjectPoint{ X = 0, Y = 2, Obj = ObjectType.None },
                new ObjectPoint{ X = 1, Y = 2, Obj = ObjectType.None },
                new ObjectPoint{ X = 2, Y = 2, Obj = ObjectType.EndPoint }
            };

            var result = WaveAlgLee.MakeWay(ref area, startPoint, endPoint);

            if (result.Count == 0)
            {
                Assert.Fail("Result path list count 0 elements");
            }
            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], result[i]);
            }
        }

        [TestMethod()]
        [TestCategory("WaveAlgLee ConvertToWaveObjectsArray")]
        public void ConvertToWaveObjectsArrayTest()
        {
            string[,] array = new string[3, 3] {
                // 0   1   2
                { "a", "", "" },     // 0
                { "", "x", "" },     // 1
                { "", "x", "b" }     // 2
            };

            WaveObject[,] expectedResult = new WaveObject[3, 3]
                {
                    { new WaveObject(value: 0, type: ObjectType.StartPoint),        // -- 0,0
                        new WaveObject(value: null, type: ObjectType.None),         // 1,0
                        new WaveObject(value: null, type: ObjectType.None) },       // 2,0
                    { new WaveObject(value: null, type: ObjectType.None),           // -- 0,1
                        new WaveObject(value: null, type: ObjectType.Block),        // 1,1
                        new WaveObject(value: null, type: ObjectType.None) },       // 2,1
                    { new WaveObject(value: null, type: ObjectType.None),           // -- 0,2
                        new WaveObject(value: null, type: ObjectType.Block),        // 1,2
                        new WaveObject(value: null, type: ObjectType.EndPoint) }    // 2,2
                };

            var result = WaveAlgLee.ConvertToWaveObjectsArray(array);
            for (int j = 0; j < (result.Rank + 1); j++)
            {
                for (int i = 0; i < result.Length / (result.Rank + 1); i++)
                {
                    Assert.AreEqual(expectedResult[i, j], result[i, j]);
                }
            }
        }

        [TestMethod()]
        [TestCategory("WaveAlgLee WavePropagation")]
        public void WavePropagation_WxH_5x10()
        {
            //  0   1   2   3   4   5   6   7   8   9   |
            //------------------------------------------+---
            //  a   '   '   x   '   '   '   '   '   '   | 0
            //  '   x   '   '   x   x   '   '   x   '   | 1
            //  '   x   x   x   '   '   '   x   x   '   | 2
            //  '   '   '   '   '   '   '   x   '   '   | 3
            //  '   x   '   x   '   '   x   '   b   x   | 4
            //------------------------------------------+---
            //  0   1   2   3   4   5   6   7   8   9   |
            WaveObject[,] area = new WaveObject[5, 10]
                {
                    { new WaveObject(value: 0, type: ObjectType.StartPoint),        // -- 0,0
                        new WaveObject(value: null, type: ObjectType.None),         //    0,1
                        new WaveObject(value: null, type: ObjectType.None),         //    0,2
                        new WaveObject(value: null, type: ObjectType.Block),        //    0,3
                        new WaveObject(value: null, type: ObjectType.None),         //    0,4
                        new WaveObject(value: null, type: ObjectType.None),         //    0,5
                        new WaveObject(value: null, type: ObjectType.None),         //    0,6
                        new WaveObject(value: null, type: ObjectType.None),         //    0,7
                        new WaveObject(value: null, type: ObjectType.None),         //    0,8
                        new WaveObject(value: null, type: ObjectType.None) },       //    0,9
                    { new WaveObject(value: null, type: ObjectType.None),           // -- 1,0
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,1
                        new WaveObject(value: null, type: ObjectType.None),         //    1,2
                        new WaveObject(value: null, type: ObjectType.None),         //    1,3
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,4
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,5
                        new WaveObject(value: null, type: ObjectType.None),         //    1,6
                        new WaveObject(value: null, type: ObjectType.None),         //    1,7
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,8
                        new WaveObject(value: null, type: ObjectType.None) },       //    1,9
                    { new WaveObject(value: null, type: ObjectType.None),           // -- 2,0
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,1
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,2
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,3
                        new WaveObject(value: null, type: ObjectType.None),         //    2,4
                        new WaveObject(value: null, type: ObjectType.None),         //    2,5
                        new WaveObject(value: null, type: ObjectType.None),         //    2,6
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,7
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,8
                        new WaveObject(value: null, type: ObjectType.None) },       //    2,9
                    { new WaveObject(value: null, type: ObjectType.None),           // -- 3,0
                        new WaveObject(value: null, type: ObjectType.None),         //    3,1
                        new WaveObject(value: null, type: ObjectType.None),         //    3,2
                        new WaveObject(value: null, type: ObjectType.None),         //    3,3
                        new WaveObject(value: null, type: ObjectType.None),         //    3,4
                        new WaveObject(value: null, type: ObjectType.None),         //    3,5
                        new WaveObject(value: null, type: ObjectType.None),         //    3,6
                        new WaveObject(value: null, type: ObjectType.Block),        //    3,7
                        new WaveObject(value: null, type: ObjectType.None),         //    3,8
                        new WaveObject(value: null, type: ObjectType.None) },       //    3,9
                    { new WaveObject(value: null, type: ObjectType.None),           // -- 4,0
                        new WaveObject(value: null, type: ObjectType.Block),        //    4,1
                        new WaveObject(value: null, type: ObjectType.None),         //    4,2
                        new WaveObject(value: null, type: ObjectType.Block),        //    4,3
                        new WaveObject(value: null, type: ObjectType.None),         //    4,4
                        new WaveObject(value: null, type: ObjectType.None),         //    4,5
                        new WaveObject(value: null, type: ObjectType.Block),        //    4,6
                        new WaveObject(value: null, type: ObjectType.None),         //    4,7
                        new WaveObject(value: null, type: ObjectType.EndPoint),     //    4,8
                        new WaveObject(value: null, type: ObjectType.Block) }       //    4,9
                };
            //  0   1   2   3   4   5   6   7   8   9   |
            //------------------------------------------+---
            //  a   '   '   x   '   '   '   '   '   '   | 0
            //  '   x   '   '   x   x   '   '   x   '   | 1
            //  '   x   x   x   '   '   '   x   x   '   | 2
            //  '   '   '   '   '   '   '   x   '   '   | 3
            //  '   x   '   x   '   '   x   '   b   x   | 4
            //------------------------------------------+---
            //  0   1   2   3   4   5   6   7   8   9   |

            var startPoint = new ObjectPoint(0, 0, ObjectType.StartPoint);
            var endPoint = new ObjectPoint(4, 8, ObjectType.EndPoint);

            WaveAlgLee.WavePropagation(ref area, startPoint, endPoint, true);

            //  0   1   2   3   4   5   6   7   8   9   |
            //------------------------------------------+---
            //  a0  1   2   x   14  13  12  13  14  15  | 0
            //  1   x   3   4   x   x   11  12  x   16  | 1
            //  2   x   x   x   8   9   10  x   x   17  | 2
            //  3   4   5   6   7   8   9   x   19  18  | 3
            //  4   x   6   x   8   9   x   '   b20 x   | 4
            //------------------------------------------+---
            //  0   1   2   3   4   5   6   7   8   9   |
            WaveObject[,] expectedArea = new WaveObject[5, 10]
                {
                    { new WaveObject(value: 0, type: ObjectType.StartPoint),        // -- 0,0
                        new WaveObject(value: 1, type: ObjectType.None),            //    0,1
                        new WaveObject(value: 2, type: ObjectType.None),            //    0,2
                        new WaveObject(value: null, type: ObjectType.Block),        //    0,3
                        new WaveObject(value: 14, type: ObjectType.None),           //    0,4
                        new WaveObject(value: 13, type: ObjectType.None),           //    0,5
                        new WaveObject(value: 12, type: ObjectType.None),           //    0,6
                        new WaveObject(value: 13, type: ObjectType.None),           //    0,7
                        new WaveObject(value: 14, type: ObjectType.None),           //    0,8
                        new WaveObject(value: 15, type: ObjectType.None) },         //    0,9
                    { new WaveObject(value: 1, type: ObjectType.None),              // -- 1,0
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,1
                        new WaveObject(value: 3, type: ObjectType.None),            //    1,2
                        new WaveObject(value: 4, type: ObjectType.None),            //    1,3
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,4
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,5
                        new WaveObject(value: 11, type: ObjectType.None),           //    1,6
                        new WaveObject(value: 12, type: ObjectType.None),           //    1,7
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,8
                        new WaveObject(value: 16, type: ObjectType.None) },         //    1,9
                    { new WaveObject(value: 2, type: ObjectType.None),              // -- 2,0
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,1
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,2
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,3
                        new WaveObject(value: 8, type: ObjectType.None),            //    2,4
                        new WaveObject(value: 9, type: ObjectType.None),            //    2,5
                        new WaveObject(value: 10, type: ObjectType.None),           //    2,6
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,7
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,8
                        new WaveObject(value: 17, type: ObjectType.None) },         //    2,9
                    { new WaveObject(value: 3, type: ObjectType.None),              // -- 3,0
                        new WaveObject(value: 4, type: ObjectType.None),            //    3,1
                        new WaveObject(value: 5, type: ObjectType.None),            //    3,2
                        new WaveObject(value: 6, type: ObjectType.None),            //    3,3
                        new WaveObject(value: 7, type: ObjectType.None),            //    3,4
                        new WaveObject(value: 8, type: ObjectType.None),            //    3,5
                        new WaveObject(value: 9, type: ObjectType.None),            //    3,6
                        new WaveObject(value: null, type: ObjectType.Block),        //    3,7
                        new WaveObject(value: 19, type: ObjectType.None),           //    3,8
                        new WaveObject(value: 18, type: ObjectType.None) },         //    3,9
                    { new WaveObject(value: 4, type: ObjectType.None),              // -- 4,0
                        new WaveObject(value: null, type: ObjectType.Block),        //    4,1
                        new WaveObject(value: 6, type: ObjectType.None),            //    4,2
                        new WaveObject(value: null, type: ObjectType.Block),        //    4,3
                        new WaveObject(value: 8, type: ObjectType.None),            //    4,4
                        new WaveObject(value: 9, type: ObjectType.None),            //    4,5
                        new WaveObject(value: null, type: ObjectType.Block),        //    4,6
                        new WaveObject(value: null, type: ObjectType.None),         //    4,7
                        new WaveObject(value: 20, type: ObjectType.EndPoint),       //    4,8
                        new WaveObject(value: null, type: ObjectType.Block) }       //    4,9
                };
            //  0   1   2   3   4   5   6   7   8   9   | Y/X
            //------------------------------------------+---
            //  a0  1   2   x   14  13  12  13  14  15  | 0 (X)
            //  1   x   3   4   x   x   11  12  x   16  | 1
            //  2   x   x   x   8   9   10  x   x   17  | 2
            //  3   4   5   6   7   8   9   x   19  18  | 3
            //  4   x   6   x   8   9   x   '   b20 x   | 4
            //------------------------------------------+---
            //  0   1   2   3   4   5   6   7   8   9   | Y/X

            int arrayHeight = area.GetUpperBound(1) + 1;
            if (arrayHeight > 0 && area.Length > 0)
            {
                int arrayWidth = area.Length / arrayHeight;
                for (int x = 0; x < arrayWidth; x++)
                {
                    for (int y = 0; y < arrayHeight; y++)
                    {
                        Assert.AreEqual(expectedArea[x, y], area[x, y]);
                    }
                }
            }
            else
            {
                Assert.Fail("Array length or height was 0");
            }
        }

        [TestMethod()]
        [TestCategory("WaveAlgLee WavePropagation")]
        public void WavePropagation()
        {
            var startPoint = new ObjectPoint(0, 0, ObjectType.StartPoint);
            var endPoint = new ObjectPoint(2, 2, ObjectType.EndPoint);
            //  0   1   2   3   |
            //------------------+---
            //  a   '   '   x   | 0
            //  '   x   '   '   | 1
            //  '   x   b   x   | 2
            WaveObject[,] area = new WaveObject[3, 4]
                {
                    { new WaveObject(value: 0, type: ObjectType.StartPoint),        // -- 0,0
                        new WaveObject(value: null, type: ObjectType.None),         //    0,1
                        new WaveObject(value: null, type: ObjectType.None),         //    0,2
                        new WaveObject(value: null, type: ObjectType.Block) },      //    0,3
                    { new WaveObject(value: null, type: ObjectType.None),           // -- 1,0
                        new WaveObject(value: null, type: ObjectType.Block),        //    1,1
                        new WaveObject(value: null, type: ObjectType.None),         //    1,2
                        new WaveObject(value: null, type: ObjectType.None) },       //    1,3
                    { new WaveObject(value: null, type: ObjectType.None),           // -- 2,0
                        new WaveObject(value: null, type: ObjectType.Block),        //    2,1
                        new WaveObject(value: null, type: ObjectType.EndPoint),     //    2,2
                        new WaveObject(value: null, type: ObjectType.Block) }       //    2,3
                };

            WaveAlgLee.WavePropagation(ref area, startPoint, endPoint, true);

            //   0      1     2      3      |
            //------------------------------+---
            //  (a)0  (-)1   (-)2   (x)'    | 0
            //  (-)1  (x)'   (-)3   (-)4    | 1
            //  (-)2  (x)'   (b)4   (x)'    | 2
            WaveObject[,] expectedArea = new WaveObject[3, 4]
                {
                    { new WaveObject(value: 0, type: ObjectType.StartPoint),    // -- 0,0
                        new WaveObject(value: 1, type: ObjectType.None),        //    0,1
                        new WaveObject(value: 2, type: ObjectType.None),        //    0,2
                        new WaveObject(value: null, type: ObjectType.Block) },  //    0,3
                    { new WaveObject(value: 1, type: ObjectType.None),          // -- 1,0
                        new WaveObject(value: null, type: ObjectType.Block),    //    1,1
                        new WaveObject(value: 3, type: ObjectType.None),        //    1,2
                        new WaveObject(value: 4, type: ObjectType.None) },      //    1,3
                    { new WaveObject(value: 2, type: ObjectType.None),          // -- 2,0
                        new WaveObject(value: null, type: ObjectType.Block),    //    2,1
                        new WaveObject(value: 4, type: ObjectType.EndPoint),    //    2,2
                        new WaveObject(value: null, type: ObjectType.Block) }   //    2,3
                };

            int arrayHeight = area.GetUpperBound(1) + 1;
            if (arrayHeight > 0 && area.Length > 0)
            {
                int arrayWidth = area.Length / arrayHeight;
                for (int x = 0; x < arrayWidth; x++)
                {
                    for (int y = 0; y < arrayHeight; y++)
                    {
                        Assert.AreEqual(expectedArea[x, y], area[x, y]);
                    }
                }
            }
            else
            {
                Assert.Fail("Array length or height was 0");
            }
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        [TestCategory("WaveAlgLee ConvertToWaveObjectsArray")]
        public void ConvertToWaveObjectsArray_BlankArray()
        {
            string[,] arr = new string[,] { };
            WaveAlgLee.ConvertToWaveObjectsArray(arr);
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        [TestCategory("WaveAlgLee ConvertToWaveObjectsArray")]
        public void ConvertToWaveObjectsArray_HalfBlankArray()
        {
            string[,] arr = new string[,] { { } };
            WaveAlgLee.ConvertToWaveObjectsArray(arr);
        }

        [TestMethod()]
        [TestCategory("WaveAlgLee RestoreWay")]
        public void RestoreWayTest()
        {
            var startPoint = new ObjectPoint(0, 0, ObjectType.StartPoint);
            var endPoint = new ObjectPoint(2, 2, ObjectType.EndPoint);
            //   0      1     2      3      |
            //------------------------------+---
            //  (a)0  (-)1   (-)2   (x)'    | 0
            //  (-)1  (x)'   (-)3   (-)4    | 1
            //  (-)2  (x)'   (b)4   (x)'    | 2
            WaveObject[,] area = new WaveObject[3, 4]
                {
                    { new WaveObject(value: 0, type: ObjectType.StartPoint),    // -- 0,0
                        new WaveObject(value: 1, type: ObjectType.None),        //    0,1
                        new WaveObject(value: 2, type: ObjectType.None),        //    0,2
                        new WaveObject(value: null, type: ObjectType.Block) },  //    0,3
                    { new WaveObject(value: 1, type: ObjectType.None),          // -- 1,0
                        new WaveObject(value: null, type: ObjectType.Block),    //    1,1
                        new WaveObject(value: 3, type: ObjectType.None),        //    1,2
                        new WaveObject(value: 4, type: ObjectType.None) },      //    1,3
                    { new WaveObject(value: 2, type: ObjectType.None),          // -- 2,0
                        new WaveObject(value: null, type: ObjectType.Block),    //    2,1
                        new WaveObject(value: 4, type: ObjectType.EndPoint),    //    2,2
                        new WaveObject(value: null, type: ObjectType.Block) }   //    2,3
                };
            // x,y
            // 0,0 -> 0,1 -> 0,2 -> 1,2 -> 2,2
            List<ObjectPoint> expectedResult = new List<ObjectPoint>()
            {
                new ObjectPoint{ X = 0, Y = 1, Obj = ObjectType.None },
                new ObjectPoint{ X = 0, Y = 2, Obj = ObjectType.None },
                new ObjectPoint{ X = 1, Y = 2, Obj = ObjectType.None },
                new ObjectPoint{ X = 2, Y = 2, Obj = ObjectType.EndPoint }
            };

            List<ObjectPoint> path = WaveAlgLee.RestoreWay(area, startPoint, endPoint);

            if (path.Count == 0)
            {
                Assert.Fail("Result path list count 0 elements");
            }
            for (int i = 0; i < path.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], path[i]);
            }
        }
    }
}