using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] public Sprite _tileprefab;
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
        // @ReynahD change this if u want to unset more/less rooms
        // increase from 3 for less and decrease from 3 for less
        int toremove = (int)((h * w) / 3);
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
        int morepaths = Random.Range(0, topath.Count - 1);
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
    public static List<GameObject> makerooms(Level l, List<GameObject> prefabrooms, int rangemin, int rangemax)
    {
        int h = l.rooms.GetLength(0);
        int w = l.rooms.GetLength(1);
        List<GameObject> rooms = new List<GameObject>();
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                // set room to 0 
                if (l.rooms[i, j] != 0)
                {
                    int roompicked = Random.Range(0, prefabrooms.Count);
                    GameObject prefabroom = Instantiate(prefabrooms[roompicked]);
                    SpriteRenderer renderer = prefabroom.GetComponent<SpriteRenderer>();
                    renderer.sortingOrder = (i + 1) * j;
                    prefabroom.transform.Translate(j * (rangemax + (rangemin / 5)), -i * (rangemax + (rangemin / 5)), 0);
                    rooms.Add(prefabroom);

                }
                else
                {
                    rooms.Add(null);
                }
            }
        }
        //set prefabs as inactive
        for (int i = 0; i < 10; i++)
        {
            prefabrooms[i].active = false;
        }
        return rooms;
    }
    public static void makepaths(Level l, List<GameObject> rooms, List<GameObject> pathprefabs, int rangemin, int rangemax)
    {
        for (int i = 0; i < l.paths.Count; i++)
        {
            // check if vertical path
            if (Mathf.Abs(l.paths[i].a - l.paths[i].b) > 1)
            {
                GameObject pathv = Instantiate(pathprefabs[0]);
                SpriteRenderer rendererv = pathv.GetComponent<SpriteRenderer>();
                rendererv.sortingOrder = -(i + 1);
                GameObject rooma = rooms[l.paths[i].a - 1];
                GameObject roomb = rooms[l.paths[i].b - 1];
                float ax = rooma.transform.position.x;
                float bx = roomb.transform.position.x;
                float ay = rooma.transform.position.y;
                float by = roomb.transform.position.y;
                // set position of path
                pathv.transform.Translate(((ax + bx) / 2), ((ay + by) / 2), 0);
            }
            // check if is horizontal path
            else
            {
                GameObject pathh = Instantiate(pathprefabs[1]);
                SpriteRenderer rendererh = pathh.GetComponent<SpriteRenderer>();
                rendererh.sortingOrder = -(i + 1);
                GameObject rooma = rooms[l.paths[i].a - 1];
                GameObject roomb = rooms[l.paths[i].b - 1];
                float ax = rooma.transform.position.x;
                float bx = roomb.transform.position.x;
                float ay = rooma.transform.position.y;
                float by = roomb.transform.position.y;
                // set position of path
                pathh.transform.Translate(((ax + bx) / 2), ((ay + by) / 2), 0);
            }
        }
        // set prefabs as inactive
        for(int i = 0; i < pathprefabs.Count; i++)
        {
            pathprefabs[i].active = false;
        }
    }
    //finds all the leaf rooms returned as a List of GameObjects 
    public static List<GameObject> leafnodes(Level l, List<GameObject> rooms)
    {
        List<GameObject> ret = new List<GameObject>();
        int count = 0;
        for (int i = 1; i != rooms.Count() + 1; i++)
        {
            count = 0;
                for (int j = 0; j < l.paths.Count; j++)
                {
                    if (l.paths[j].a == i || l.paths[j].b == i)
                    {
                        count++;
                    }
                }
            if (count == 1)
            {
                ret.Add(rooms[i - 1]);
            }
        }
        return ret;
    }
    //replaces obj1 with obj2
    public static void replace(GameObject obj1, GameObject obj2)
    {
        Instantiate(obj2, obj1.transform.position, Quaternion.identity);
        Destroy(obj1);
    }
    // returns the end room
    public static GameObject findEndRoom(List<GameObject> leaves, List<GameObject> prefabend)
    {
        int rand = Random.Range(0, leaves.Count);
        int randend = Random.Range(0, prefabend.Count);
        GameObject end = leaves[rand];
        leaves.Remove(end);
        replace(end, prefabend[randend]);
       
        for (int i = 0; i < prefabend.Count; i++)
        {
            prefabend[i].active = false;
        }
        return prefabend[randend];
    }
    // returns the gameobject that is the startroom
    public static GameObject findStartRoom(List<GameObject> leaves, List<GameObject> prefabspawns)
    {
        //find the index of the room that is the end room
        int rand = Random.Range(0, leaves.Count);
        int randspawn = Random.Range(0, prefabspawns.Count);
        GameObject start = leaves[rand];
        leaves.Remove(start);
        replace(start, prefabspawns[randspawn]);
        
        for (int i = 0; i < prefabspawns.Count; i++)
        {
            prefabspawns[i].active = false;
        }
        return prefabspawns[randspawn];
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
        List<GameObject> prefabspawns = new List<GameObject>();
        List<GameObject> prefabend = new List<GameObject>();
        List<GameObject> prefabrooms = new List<GameObject>();
        // list of prefabpaths
        List<GameObject> pathprefabs = new List<GameObject>();
        // @ReynahD change these values to the minumum size and maximum size of the rooms.
        // so currently lowest is 10x10 room
        // highest is 15x15 room
        int rangemin = 10;
        int rangemax = 15;
        // making prefabrooms
        // @ReynahD replace this list with the prefab rooms u want
        // prefab spawn rooms
        for (int i = 0;i < 10; i++)
        {
            GameObject spawn = new GameObject("spawn");
            SpriteRenderer spawnr = spawn.AddComponent<SpriteRenderer>();
            spawnr.sprite = _tileprefab;
            spawnr.color = new Color(0, 1, 0, 1);
            int sizemodx = Random.Range(rangemin, rangemax + 1);
            int sizemody = Random.Range(rangemin, rangemax + 1);
            spawn.transform.localScale = new Vector3(sizemodx, sizemody, 0);
            prefabspawns.Add(spawn);
        }
        for (int i = 0; i < 10; i++)
        {
            GameObject end = new GameObject("end");
            SpriteRenderer endr = end.AddComponent<SpriteRenderer>();
            endr.sprite = _tileprefab;
            endr.color = new Color(1, 0, 0, 1);
            int sizemodx = Random.Range(rangemin, rangemax + 1);
            int sizemody = Random.Range(rangemin, rangemax + 1);
            end.transform.localScale = new Vector3(sizemodx, sizemody, 0);
            prefabend.Add(end);
        }
        // @ReynahD all u have to do is replace my code by just adding the rooms u created into the list prefabrooms
        for (int i = 0; i < 10; i++)
        {
            GameObject room = new GameObject("prefabroom_" + (i));
            // this is just to use rn cause we didnt actually make any maps
            SpriteRenderer render = room.AddComponent<SpriteRenderer>();
            render.sprite = _tileprefab;
            render.color = new Color(0, 0, 1, 1);
            int sizemodx = Random.Range(rangemin, rangemax + 1);
            int sizemody = Random.Range(rangemin, rangemax + 1);
            room.transform.localScale = new Vector3(sizemodx, sizemody, 0);
            prefabrooms.Add(room);
        }
        // making prefabpaths
        // @ReynahD all u have to do is replace my code by just adding the 2 paths u created into the list prefabpaths
        GameObject prefabpathv = new GameObject("pathw");
            SpriteRenderer rendererv = prefabpathv.AddComponent<SpriteRenderer>();
            rendererv.sprite = _tileprefab;
            pathprefabs.Add(prefabpathv);
            prefabpathv.transform.localScale = new Vector3(rangemin / 2, rangemin * 2, 0);
            GameObject prefabpathh = new GameObject("pathv");
            SpriteRenderer rendererh = prefabpathh.AddComponent<SpriteRenderer>();
            rendererh.sprite = _tileprefab;
            prefabpathh.transform.localScale = new Vector3(rangemin * 2, rangemin / 2, 0);
            pathprefabs.Add(prefabpathh);
        // making the rooms
        rooms = makerooms(l, prefabrooms, rangemin, rangemax);
        //making the paths
        makepaths(l, rooms, pathprefabs, rangemin, rangemax);
        List<GameObject> leaves = leafnodes(l, rooms);
        GameObject endRoom = findEndRoom(leaves, prefabend);
        GameObject startRoom = findStartRoom(leaves,prefabspawns);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
