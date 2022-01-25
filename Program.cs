using System;
using System.Collections.Generic;
using System.Linq;
// is no where near optimal in speed, but this should only realy be done when loading in a level, and not during gameplay.
// optimization will occur if playtesting proves the loading too slow
namespace DungeonGenerator
{
    class Dungeon
    {
        public struct path
        {
            public int a;
            public int b;
            public int cost;
        }
        public struct Level
        {
            public List<path> paths;
            public int[,] rooms;
            public List<int> roomsremoved;
        }
        public static void Print2DArray<T>(T[,] matrix)
        {
            Console.Write("ROOMS: " + "\n");
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.Write(matrix[i, j] + "\t");
                }
                Console.WriteLine();
            }
        }
        public static void Printpaths(List<path> p)
        {
            Console.Write("PATHS: ");
            for (int i = 0; i < p.Count; i++)
            {
                Console.Write("(" + "a: " + p[i].a + " b: " + p[i].b + " cost: " + p[i].cost + ")");
            }
        }
        public static List<path> removeallpaths(List<path> paths, int vert)
        {
            for (int i = 0; i < paths.Count; i++)
            {
                if (paths[i].a == vert)
                {
                    paths.Remove(paths[i]);
                }
                if (paths[i].b == vert)
                {
                    paths.Remove(paths[i]);
                }
            }
            return paths;
        }
        public static bool ListContains(List<int> p, int vert)
        {
            for (int i = 0; i < p.Count; i++)
            {
                if (p[i] == vert)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ListContains(List<path> p, int vert)
        {
            for (int i = 0; i < p.Count; i++)
            {
                if (p[i].a == vert)
                {
                    return true;
                }
                if (p[i].b == vert)
                {
                    return true;
                }
            }
            return false;
        }
 
        public static bool comparepaths(path a, path b)
        {
            if(a.a == b.a && a.b == b.b)
            {
                return true;
            }
            if(a.a == b.b && a.b == b.a)
            {
                return true;
            }
            return false;
        }
        public static bool comparepaths(List<path> a, path b)
        {
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].a == b.a && a[i].b == b.b)
                {
                    return true;
                }
                if (a[i].a == b.b && a[i].b == b.a)
                {
                    return true;
                }
            }
            return false;
        }
        public static void printpath(path p)
        {
            Console.Write("(" + "a: " + p.a + " b: " + p.b + " cost: " + p.cost + ")");
        }

        // returns a random int up to a range excluding a list of numbers
        public static int random_except_list(int n, List<int> x)
        {
            Random r = new Random();
            int result = r.Next(1, n - x.Count);

            for (int i = 0; i < x.Count; i++)
            {
                if (result < x[i])
                    return result;
                result++;
            }
            return result;
        }
        //inits a level with list of all possible paths, and an array representing a 2d array of rooms, given width and height of level
        public static Level levelinit(int w, int h)
        {
            int[,] rooms = new int[h, w];
            Random ran = new Random();
            List<path> paths = new List<path>();
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    // create list of potential paths
                    if (j != (w - 1))
                    {
                        path pathw = new path();
                        pathw.a = ((j + 1) + ((i) * w));
                        pathw.b = ((j + 2) + ((i) * w));
                        // sets random cost
                        pathw.cost = ran.Next(1, 11);
                        paths.Add(pathw);
                    }
                    if (i != (h - 1))
                    {
                        path pathv = new path();
                        pathv.a = ((j + 1) + ((i) * w));
                        pathv.b = ((j + 1) + ((i + 1) * w));
                        // sets random cost
                        pathv.cost = ran.Next(1, 11);
                        paths.Add(pathv);
                    }
                    //name all rooms to corresponding number
                    rooms[i, j] = ((j + 1) + ((i) * w));
                }
            }
            Level ret = new Level();
            List<int> roomrems = new List<int>();
            ret.roomsremoved = roomrems;
            ret.paths = paths;
            ret.rooms = rooms;
            return ret;
        }

        // returns true if the level is connected
        // returns false if the level is not connected
        public static bool isconnected(Level l)
        {
            // todo 
            // dfs through level to see if the rooms are connected
            Stack<int> stack = new Stack<int>();
            List<int> visited = new List<int>();
            int randroot = random_except_list(l.rooms.Length, l.roomsremoved);
            stack.Push(randroot);
            while(stack.Count > 0)
            {
                int v = stack.Pop();
                if ((!ListContains(visited, v)) && (v != 0))
                {
                    visited.Add(v);
                    for(int i = 0; i < l.paths.Count; i++)
                    {
                        if(((ListContains(visited,l.paths[i].a)) && (l.paths[i].a != 0)))
                        {
                            if ((!ListContains(visited, l.paths[i].b)) && (l.paths[i].b != 0))
                            {
                                stack.Push(l.paths[i].b);
                            }
                        }
                        if(((ListContains(visited, l.paths[i].b)) && (l.paths[i].b != 0)))
                        {
                            if ((!ListContains(visited, l.paths[i].a)) && (l.paths[i].a != 0))
                            {
                                stack.Push(l.paths[i].a);
                            }
                        }
                    }
                }
            }
            if (visited.Count == (l.rooms.Length - l.roomsremoved.Count + 1))
            {
                return true;
            }
            else {

                return false;
            }
        }
        // returns new level with ranomdly unset rooms
        public static Level unsetrooms(Level l)
        {
            Random rand = new Random();
            int h = l.rooms.GetLength(0);
            int w = l.rooms.GetLength(1);
            // the limit of rooms toremove, we can update this later
            int removelimit =  (h*w)/3;
            // the number of rooms to remove
            int toremove = rand.Next(1, removelimit);
            while (toremove > 0)
            {
                // remove acertain number of rooms and make sure that the level is still connected
                int randroot = random_except_list(l.rooms.Length, l.roomsremoved);
                List<path> pathsremoved = new List<path>();
                l.roomsremoved.Add(randroot);
                // removes all the paths with the root
                for (int i = 0; i < l.paths.Count; i++)
                {
                    // add path if it has the root
                    if ((l.paths[i].a == randroot) || (l.paths[i].b == randroot))
                    {
                        pathsremoved.Add(l.paths[i]);
                        l.paths.Remove(l.paths[i]);
                        // dont know how to remove something by just decrementing without writing more code so we will use this inefficient method
                        i = 0;
                    }
                }
                // check connectivity
                bool removeable = isconnected(l);
                // if the level stays connected...
                if (removeable == true)
                {
                    for (int i = 0; i < h; i++)
                    {
                        for (int j = 0; j < w; j++)
                        {
                            // set room to 0 
                            if (l.rooms[i, j] == randroot)
                            {
                                l.rooms[i, j] = 0;
                            }
                        }
                    }
                    // and remove all the paths from pathsremoved
                    while (pathsremoved.Count != 0)
                    {
                        pathsremoved.RemoveAt(0);
                    }

                    // decrement remove
                    toremove--;
                }
                // the level does not stay connected ..
                if (removeable == false)
                {
                    while (pathsremoved.Count != 0)
                    {
                        l.paths.Add(pathsremoved[0]);
                        l.roomsremoved.Remove(randroot);
                        pathsremoved.RemoveAt(0);
                    }
                }
            }
            return l;
        }
        // creates a randomly pathed connected level given an init level
        // level must be already initialized and already unset
        public static Level pathLevel(Level l)
        {
            // this is to help decide how many we want to remove from the get go
            int numpaths = l.paths.Count;
            int numrooms = l.rooms.Length;
            
            // we need to check and make sure that randroot is not one of the roots we removed
            List<int> vertvisited = new List<int>();
            List<path> visited = new List<path>();
            List<path> connected = new List<path>();
            // make sure randroot isnt = 0 lol
            int randroot = random_except_list(l.rooms.Length, l.roomsremoved);
            int i = 0;
            int smallest = 100;
            while (vertvisited.Count < numrooms)
            {
                vertvisited.Add(randroot);
                smallest = 100;
                path smallestpath = new path();
                for (i = 0; i < l.paths.Count; i++)
                {
                    // add path if it has the root
                    if ((l.paths[i].a == randroot) || (l.paths[i].b == randroot))
                    {
                        connected.Add(l.paths[i]);
                    }
                }
                //process lowest edge not already in list
                for (i = 0; i < connected.Count; i++)
                {
                    // we want to check to make sure path does not contain 2 vertices that have already been vistied
                    if (!((ListContains(vertvisited, connected[i].a) == true) && (ListContains(vertvisited, connected[i].b) == true)))
                    {
                        if (connected[i].cost < smallest)
                        {
                            smallest = connected[i].cost;
                            smallestpath = connected[i];
                        }
                    }

                }
                //remove from connected list and add to visited list
                for (i = 0; i < connected.Count; i++)
                {
                    if (comparepaths(smallestpath, connected[i]))
                    {
                        visited.Add(connected[i]);
                        connected.Remove(connected[i]);
                    }
                }
                // we need it to check the last entry added into visited, and make it pick the entry that does not exist in the list vertvisited
                if (ListContains(vertvisited,visited.Last().a) != true)
                {
                    randroot = visited.Last().a;
                }
                else
                {
                    randroot = visited.Last().b;
                }
            }
            // copy visited into new path
            List<path> topath = new List<path>();   
            for(i=0; i < l.paths.Count;i++)
            {
                if (comparepaths(visited, l.paths[i]) == false)
                    topath.Add(l.paths[i]);
            }
            //we have created a random MST!
            //add a random amount of extra edges to finish algo!

            Random rand = new Random();
            int morepaths = rand.Next(0, topath.Count);
            Random moreran = new Random();
            for (int k = 0; k < morepaths; k++)
            {
                int picked = moreran.Next(0, topath.Count);
                visited.Add(topath[picked]);
                topath.Remove(topath[picked]);
                
            }
            Level ret = new Level();
            ret.paths = visited;
            ret.rooms = l.rooms;
            return ret;

           
        }
        public static void Main(string[] args)
        {
            Level l = levelinit(6, 6);
            l = unsetrooms(l);
            l = pathLevel(l);
            Printpaths(l.paths);
            Console.Write("\n");
            Print2DArray<int>(l.rooms);
        }
    }
}