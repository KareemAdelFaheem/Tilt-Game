
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

class Program
{

    public static void Main(String[] args)
    {
        List<(int, int)> newsliders = new List<(int, int)>() { };
        List<(int, int)> ssliders = new List<(int, int)>() { };
        // List<(int, int)> state = new List<(int, int)>() { };
        string txt = "C:\\Users\\KareemAdel\\Desktop\\Tilt Game\\Test Cases\\Sample Tests\\Case2.txt";
        string[] lines = File.ReadAllLines(txt);
        string[] linesToProcess = lines.Skip(1).Take(lines.Length - 2).ToArray();
        int row = linesToProcess.Length;
        int col = System.Convert.ToInt32(lines[0]);
        string[] targetstring = lines[lines.Length - 1].Split(',' + " ");
        (int, int) target = (System.Convert.ToInt32(targetstring[0]), System.Convert.ToInt32(targetstring[1]));

        // Console.Write(target);


        string[,] board = new string[row, col];
        for (int i = 0; i < row; i++)
        {
            string[] value = linesToProcess[i].Split("," + " ");

            for (int j = 0; j < col; j++)
            {

                board[i, j] = value[j];



            }
        }



        List<(int, int)> sliders = new List<(int, int)>() { };
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == "o")
                {
                    sliders.Add((j, i));
                }

            }
        }


        //// preparing obstacles in each row and column
        Dictionary<int, List<(int, int)>> obstacles = new Dictionary<int, List<(int, int)>>();
        Dictionary<int, List<(int, int)>> obstaclescol = new Dictionary<int, List<(int, int)>>();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] == "#")
                {
                    if (obstacles.ContainsKey(i))
                    {
                        obstacles[i].Add((j, i));



                    }
                    else
                    {
                        List<(int, int)> obstaclespoints = new List<(int, int)>();
                        obstaclespoints.Add((j, i));

                        obstacles.Add(i, obstaclespoints);
                    }
                    if (obstaclescol.ContainsKey(j))
                    {
                        obstaclescol[j].Add((j, i));



                    }
                    else
                    {
                        List<(int, int)> obstaclespoints = new List<(int, int)>();
                        obstaclespoints.Add((j, i));

                        obstaclescol.Add(j, obstaclespoints);
                    }


                }


            }

        }



        ////// preparing adjacency list 
        Dictionary<(int, int), List<(int, int)>> adjlist = new Dictionary<(int, int), List<(int, int)>>() { };
        (int, int) left;
        (int, int) right;
        (int, int) up;
        (int, int) down;
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                if (board[i, j] != "#")
                {
                    (int, int) horizon = (-1, -1);
                    (int, int) vertical = (-1, -1);

                    (int, int) point = (j, i);
                    if (obstacles.ContainsKey(i))
                    {
                        horizon = getadj(obstacles[i], point, "row");
                    }
                    if (obstaclescol.ContainsKey(j))

                    {

                        vertical = getadj(obstaclescol[j], point, "col");
                    }
                    List<(int, int)> points = new List<(int, int)>();
                    left = (horizon.Item1, point.Item2);
                    right = (horizon.Item2, point.Item2);
                    up = (point.Item1, vertical.Item1);
                    down = (point.Item1, vertical.Item2);

                    if (left.Item1 == -1)
                    {
                        left.Item1 = 0;
                    }
                    if (right.Item1 == -1)
                    {
                        right.Item1 = board.GetLength(1) - 1;
                    }
                    if (up.Item2 == -1)
                    {
                        up.Item2 = 0;

                    }
                    if (down.Item2 == -1)
                    {
                        down.Item2 = board.GetLength(0) - 1;
                    }
                    points.Add(left);
                    points.Add(right);
                    points.Add(up);
                    points.Add(down);

                    adjlist.Add(point, points);

                }



            }


        }//ending adjacency list

        int x = BFS(sliders[0], adjlist);
        Console.WriteLine(x);



        int BFS((int, int) root, Dictionary<(int, int), List<(int, int)>> adjlist)
        {

            ((int, int), int) u;
            int iterations = 0;
            int level = 0;
            Queue<((int, int), int)> qt = new Queue<((int, int), int)>();
            qt.Enqueue((root, 4));

            while (qt.Count != 0)
            {
                // state.Clear();
                u = qt.Dequeue();



                if (u.Item2 == 4)
                {
                    List<(int, int)> state = new List<(int, int)>() { };
                    for (int i = 0; i < 4; i++)
                    {
                        state.Clear();
                        qt.Enqueue((adjlist[u.Item1][i], i));
                        foreach (var slider in sliders)
                        {
                            (int, int) temp = adjlist[slider][i];

                            if (state.Contains(adjlist[slider][i]) && i == 0)
                            {
                                state.Add((temp.Item1 + 1, temp.Item2));

                            }
                            else if (state.Contains(adjlist[slider][i]) && i == 1) { state.Add((temp.Item1 - 1, temp.Item2)); }
                            else if (state.Contains(adjlist[slider][i]) && i == 2) { state.Add((temp.Item1, temp.Item2 + 1)); }
                            else if (state.Contains(adjlist[slider][i]) && i == 3) { state.Add((temp.Item1, temp.Item2 - 1)); }
                            else
                            {

                                state.Add(temp);
                            }
                        }
                        if (state.Contains(target))
                        {
                            qt.Clear();
                            break;
                        }

                    }
                    level++;
                    state.Clear();



                }
                else
                {

                    newsliders.Clear();
                    // for (int j = 0; j < sliders.Count; j++)
                    // {
                    //     // newsliders.Add(adjlist[sliders[j]][u.Item2]);
                    //     (int, int) temp = adjlist[sliders[j]][u.Item2];

                    //     if (newsliders.Contains(adjlist[sliders[j]][u.Item2]) && j == 0)
                    //     {
                    //         newsliders.Add((temp.Item1 + 1, temp.Item2));

                    //     }
                    //     else if (newsliders.Contains(adjlist[sliders[j]][u.Item2]) && j == 1) { newsliders.Add((temp.Item1 - 1, temp.Item2)); }
                    //     else if (newsliders.Contains(adjlist[sliders[j]][u.Item2]) && j == 2) { newsliders.Add((temp.Item1, temp.Item2 + 1)); }
                    //     else if (newsliders.Contains(adjlist[sliders[j]][u.Item2]) && j == 3) { newsliders.Add((temp.Item1, temp.Item2 - 1)); }
                    //     else
                    //     {

                    //         newsliders.Add(temp);
                    //     }
                    // }
                    iterations++;
                    if (iterations == 4) { level++; iterations = 0; }
                    int index = -1;
                    if (u.Item2 == 0 || u.Item2 == 1)
                    {
                        index = 2;
                    }
                    if (u.Item2 == 2 || u.Item2 == 3)
                    {
                        index = 0;
                    }
                    for (int i = index; i < index + 2; i++)
                    {
                        for (int j = 0; j < sliders.Count; j++)
                        {
                            // newsliders.Add(adjlist[sliders[j]][u.Item2]);
                            (int, int) temp = adjlist[sliders[j]][u.Item2];

                            if (newsliders.Contains(adjlist[sliders[j]][u.Item2]) && i == 0)
                            {
                                newsliders.Add((temp.Item1 + 1, temp.Item2));

                            }
                            else if (newsliders.Contains(adjlist[sliders[j]][u.Item2]) && i == 1) { newsliders.Add((temp.Item1 - 1, temp.Item2)); }
                            else if (newsliders.Contains(adjlist[sliders[j]][u.Item2]) && i == 2) { newsliders.Add((temp.Item1, temp.Item2 + 1)); }
                            else if (newsliders.Contains(adjlist[sliders[j]][u.Item2]) && i == 3) { newsliders.Add((temp.Item1, temp.Item2 - 1)); }
                            else
                            {

                                newsliders.Add(temp);
                            }
                        }
                    }
                    for (int i = index; i < index + 2; i++)
                    {
                        // state.Clear();
                        qt.Enqueue((adjlist[u.Item1][i], i));
                        List<(int, int)> newslider = new List<(int, int)>() { };
                        // for (int j = 0; j < sliders.Count; j++)
                        // {
                        //     newsliders.Add(adjlist[sliders[j]][i]);
                        // }
                        for (int j = 0; j < sliders.Count; j++)
                        {

                            (int, int) temp = adjlist[newsliders[j]][i];

                            if (ssliders.Contains(adjlist[newsliders[j]][i]) && i == 0)
                            {
                                ssliders.Add((temp.Item1 + 1, temp.Item2));

                            }
                            else if (ssliders.Contains(adjlist[newsliders[j]][i]) && i == 1) { ssliders.Add((temp.Item1 - 1, temp.Item2)); }
                            else if (ssliders.Contains(adjlist[newsliders[j]][i]) && i == 2) { ssliders.Add((temp.Item1, temp.Item2 + 1)); }
                            else if (ssliders.Contains(adjlist[newsliders[j]][i]) && i == 3) { ssliders.Add((temp.Item1, temp.Item2 - 1)); }
                            else
                            {

                                ssliders.Add(temp);
                            }


                        }
                        if (ssliders.Contains(target))
                        {
                            qt.Clear();
                            break;
                        }
                        ssliders.Clear();

                    }

                }

            }


            return level;
        }
        /// //////////////////////////////////////end of BFS***************************************************************************************
        /////////////////////////////////////////end of BFS***************************************************************************************
        /////////////////////////////////////////end of BFS***************************************************************************************
        /////////////////////////////////////////end of BFS***************************************************************************************
        ///
        foreach (var item in ssliders)
        {
            Console.WriteLine(item);
        }


        (int, int) getadj(List<(int, int)> obstacles, (int, int) point, string aspect)
        {
            var begin = -1;
            var end = -1;
            foreach (var item in obstacles)
            {
                if (aspect == "row")
                {
                    if (item.Item1 < point.Item1)
                    {
                        begin = item.Item1 + 1;

                    }
                    if (item.Item1 > point.Item1)
                    {
                        end = item.Item1 - 1;
                        break;
                    }
                }
                else
                {
                    if (item.Item2 < point.Item2)
                    {
                        begin = item.Item2 + 1;

                    }
                    if (item.Item2 > point.Item2)
                    {
                        end = item.Item2 - 1;
                        break;
                    }
                }
            }
            return (begin, end);
        }

    }
}


// foreach (var keyValuePair in obstacles)
// {
//     Console.WriteLine($"Key: {keyValuePair.Key}");
//     foreach (var value in keyValuePair.Value)
//     {
//         Console.WriteLine($"Value: {value}");
//     }
//     Console.WriteLine(); // Separate each key-value pair for clarity
// }


// foreach (var item in sliders)
// {
//     Console.WriteLine(item);
// }

// Console.WriteLine("\n");

// foreach (var keyValuePair in obstaclescol)
// {
//     Console.WriteLine($"Key: {keyValuePair.Key}");
//     foreach (var value in keyValuePair.Value)
//     {
//         Console.WriteLine($"Value: {value}");
//     }
//     Console.WriteLine(); // Separate each key-value pair for clarity
// }


// foreach (var keyValuePair in adjlist)
// {
//     Console.WriteLine($"Key: {keyValuePair.Key}");
//     foreach (var value in keyValuePair.Value)
//     {
//         Console.WriteLine($"    points : {value}");
//     }

// }


// for (int i = 0; i < board.GetLength(0); i++)
// {
//     for (int j = 0; j < board.GetLength(1); j++)
//         Console.Write(String.Format("{0}\t", board[i, j]));
//     Console.WriteLine();
// }

// Console.WriteLine(board.GetLength(0));
// Console.WriteLine(board.GetLength(1));



// foreach (var node in adjlist[u.Item1])
// {
//     qt.Enqueue((node, Array.IndexOf(adjlist[u.Item1].ToArray(), node)));
//     Console.WriteLine(node);
//     if (node == target)
//     {
//         qt.Clear();
//         break;
//     }
// }



// if (adjlist[u.Item1][i] == target)
// {
//     qt.Clear();
//     break;
// }


// newstate=new List<(int, int)>(state);

//                         foreach(var slider in state)
//                         {

//                             // (int,int) temp=adjlist[u.Item1][i];
//                             (int,int) temp=adjlist[slider][i];
//                             if(state.Contains(adjlist[u.Item1][i])&&i==0){newstate.Add((temp.Item1+1,temp.Item2));}
//                             else if(state.Contains(adjlist[slider][i])&&i==1){newstate.Add((temp.Item1-1,temp.Item2));}
//                             else if(state.Contains(adjlist[slider][i])&&i==2){newstate.Add((temp.Item1,temp.Item2+1));}
//                             else if(state.Contains(adjlist[slider][i])&&i==3){newstate.Add((temp.Item1,temp.Item2-1));}
//                             else{
//                                     newstate.Add(temp);
//                             }
//                         }


// if (state.Contains(target))
// {
//     qt.Clear();
//     break;
// }