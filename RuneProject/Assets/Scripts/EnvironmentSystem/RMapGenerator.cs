using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.EnvironmentSystem
{
    enum TileType
    {
        WALL,
        EMPTY,
        CORNER,
        EDGE,
        PILLAR,
        DOOR
    }

    enum Orientation
    {
        NORTH,
        EAST,
        SOUTH,
        WEST
    }

    public class RMapGenerator : MonoBehaviour
    {
        private KeyValuePair<TileType, Orientation>[][] tileMap;
        [SerializeField] private Vector2 tileSize;
        [SerializeField] private GameObject[] tileObjects;

        [Header("Enemies")]
        [SerializeField] private GameObject slime;
        [SerializeField] private GameObject fairy;

        private Dictionary<char, Vector2> waypoints = new Dictionary<char, Vector2>();

        internal KeyValuePair<TileType, Orientation>[][] TileMap { get => tileMap; set => tileMap = value; }

        void Start()
        {
            tileMap = tileMapFromString(
                "||||||||||||||||||||#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|                  |#" +
                "|    1        2    |#" +
                "|                  |#" +
                "||||||        ||||||#");

            string enemyLayout =
                "S1,2#" +
                "S2,1#";

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
                            waypoint = Instantiate(new GameObject(), tilePosition(xy.x,xy.y) + Vector3.up, new Quaternion(), transform).transform;
                            instantiated.Add(idx, waypoint);
                        } else
                        {
                            waypoint = instantiated[idx];
                        }
                        path.Add(waypoint);
                        if (c == '#')
                        {
                            GameObject enemy = Instantiate(enemyType, path[0].position, new Quaternion(), transform);
                            enemy.GetComponent<EnemySystem.REnemyAI>().Path = path;
                            path = new List<Transform>();
                        }
                        break;
                    default:
                        idx = c;
                        break;

                }
            }

            GenerateMap();
        }

        private void GenerateMap()
        {
            for (int y = 0; y < tileMap.Length; y++)
            {
                for (int x = 0; x < tileMap[y].Length; x++)
                {
                    if (TileMap[x][y].Key != TileType.EMPTY)
                        Instantiate(
                            GetGameObject(TileMap[x][y].Key),
                            tilePosition(x, y),
                            GetQuaternion(TileMap[x][y].Value),
                            transform);
                }
            }
        }

        private GameObject GetGameObject(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.WALL:
                    return tileObjects[0];
                case TileType.CORNER:
                    return tileObjects[1];
                case TileType.PILLAR:
                    return tileObjects[2];
                case TileType.DOOR:
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

        private KeyValuePair<TileType, Orientation>[][] tileMapFromString(string map)
        {

            List<TileType[]> lookup_temp = new List<TileType[]>();
            List<TileType> lookup_row = new List<TileType>();

            int ix = 0, iy = 0;
            foreach (char c in map)
            {
                switch (c)
                {
                    case ' ':
                        lookup_row.Add(TileType.EMPTY);
                        break;
                    case '|':
                        lookup_row.Add(TileType.WALL);
                        break;
                    case ':':
                        lookup_row.Add(TileType.DOOR);
                        break;
                    case '#':
                        lookup_temp.Add(lookup_row.ToArray());
                        lookup_row = new List<TileType>();
                        ix = -1;
                        iy++;
                        break;
                    default:
                        waypoints.Add(c, new Vector2(ix, iy));
                        lookup_row.Add(TileType.EMPTY);
                        break;
                }
                ix++;
            }

            lookup_temp.Reverse();

            Dictionary<char, Vector2> rwayp = new Dictionary<char, Vector2>();
            foreach (KeyValuePair<char, Vector2> wp in waypoints)
            {
                rwayp[wp.Key] = new Vector2(wp.Value.x, lookup_temp.Count - wp.Value.y);
            }
            waypoints = rwayp;

            TileType[][] lookup = lookup_temp.ToArray();


            KeyValuePair<TileType, Orientation>[][] o = new KeyValuePair<TileType, Orientation>[lookup.Length][];

            for (int y = 0; y < lookup.Length; y++)
            {
                List<KeyValuePair<TileType, Orientation>> row = new List<KeyValuePair<TileType, Orientation>>();
                for (int x = 0; x < lookup[y].Length; x++)
                {
                    switch (lookup[x][y])
                    {
                        case TileType.WALL:
                        case TileType.DOOR:
                            bool cat = false;

                            bool open_n = y != 0;
                            bool wall_n = open_n && (lookup[x][y - 1] == TileType.WALL || lookup[x][y - 1] == TileType.DOOR);
                            bool open_e = x != lookup.Length - 1;
                            bool wall_e = open_e && (lookup[x + 1][y] == TileType.WALL || lookup[x + 1][y] == TileType.DOOR);
                            bool open_s = y != lookup[x].Length - 1;
                            bool wall_s = open_s && (lookup[x][y + 1] == TileType.WALL || lookup[x][y + 1] == TileType.DOOR);
                            bool open_w = x != 0;
                            bool wall_w = open_w && (lookup[x - 1][y] == TileType.WALL || lookup[x - 1][y] == TileType.DOOR);

                            if (lookup[x][y] == TileType.WALL)
                            {
                                if (!wall_n && !wall_e && !wall_s && !wall_w) {
                                    row.Add(new KeyValuePair<TileType, Orientation>(TileType.PILLAR, Orientation.NORTH));
                                    cat = true;
                                }

                                if (wall_n && wall_e) {
                                    row.Add(new KeyValuePair<TileType, Orientation>(TileType.CORNER, Orientation.SOUTH));
                                    cat = true;
                                }
                                if (wall_e && wall_s) {
                                    row.Add(new KeyValuePair<TileType, Orientation>(TileType.CORNER, Orientation.WEST));
                                    cat = true;
                                }
                                if (wall_s && wall_w) {
                                    row.Add(new KeyValuePair<TileType, Orientation>(TileType.CORNER, Orientation.NORTH));
                                    cat = true;
                                }
                                if (wall_w && wall_n) {
                                    row.Add(new KeyValuePair<TileType, Orientation>(TileType.CORNER, Orientation.EAST));
                                    cat = true;
                                }
                            }

                            if (!cat && (wall_n || wall_s)) {
                                row.Add(new KeyValuePair<TileType, Orientation>(lookup[x][y], Orientation.EAST));
                                cat = true;
                            }
                            if (!cat && (wall_e || wall_w)) {
                                row.Add(new KeyValuePair<TileType, Orientation>(lookup[x][y], Orientation.NORTH));
                                cat = true;
                            }
                            if (!cat) row.Add(new KeyValuePair<TileType, Orientation>(TileType.PILLAR, Orientation.NORTH));
                            break;

                        case TileType.EMPTY:
                            row.Add(new KeyValuePair<TileType, Orientation>(TileType.EMPTY, Orientation.NORTH));
                            break;
                    }
                }
                o[y] = row.ToArray();
            }

            return o;
        }

        Vector3 tilePosition(float x, float y)
        {
            return new Vector3(transform.position.x + (x + 0.5f) * tileSize.x, transform.position.y, transform.position.z + (y + 0.5f) * tileSize.y);
        }
    }
}