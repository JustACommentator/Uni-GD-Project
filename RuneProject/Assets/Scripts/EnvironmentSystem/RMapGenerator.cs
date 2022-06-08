using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    enum TileType
    {
        VOID,
        FLOOR,
        WALL,
        STAIRS,
        TOURCH,
        STATUE,
        SOCKET,
        RAMP,
        POT,
        BOX,
        MOVEABLE_STONE,
        CHEST_WOOD,
        CHEST_GOLD,
        LOCKED_DOOR,
        KEY,
        SPIKES,
        SWITCH,
        BOSS_GATE,
        DOOR
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
        public bool[] doormap;
        public Dictionary<char, Vector2> waypoints;
        public string enemyLayout;
    }

    public class RMapGenerator : MonoBehaviour
    {
        private List<string> roomLayouts;
        private List<GameObject> rooms;
        [SerializeField] private int seed = 69420;
        [SerializeField] private float roomProb = 0.5f;
        [SerializeField] private bool loadOnStart = false;

        [Header("Tiles")]
        [SerializeField] private GameObject[] tileObjects;
        [Header("Enemies")]
        [SerializeField] private GameObject[] enemies;

        private Vector2 tileSize = Vector2.one;

        public int Seed { get => seed; set { seed = value; Create();  } }

        void Start()
        {
            if (loadOnStart)
                Create();
        }

        public void Create()
        {
            tileSize = Vector2.one;
            roomLayouts = AddRooms();
            LoadMap();
        }

        public void Delete()
        {
            if (Application.isEditor)
            {
                List<Transform> ts = new List<Transform>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    ts.Add(transform.GetChild(i));
                }

                foreach(Transform t in ts)
                {
                    DestroyImmediate(t.gameObject);
                }
            }
            else
            {
                List<Transform> ts = new List<Transform>();
                for (int i = 0; i < transform.childCount; i++)
                {
                    ts.Add(transform.GetChild(i));
                }

                foreach (Transform t in ts)
                {
                    Destroy(t.gameObject);
                }
            }
        }

        private void LoadMap()
        {
            Random.InitState(seed);

            bool[][] bitmap = new bool[8][];

            for (int y = 0; y < bitmap.Length; y++)
            {
                bitmap[y] = new bool[8] {
                    Random.Range(0f, 1f) < roomProb,
                    Random.Range(0f, 1f) < roomProb,
                    Random.Range(0f, 1f) < roomProb,
                    Random.Range(0f, 1f) < roomProb,
                    Random.Range(0f, 1f) < roomProb,
                    Random.Range(0f, 1f) < roomProb,
                    Random.Range(0f, 1f) < roomProb,
                    Random.Range(0f, 1f) < roomProb
                };
            }

            string mapLayout = "";

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    bool open_w = x != 0;
                    bool room_w = open_w && bitmap[y][x - 1];
                    bool open_s = y != 7;
                    bool room_s = open_s && bitmap[y + 1][x];
                    bool open_e = x != 7;
                    bool room_e = open_e && bitmap[y][x + 1];
                    bool open_n = y != 0;
                    bool room_n = open_n && bitmap[y - 1][x];

                    if (bitmap[y][x])
                    {
                        List<int> options = FilterRooms(roomLayouts, new bool[4] { room_n, room_e, room_s, room_w }, x == 0 && y == 0);

                        if (options.Count > 0)
                        {
                            mapLayout += options[Random.Range(0, options.Count)];
                        }
                    }
                    mapLayout += ",";
                }
                mapLayout += "#";
            }

            int rx = 0, ry = 7;
            string idx = "";
            foreach (char c in mapLayout)
            {
                if (c == '#')
                {
                    rx = 0;
                    ry--;
                }
                else if (c == ',')
                {
                    if (idx != "")
                    {
                        GameObject room = new GameObject("Room " + rx + ":" + ry);
                        room.transform.SetParent(transform);
                        room.transform.position = tilePosition(4.5f, 3.5f, rx, ry);
                        GenerateRoom(roomLayouts[int.Parse(idx)], rx, ry, room.transform);
                        idx = "";
                    }
                    rx++;
                }
                else
                {
                    idx += c;
                }
            }
        }

        private List<string> AddRooms()
        {
            List<string> roomLayouts = new List<string>();

            // 4 Doors
            roomLayouts.Add(
                "||||: ||||#" +
                "|-      -|#" +
                "| 1    2 |#" +
                ":   VV   :#" +
                "    VV    #" +
                "| 4    3 |#" +
                "|-      -|#" +
                "||||: ||||#" +
                "S1,2,3,4#" +
                "S3,4,1,2#"
            );
            // 3 Doors
            roomLayouts.Add(
                "||||  ||||#" +
                "|        |#" +
                "|        |#" +
                "         |#" +
                "         |#" +
                "|        |#" +
                "|        |#" +
                "||||  ||||#"
            );
            roomLayouts.Add(
                "||||||||||#" +
                "|        |#" +
                "|        |#" +
                "          #" +
                "          #" +
                "|        |#" +
                "|        |#" +
                "||||  ||||#"
            );
            roomLayouts.Add(
                "||||  ||||#" +
                "|xxxxxxxx|#" +
                "|xxxxx xx|#" +
                "|xxxx xxx #" +
                "|xxxx x   #" +
                "|xxxxx xx|#" +
                "| xxxx xx|#" +
                "||||  ||||#"
            );
            roomLayouts.Add(
                "||||  ||||#" +
                "|2      3|#" +
                "|  i||i  |#" +
                "   ||||   #" +
                "   ||||   #" +
                "|  i||i  |#" +
                "|1      4|#" +
                "||||||||||#" +
                "F1,2,3,4#" +
                "F2,3,4,1#" +
                "F3,4,1,2#" +
                "F4,1,2,3#"
            );
            // 2 Doors
            roomLayouts.Add(
                "||||X ||||#" +
                "|VVi  iVV|#" +
                "|VV    VV|#" +
                "| 1    2 |#" +
                "| 3    4 |#" +
                "|VVV  VVV|#" +
                "|VVV  VVV|#" +
                "||||  ||||#" +
                "S1,2#" +
                "S3,4#"
            );
            roomLayouts.Add(
                "||||||||||#" +
                "|  ****  |#" +
                "|        |#" +
                "     xx   #" +
                "    |Kx   #" +
                "|   ||   |#" +
                "|        |#" +
                "||||||||||#"
            );
            roomLayouts.Add(
                "||||||||||#" +
                "|  ****  |#" +
                "|        |#" +
                "     xx   #" +
                "    |cx   #" +
                "|   ||   |#" +
                "|        |#" +
                "||||||||||#"
            );

            roomLayouts.Add(
                "||||||||||#" +
                "|  ****  |#" +
                "|        |#" +
                "     xx   #" +
                "    |gx   #" +
                "|   ||   |#" +
                "|        |#" +
                "||||||||||#"
            );
            roomLayouts.Add(
                "||||  ||||#" +
                "|       ||#" +
                "| xxxxxx |#" +
                "|      x  #" +
                "|2 4 6 x  #" +
                "|  g   x |#" +
                "|1 3 5   |#" +
                "||||||||||#" +
                "S1#" +
                "S2#" +
                "S3#" +
                "S4#" +
                "S5#" +
                "S6#"
            );
            roomLayouts.Add(
                "||||  ||||#" +
                "|  x  x  |#" +
                "|       x|#" +
                "|  -      #" +
                "|  -      #" +
                "|   --  x|#" +
                "|        |#" +
                "||||||||||#"
            );
            roomLayouts.Add(
                "||||  ||||#" +
                "|VVV  VVV|#" +
                "|V 1  2 V|#" +
                "    ||  c|#" +
                "    ||  V|#" +
                "|V 4  3 V|#" +
                "|VVcVVVVV|#" +
                "||||||||||#" +
                "F1,2,3,4#" +
                "F2,3,4,1#" +
                "F3,4,1,2#" +
                "F4,1,2,3#"
            );
            roomLayouts.Add(
                "||||||||||#" +
                "|        |#" +
                "|        |#" +
                "| *    *  #" +
                "|         #" +
                "| *    * |#" +
                "| 1    2 |#" +
                "||||  ||||#" +
                "S1,2"
            );
            roomLayouts.Add(
                "||||||||||#" +
                "|g      u|#" +
                "||||||X ||#" +
                "         |#" +
                "         |#" +
                "|*       |#" +
                "|K*      |#" +
                "||||  ||||#"
            );
            // 1 Door
            roomLayouts.Add(
                "||||  ||||#" +
                "|i      i|#" +
                "|        |#" +
                "|   uu   |#" +
                "|   uu   |#" +
                "|     |X |#" +
                "|i    | L|#" +
                "||||||||||#"
            );
            roomLayouts.Add(
                "||||||||||#" +
                "|   V  1 |#" +
                "| V V   3|#" +
                "  V2  V  |#" +
                "  VVVVV  |#" +
                "|VV   V  |#" +
                "|g  V 4  |#" +
                "||||||||||#" +
                "F1,2#" +
                "F3,4#"
            );
            roomLayouts.Add(
                "VVVVVVVVVV#" +
                "VVVVVVVVVV#" +
                "VVVVg VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "||||  ||||#"
            );
            roomLayouts.Add(
                "VVVVVVVVVV#" +
                "VVVVVVVVVV#" +
                "VVVVc VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "||||  ||||#"
            );
            roomLayouts.Add(
                "VVVVVVVVVV#" +
                "VVVVVVVVVV#" +
                "VVVV12VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "VVVV  VVVV#" +
                "||||  ||||#" +
                "F1,2#"
            );
            roomLayouts.Add(
                "||||||||||#" +
                "|        |#" +
                "|        |#" +
                "|        X#" +
                "| L       #" +
                "|        |#" +
                "|        |#" +
                "||||||||||#"
            );

            return roomLayouts;
        }

        private List<int> FilterRooms(List<string> roomLayouts, bool[] specs, bool isPeacefull)
        {
            List<int> filtered = new List<int>();

            for (int i = 0; i < roomLayouts.Count; i++)
            {
                RoomLayout layout = roomLayoutFromString(roomLayouts[i]);
                if (layout.doormap[0] == specs[0] && layout.doormap[1] == specs[1] && layout.doormap[2] == specs[2] && layout.doormap[3] == specs[3])
                {
                    if (!(isPeacefull && layout.enemyLayout != ""))
                        filtered.Add(i);
                }
            }

            return filtered;
        }

        private void GenerateRoom(string room, float room_x, float room_y, Transform parent)
        {
            RoomLayout rl = roomLayoutFromString(room);
            KeyValuePair<TileType, Orientation>[][] tileMap = rl.room;

            for (int y = 0; y < tileMap.Length; y++)
            {
                for (int x = 0; x < tileMap[y].Length; x++)
                {
                    GameObject tile = Instantiate(
                            GetGameObject(tileMap[y][x].Key),
                            tilePosition(x, y, room_x, room_y),
                            GetQuaternion(tileMap[y][x].Value),
                            parent);
                    tile.name = "Tile " + tileMap[y][x].Key;
                    if (tileMap[y][x].Key != TileType.WALL && tileMap[y][x].Key != TileType.VOID)
                    {
                        GameObject tilef = Instantiate(
                                GetGameObject(TileType.FLOOR),
                                tilePosition(x, y, room_x, room_y),
                                GetQuaternion(tileMap[y][x].Value),
                                parent);
                        tilef.name = "Tile " + tileMap[y][x].Key;
                    }
                }
            }


            GenerateEnemies(rl.waypoints, rl.enemyLayout, room_x, room_y, parent.transform);
        }

        private void GenerateEnemies(Dictionary<char, Vector2> waypoints, string enemyLayout, float room_x, float room_y, Transform parent)
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
                        enemyType = enemies[0];
                        break;
                    case 'F':
                        enemyType = enemies[1];
                        break;
                    case ',':
                    case '#':
                        Transform waypoint;
                        Vector2 xy = waypoints[idx];
                        if (!instantiated.ContainsKey(idx))
                        {
                            GameObject wp = new GameObject("Waypoint " + xy.x + ":" + xy.y);
                            wp.transform.position = tilePosition(xy.x, xy.y, room_x, room_y) + Vector3.up;
                            wp.transform.SetParent(parent);

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
                            GameObject enemy = Instantiate(enemyType, path[0].position, Quaternion.identity, parent);
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
                case TileType.VOID:
                    return tileObjects[0];
                case TileType.FLOOR:
                    return tileObjects[1];
                case TileType.WALL:
                    return tileObjects[2];
                case TileType.STAIRS:
                    return tileObjects[3];
                case TileType.TOURCH:
                    return tileObjects[4];
                case TileType.STATUE:
                    return tileObjects[5];
                case TileType.SOCKET:
                    return tileObjects[6];
                case TileType.RAMP:
                    return tileObjects[7];
                case TileType.POT:
                    return tileObjects[8];
                case TileType.BOX:
                    return tileObjects[9];
                case TileType.MOVEABLE_STONE:
                    return tileObjects[10];
                case TileType.CHEST_WOOD:
                    return tileObjects[11];
                case TileType.CHEST_GOLD:
                    return tileObjects[12];
                case TileType.LOCKED_DOOR:
                    return tileObjects[13];
                case TileType.KEY:
                    return tileObjects[14];
                case TileType.SPIKES:
                    return tileObjects[15];
                case TileType.SWITCH:
                    return tileObjects[16];
                case TileType.BOSS_GATE:
                    return tileObjects[17];
                case TileType.DOOR:
                    return tileObjects[18];
                default:
                    return null;
            }
        }

        private Quaternion GetQuaternion(Orientation orientation)
        {
            Quaternion quat = Quaternion.identity;
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

            Dictionary<char, Vector2> waypoints = new Dictionary<char, Vector2>();
            string enemyLayout = "";

            int ix = 0, iy = 0;
            foreach (char c in map)
            {
                switch (c)
                {
                    case 'V':
                        lookup_row.Add(TileType.VOID);
                        break;
                    case ' ':
                        lookup_row.Add(TileType.FLOOR);
                        break;
                    case '|':
                        lookup_row.Add(TileType.WALL);
                        break;
                    case 'L':
                        lookup_row.Add(TileType.STAIRS);
                        break;
                    case 'i':
                        lookup_row.Add(TileType.TOURCH);
                        break;
                    case 'T':
                        lookup_row.Add(TileType.STATUE);
                        break;
                    case '-':
                        lookup_row.Add(TileType.SOCKET);
                        break;
                    case '/':
                        lookup_row.Add(TileType.RAMP);
                        break;
                    case 'u':
                        lookup_row.Add(TileType.POT);
                        break;
                    case 'n':
                        lookup_row.Add(TileType.BOX);
                        break;
                    case 'x':
                        lookup_row.Add(TileType.MOVEABLE_STONE);
                        break;
                    case 'c':
                        lookup_row.Add(TileType.CHEST_WOOD);
                        break;
                    case 'g':
                        lookup_row.Add(TileType.CHEST_GOLD);
                        break;
                    case 'X':
                        lookup_row.Add(TileType.LOCKED_DOOR);
                        break;
                    case 'K':
                        lookup_row.Add(TileType.KEY);
                        break;
                    case '*':
                        lookup_row.Add(TileType.SPIKES);
                        break;
                    case '%':
                        lookup_row.Add(TileType.SWITCH);
                        break;
                    case 'H':
                        lookup_row.Add(TileType.BOSS_GATE);
                        break;
                    case ':':
                        lookup_row.Add(TileType.DOOR);
                        break;
                    case '#':
                        if (iy < 8)
                        {
                            lookup_temp.Add(lookup_row.ToArray());
                            lookup_row = new List<TileType>();
                            ix = -1;
                            iy++;
                        }
                        else
                        {
                            enemyLayout += c;
                        }
                        break;
                    default:
                        if (iy < 8)
                        {
                            waypoints.Add(c, new Vector2(ix, iy));
                            lookup_row.Add(TileType.FLOOR);
                        }
                        else
                        {
                            enemyLayout += c;
                        }
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
                    if (lookup[y][x] == TileType.LOCKED_DOOR || lookup[y][x] == TileType.BOSS_GATE || lookup[y][x] == TileType.DOOR)
                    {
                        if (x == 0 || x == 9)
                            row.Add(new KeyValuePair<TileType, Orientation>(lookup[y][x], Orientation.EAST));
                        else
                            row.Add(new KeyValuePair<TileType, Orientation>(lookup[y][x], Orientation.NORTH));
                    } else
                        row.Add(new KeyValuePair<TileType, Orientation>(lookup[y][x], Orientation.NORTH));
                }
                o[y] = row.ToArray();
            }

            RoomLayout layout = new RoomLayout();
            layout.room = o;
            layout.doormap = new bool[4] { o[7][5].Key != TileType.WALL && o[7][5].Key != TileType.VOID, o[3][9].Key != TileType.WALL && o[3][9].Key != TileType.VOID, o[0][5].Key != TileType.WALL && o[0][5].Key != TileType.VOID, o[3][0].Key != TileType.WALL && o[3][0].Key != TileType.VOID };
            layout.waypoints = waypoints;
            layout.enemyLayout = enemyLayout;

            return layout;
        }

        Vector3 tilePosition(float x, float y, float room_x, float room_y)
        {
            return new Vector3(transform.position.x + (x + room_x * 9 + 0.5f) * tileSize.x, transform.position.y, transform.position.z + (y + room_y * 7 + 0.5f) * tileSize.y);
        }
    }
}