using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    enum TileType
    {
        WALL,
        FLOOR,
        VOID,
        PILLAR
    }

    enum Orientation
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

    struct RoomLayout
    {
        public KeyValuePair<TileType, Orientation>[][] room;
        public bool[] dooramp;
    }

    public class RMapGenerator : MonoBehaviour
    {
        private List<RoomLayout> roomLayouts;
        private List<GameObject> rooms;
        [SerializeField] private GameObject[] tileObjects;

        [Header("Enemies")]
        [SerializeField] private GameObject slime;
        [SerializeField] private GameObject fairy;

        private Dictionary<char, Vector2> waypoints = new Dictionary<char, Vector2>();
        private Vector2 tileSize;

        void Start()
        {
            int seed = 1234;

            Random.InitState(seed);

            tileSize = new Vector2(1, 1);
            roomLayouts = new List<RoomLayout>();
            roomLayouts.Add(roomLayoutFromString(
                "|||  |||#" +
                "|.    .|#" +
                "  1     #" +
                "     2  #" +
                "|.    .|#" +
                "|||  |||#")
            );

            string[] enemyLayouts =
            {
                "S1,2#" +
                "S2,1#",
            };

            RoomLayout rootRoom = roomLayouts[Random.Range(0, roomLayouts.Count)];

            string mapLayout =
                "0#";


            int rx = 0, ry = 7;
            foreach (char c in mapLayout)
            {
                if (c == '#')
                {
                    rx = 0;
                    ry--;
                } else
                {
                    GameObject room = Instantiate(new GameObject(), transform);
                    room.name = "Room " + rx + ":" + ry;

                    GenerateEnemies(enemyLayouts[int.Parse(c.ToString())], rx, ry, room.transform);

                    GenerateMap(roomLayouts[int.Parse(c.ToString())].room, rx, ry, room.transform);
                    rx++;
                }
            }
        }

        private void GenerateMap(KeyValuePair<TileType, Orientation>[][] tileMap, float room_x, float room_y, Transform parent)
        {
            for (int y = 0; y < tileMap.Length; y++)
            {
                for (int x = 0; x < tileMap[y].Length; x++)
                {
                    if (tileMap[y][x].Key != TileType.FLOOR)
                    {
                        GameObject tile = Instantiate(
                            GetGameObject(tileMap[y][x].Key),
                            tilePosition(x, y, room_x, room_y),
                            GetQuaternion(tileMap[y][x].Value),
                            parent);
                        tile.name = "Tile " + tileMap[y][x].Key;
                    }
                }
            }
        }

        private void GenerateEnemies(string enemyLayout, float room_x, float room_y, Transform parent)
        {
            GameObject enemyType = null;
            char idx = ' ';
            List<Transform> path = new List<Transform>();
            Dictionary<char, Transform> instantiated = new Dictionary<char, Transform>();

            foreach (char c in enemyLayout)
            {
                switch (c)
                {
                    case 'S':
                        enemyType = slime;
                        break;
                    case 'F':
                        enemyType = fairy;
                        break;
                    case ',':
                    case '#':
                        Transform waypoint;
                        Vector2 xy = waypoints[idx];
                        if (!instantiated.ContainsKey(idx))
                        {
                            GameObject wp = Instantiate(new GameObject(), tilePosition(xy.x, xy.y, room_x, room_y) + Vector3.up, new Quaternion(), parent);
                            wp.name = "Waypoint " + xy.x + ":" + xy.y;

                            waypoint = wp.transform;
                            instantiated.Add(idx, waypoint);
                        }
                        else
                        {
                            waypoint = instantiated[idx];
                        }
                        path.Add(waypoint);
                        if (c == '#')
                        {
                            GameObject enemy = Instantiate(enemyType, path[0].position, new Quaternion(), parent);
                            enemy.GetComponent<EnemySystem.REnemyAI>().Path = path;
                            enemy.name = enemyType.name;
                            path = new List<Transform>();
                        }
                        break;
                    default:
                        idx = c;
                        break;

                }
            }
        }

        private GameObject GetGameObject(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.WALL:
                    return tileObjects[0];
                case TileType.FLOOR:
                    return tileObjects[1];
                case TileType.VOID:
                    return tileObjects[2];
                case TileType.PILLAR:
                    return tileObjects[3];
                default:
                    return null;
            }
        }

        private Quaternion GetQuaternion(Orientation orientation)
        {
            Quaternion quat = new Quaternion();
            switch (orientation)
            {
                case Orientation.EAST:
                    quat = Quaternion.Euler(0, 90, 0);
                    break;
                case Orientation.SOUTH:
                    quat = Quaternion.Euler(0, 180, 0);
                    break;
                case Orientation.WEST:
                    quat = Quaternion.Euler(0, 270, 0);
                    break;
            }
            return quat;
        }

        private RoomLayout roomLayoutFromString(string map)
        {

            List<TileType[]> lookup_temp = new List<TileType[]>();
            List<TileType> lookup_row = new List<TileType>();

            int ix = 0, iy = 0;
            foreach (char c in map)
            {
                switch (c)
                {
                    case ' ':
                        lookup_row.Add(TileType.FLOOR);
                        break;
                    case '|':
                        lookup_row.Add(TileType.WALL);
                        break;
                    case '.':
                        lookup_row.Add(TileType.VOID);
                        break;
                    case '#':
                        lookup_temp.Add(lookup_row.ToArray());
                        lookup_row = new List<TileType>();
                        ix = -1;
                        iy++;
                        break;
                    default:
                        waypoints.Add(c, new Vector2(ix, iy));
                        lookup_row.Add(TileType.FLOOR);
                        break;
                }
                ix++;
            }

            lookup_temp.Reverse();

            Dictionary<char, Vector2> rwayp = new Dictionary<char, Vector2>();
            foreach (KeyValuePair<char, Vector2> wp in waypoints)
            {
                rwayp[wp.Key] = new Vector2(wp.Value.x, lookup_temp.Count - 1 - wp.Value.y);
            }
            waypoints = rwayp;

            TileType[][] lookup = lookup_temp.ToArray();

            KeyValuePair<TileType, Orientation>[][] o = new KeyValuePair<TileType, Orientation>[lookup.Length][];

            for (int y = 0; y < lookup.Length; y++)
            {
                List<KeyValuePair<TileType, Orientation>> row = new List<KeyValuePair<TileType, Orientation>>();
                for (int x = 0; x < lookup[y].Length; x++)
                {
                    switch (lookup[y][x])
                    {
                        case TileType.WALL:
                            bool cat = false;

                            bool open_n = x != 0;
                            bool wall_n = open_n && (lookup[y][x - 1] == TileType.WALL);
                            bool open_e = y != lookup.Length - 1;
                            bool wall_e = open_e && (lookup[y + 1][x] == TileType.WALL);
                            bool open_s = x != lookup[y].Length - 1;
                            bool wall_s = open_s && (lookup[y][x + 1] == TileType.WALL);
                            bool open_w = y != 0;
                            bool wall_w = open_w && (lookup[y - 1][x] == TileType.WALL);

                            if (lookup[y][x] == TileType.WALL &&!wall_n && !wall_e && !wall_s && !wall_w) {
                                row.Add(new KeyValuePair<TileType, Orientation>(TileType.PILLAR, Orientation.NORTH));
                                cat = true;
                            }

                            if (!cat && (wall_n || wall_s)) {
                                row.Add(new KeyValuePair<TileType, Orientation>(lookup[y][x], Orientation.EAST));
                                cat = true;
                            }
                            if (!cat && (wall_e || wall_w)) {
                                row.Add(new KeyValuePair<TileType, Orientation>(lookup[y][x], Orientation.NORTH));
                                cat = true;
                            }
                            if (!cat) row.Add(new KeyValuePair<TileType, Orientation>(TileType.PILLAR, Orientation.NORTH));
                            break;

                        case TileType.FLOOR:
                            row.Add(new KeyValuePair<TileType, Orientation>(TileType.FLOOR, Orientation.NORTH));
                            break;
                        case TileType.VOID:
                            row.Add(new KeyValuePair<TileType, Orientation>(TileType.VOID, Orientation.NORTH));
                            break;
                    }
                }
                o[y] = row.ToArray();
            }

            RoomLayout layout = new RoomLayout();
            layout.room = o;
            layout.dooramp = new bool[4] { o[5][3].Key == TileType.FLOOR, o[2][7].Key == TileType.FLOOR, o[0][3].Key == TileType.FLOOR, o[2][0].Key == TileType.FLOOR };

            return layout;
        }

        Vector3 tilePosition(float x, float y, float room_x, float room_y)
        {
            return new Vector3(transform.position.x + (x + room_x * 8 + 0.5f) * tileSize.x, transform.position.y, transform.position.z + (y + room_y * 6 + 0.5f) * tileSize.y);
        }
    }
}