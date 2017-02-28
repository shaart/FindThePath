using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindThePath
{
    public static class WaveAlgLee
    {
        public static WaveObject MakeFromString(string typeKey)
        {
            WaveObject resultObj = new WaveObject();
            resultObj.Type = ObjectTypeStrings.GetFromString(typeKey);
            if (resultObj.Type == ObjectType.StartPoint)
            {
                // Wave alg init
                resultObj.Value = 0;
            }
            return resultObj;
        }

        public static WaveObject[,] ConvertToWaveObjectsArray(string[,] array)
        {
            int arrayHeight = array.GetUpperBound(1) + 1;
            int arrayWidth = array.Length / arrayHeight;
            if (array.Length > 0)
            {
                var waveObjects = new WaveObject[arrayWidth, arrayHeight];
                for (int j = 0; j < arrayHeight; j++)
                {
                    for (int i = 0; i < arrayWidth; i++)
                    {
                        waveObjects[i, j] = MakeFromString(array[i, j]);
                    }
                }
                return waveObjects;
            }
            else
            {
                throw new ArgumentException("Array length = 0");
            }
        }

        public static List<ObjectPoint> MakeWay(ref string[,] area, ObjectPoint startPoint, ObjectPoint endPoint)
        {
            if (startPoint.Obj != ObjectType.StartPoint)
            {
                throw new ArgumentException("Incorrect 'startPoint' argument: object type must be 'StartPoint'");
            }
            if (endPoint.Obj != ObjectType.EndPoint)
            {
                throw new ArgumentException("Incorrect 'endPoint' argument: object type must be 'EndPoint'");
            }

            // Alg init
            WaveObject[,] waveObjects = ConvertToWaveObjectsArray(area);

            // Wave
            WavePropagation(ref waveObjects, startPoint, endPoint);

            // Area W x H
            int areaHeight = area.GetUpperBound(1) + 1,
                areaWidth = area.Length / areaHeight;

            // Transfer data to source array
            for (int y = 0; y < areaHeight; y++)
            {
                for (int x = 0; x < areaWidth; x++)
                {
                    area[x, y] = waveObjects[x, y].Value == null? "" : waveObjects[x, y].Value.ToString();
                    if (waveObjects[x, y].Type == ObjectType.StartPoint)
                    {
                        area[x, y] = area[x, y].Insert(0, "a");
                    }
                    if (waveObjects[x, y].Type == ObjectType.EndPoint)
                    {
                        area[x, y] = area[x, y].Insert(0, "b");
                    }
                    if (waveObjects[x, y].Type == ObjectType.Block)
                    {
                        area[x, y] = area[x, y].Insert(0, "x");
                    }
                }
            }

            // End of alg
            List<ObjectPoint> path = RestoreWay(waveObjects, startPoint, endPoint);

            return path;
        }

        public static List<ObjectPoint> RestoreWay(WaveObject[,] area, ObjectPoint startPoint, ObjectPoint endPoint)
        {
            List<ObjectPoint> path = new List<ObjectPoint>();

            if (area[endPoint.X, endPoint.Y].Value != null)
            {
                int[,] dirXY = new int[,]
                {
                { 0, -1 },      // up
                { 1, 0 },       // right
                { 0, 1 },       // down
                { -1, 0 }       // left
                };
                // Steps to start point
                int step = area[endPoint.X, endPoint.Y].Value.Value;
                // Point coords counters
                int dx = endPoint.X,
                    dy = endPoint.Y;
                // Area W x H
                int areaHeight = area.GetUpperBound(1) + 1,
                    areaWidth = area.Length / areaHeight;
                // Coords of environment point 
                int envX = 0,
                    envY = 0;

                while ((dx != startPoint.X || dy != startPoint.Y) && step > 0)
                {
                    // From finish point
                    path.Add(new ObjectPoint(dx, dy, area[dx, dy].Type));
                    step--;
                    for (int dir = 0; dir < dirXY.Length / 2; dir++)
                    {
                        envX = dx + dirXY[dir, 0];
                        envY = dy + dirXY[dir, 1];
                        if (envX >= 0 && envX < areaWidth &&
                            envY >= 0 && envY < areaHeight &&
                            area[envX, envY].Type != ObjectType.Block &&
                            area[envX, envY].Value == step)
                        {
                            dx = envX;
                            dy = envY;
                            break;
                        }
                    }
                }
                // Reverse result array: must be from start to finish, not vice versa
                path.Reverse();
            }

            return path;
        }

        public static void WavePropagation(ref WaveObject[,] area, ObjectPoint startPoint, ObjectPoint endPoint, bool traceConsole = false)
        {
            int[,] dirXY = new int[,]
            {
                { 0, -1 },      // up
                { 1, 0 },       // right
                { 0, 1 },       // down
                { -1, 0 }       // left
            };
            bool canPropagateWave;
            int step = 0;
            int envX = 0, envY = 0;
            int areaHeight = area.GetUpperBound(1) + 1;
            int areaWidth = area.Length / areaHeight;
            do
            {
                canPropagateWave = false;
                for (int dy = 0; dy < areaHeight; dy++)
                {
                    for (int dx = 0; dx < areaWidth; dx++)
                    {
                        if (area[dx, dy].Value == step)
                        {
                            // 4 directions: up, right, down, left
                            // dirXY - 2-rank array
                            for (int dir = 0; dir < dirXY.Length / 2; dir++)
                            {
                                envX = dx + dirXY[dir, 0];
                                envY = dy + dirXY[dir, 1];
                                if (envX >= 0 && envX < areaWidth &&
                                    envY >= 0 && envY < areaHeight &&
                                    area[envX, envY].Type != ObjectType.Block &&
                                    area[envX, envY].Type != ObjectType.StartPoint &&
                                    area[envX, envY].Value == null)
                                {
                                    canPropagateWave = true;
                                    area[envX, envY].Value = step + 1;
                                }
                            }
                        }
                    }
                }
                step++;
                #region If TRACE - out
#if TRACE
                if (traceConsole)
                {
                    // Console out
                    System.Diagnostics.Trace.WriteLine("-----------\nStep: " + step.ToString() + '\n');
                    for (int dx = 0; dx < areaWidth; dx++)
                    {
                        for (int dy = 0; dy < areaHeight; dy++)
                        {
                            string str = "";
                            switch (area[dx, dy].Type)
                            {
                                case ObjectType.Block:
                                    str = "(x)";
                                    break;
                                case ObjectType.None:
                                    str = "(-)" + (area[dx, dy].Value == null ? "\"" : area[dx, dy].Value.ToString());
                                    break;
                                case ObjectType.StartPoint:
                                    str = "(a)" + (area[dx, dy].Value == null ? "\"" : area[dx, dy].Value.ToString());
                                    break;
                                case ObjectType.EndPoint:
                                    str = "(b)" + (area[dx, dy].Value == null ? "\"" : area[dx, dy].Value.ToString());
                                    break;
                            }
                            System.Diagnostics.Trace.Write(str);
                            string space = " ";
                            for (int c = 0; c <= 5 - str.Length; c++)
                            {
                                space += " ";
                            }
                            System.Diagnostics.Trace.Write(space);
                        }
                        System.Diagnostics.Trace.Write("\n");
                    }
                }
#endif
                #endregion
            }
            while (area[endPoint.X, endPoint.Y].Value == null && canPropagateWave);
        }
    }
}
