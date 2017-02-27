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
        public void WavePropagationTest()
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

            for (int x = 0; x < (area.Rank + 1); x++)
            {
                for (int y = 0; y < area.Length / (area.Rank + 1); y++)
                {
                    Assert.AreEqual(expectedArea[x, y], area[x, y]);
                }
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