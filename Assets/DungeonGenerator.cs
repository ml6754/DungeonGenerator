using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private Sprite _tileprefab;
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
        if (a.a == b.a && a.b == b.b)
        {
            return true;
        }
        if (a.a == b.b && a.b == b.a)
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
    //inits a level with list of all possible paths, and an array representing a 2d array of rooms, given width and height of level
    public static Level levelinit(int w, int h)
    {
        int[,] rooms = new int[h, w];
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
                    pathw.cost = (Random.Range(1, 11));
                    paths.Add(pathw);
                }
                if (i != (h - 1))
                {
                    path pathv = new path();
                    pathv.a = ((j + 1) + ((i) * w));
                    pathv.b = ((j + 1) + ((i + 1) * w));
                    // sets random cost
                    pathv.cost = (Random.Range(1, 11));
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
        Stack<int> stack = new Stack<int>();
        List<int> visited = new List<int>();
        int randroot = 0;
        do
        {
            randroot = (Random.Range(1, l.rooms.Length));
        }
        while (ListContains(l.roomsremoved, randroot));
        stack.Push(randroot);
        while (stack.Count > 0)
        {
            int v = stack.Pop();
            if ((!ListContains(visited, v)))
            {
                visited.Add(v);
                for (int i = 0; i < l.paths.Count; i++)
                {
                    if ((ListContains(visited, l.paths[i].a)) && (!ListContains(l.roomsremoved, l.paths[i].a)))
                    {
                        if ((!ListContains(visited, l.paths[i].b)))
                        {
                            stack.Push(l.paths[i].b);
                        }
                    }
                    if ((ListContains(visited, l.paths[i].b)) && (!ListContains(l.roomsremoved, l.paths[i].b)))
                    {
                        if ((!ListContains(visited, l.paths[i].a)))
                        {
                            stack.Push(l.paths[i].a);
                        }
                    }
                }
            }
        }
        if ((visited.Count) == (l.rooms.Length - l.roomsremoved.Count))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // returns new level with ranomdly unset rooms
    public static Level unsetrooms(Level l)
    {
        int h = l.rooms.GetLength(0);
        int w = l.rooms.GetLength(1);
        // the number of time we want this loop to run 
        int toremove = (h * w) / 3;
        List<path> pathsremoved = new List<path>();
        while (toremove > 0)
        {
            // remove a certain number of rooms and make sure that the level is still connected
            int randroot = 0;
            do
            {
                randroot = (Random.Range(1, l.rooms.Length));
            }
            while (ListContains(l.roomsremoved, randroot));
            l.roomsremoved.Add(randroot);
            // removes all the paths with the root
            for (int i = l.paths.Count - 1; i >= 0; i--)
            {
                if ((l.paths[i].a == randroot) || (l.paths[i].b == randroot))
                {
                    pathsremoved.Add(l.paths[i]);
                    l.paths.RemoveAt(i);
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
            }
            // the level does not stay connected ..
            if (removeable == false)
            {
                l.roomsremoved.Remove(randroot);
                while (pathsremoved.Count != 0)
                {
                    l.paths.Add(pathsremoved[0]);
                    pathsremoved.Remove(pathsremoved[0]);
                }

            }
            toremove--;
        }
        return l;
    }
    // creates a randomly pathed connected level given an init level
    // level must be already initialized and already unset
    public static Level pathLevel(Level l)
    {
        // this is to help decide how many we want to remove from the get go
        int numpaths = l.paths.Count;
        int numrooms = l.rooms.Length - l.roomsremoved.Count;

        // we need to check and make sure that randroot is not one of the roots we removed
        List<int> vertvisited = new List<int>();
        List<path> visited = new List<path>();
        List<path> connected = new List<path>();
        // make sure randroot isnt = 0 lol
        int randroot = 0;
        do
        {
            randroot = (Random.Range(1, l.rooms.Length));
        }
        while (ListContains(l.roomsremoved, randroot));
        while (ListContains(l.roomsremoved, randroot))
        {
            randroot = (Random.Range(1, l.roomsremoved.Count));
        }
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
            if (ListContains(vertvisited, visited.Last().a) != true)
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
        for (i = 0; i < l.paths.Count; i++)
        {
            if (comparepaths(visited, l.paths[i]) == false)
                topath.Add(l.paths[i]);
        }
        //we have created a random MST!
        //add a random amount of extra edges to finish algo!

        int morepaths = Random.Range(0, topath.Count);
        for (int k = 0; k < morepaths; k++)
        {
            int picked = (Random.Range(0, topath.Count));
            visited.Add(topath[picked]);
            topath.Remove(topath[picked]);

        }
        Level ret = new Level();
        ret.paths = visited;
        ret.rooms = l.rooms;
        return ret;
    }
    // Start is called before the first frame update
    void Start()
    {
        Level l = levelinit(5, 5);
        l = unsetrooms(l);
        l = pathLevel(l);
        int h = l.rooms.GetLength(0);
        int w = l.rooms.GetLength(1);
        // list of the actual game objects to place the rooms in
        List<GameObject> rooms = new List<GameObject>();
        // list of prefab rooms to place onto the game objects
        List<GameObject> prefabrooms = new List<GameObject>();
        // making the rooms
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                // set room to 0 
                if (l.rooms[i, j] != 0)
                {
                    // this is just to use rn cause we didnt actually make any maps
                    GameObject room = new GameObject("room_" + ((j + 1) + ((i) * w)));
                    SpriteRenderer renderer = room.AddComponent<SpriteRenderer>();
                    renderer.sortingOrder = +100 + i*j;
                    renderer.sprite = _tileprefab;
                    rooms.Add(room);
                    renderer.color = new Color(1, 0, 0, 1);
                    int sizemodx = Random.Range(1, 3);
                    int sizemody = Random.Range(1, 3);
                    room.transform.localScale = new Vector3(1 + sizemodx, 1 + sizemody, 0);
                    room.transform.Translate(j * 4, -i * 4, 0);

                }
                else
                {
                    rooms.Add(null);
                }
            }
        }
        //making the paths
        for (int i = 0; i < l.paths.Count; i++)
        {
            GameObject path = new GameObject("path_" + (i + 1));
            SpriteRenderer renderer = path.AddComponent<SpriteRenderer>();
            renderer.sprite = _tileprefab;
            renderer.sortingOrder = -i;
            renderer.color = new Color(0,0,1, 1);
            GameObject rooma = rooms[l.paths[i].a - 1];
            GameObject roomb = rooms[l.paths[i].b - 1];
            float ax = rooma.transform.position.x;
            float bx = roomb.transform.position.x;
            float ay = rooma.transform.position.y;
            float by = roomb.transform.position.y;
            path.transform.Translate(((ax + bx) / 2), ((ay + by) / 2), 0);
            // check if vertical path
            if (Mathf.Abs(l.paths[i].a - l.paths[i].b) > 1)
            {
                path.transform.localScale = new Vector3(1, 2, 0);
            }
            // must be horizontal path
            else
            {
                path.transform.localScale = new Vector3(2, 1, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
