using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FireModelling
{


    public class Grid : MonoBehaviour
    {
        public Cell[,] cellGrid;


        // Use this for initialization
        void Start()
        {
            cellGrid = new Cell[100, 100];

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    cellGrid[x, y] = new Cell(this, new Vector2(x, y));
                }
            }

            cellGrid[Random.Range(0, 100), Random.Range(0, 100)].state = Cell.State.OnFire;
        }

        float timeToStep = 1.0f;

        // Update is called once per frame
        void Update()
        {
            timeToStep -= Time.deltaTime;
            if (timeToStep<=0)
            {

                SimulationTick();
                timeToStep = 1.0f;
            }
        }

        void SimulationTick()
        {
            Cell[,] newGrid = new Cell[100, 100];
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    newGrid[x, y] = cellGrid[x, y].SimulationTick();
                }
            }
            cellGrid = newGrid.Clone() as Cell[,];
        }

        private void OnDrawGizmos()
        {
            foreach (Cell cell in cellGrid)
            {
                Gizmos.color = cell.DebugColor();
                Gizmos.DrawCube(new Vector3(cell.position.x, 0, cell.position.y), Vector3.one*.9f);
            }
        }


    }


    public class Cell
    {
        Grid grid;

        public enum State
        {
            Clear,
            OnFire,
            Burnt
        }

        public class Info
        {
            public float Temperature;
            public float IgnPoint;
            public Vector2 Wind;
            public float Fuel;

            public Info(float Temperature, float IgnPoint, float Fuel)
            {
                this.Temperature = Temperature;
                this.IgnPoint = IgnPoint;
                this.Wind = new Vector2(.1f, 0);
                this.Fuel = Fuel;
            }
            public Info(float Temperature, float IgnPoint, Vector2 Wind, float Fuel)
            {
                this.Temperature = Temperature;
                this.IgnPoint = IgnPoint;
                this.Wind = Wind;
                this.Fuel = Fuel;
            }

        }

        public Info info;

        public State state;
        public Vector2 position;


        public Cell(Cell current, Info newInfo)
        {
            this.grid = current.grid;
            this.info = newInfo;
            this.position = current.position;
            this.state = current.state;
        }


        public Cell(Grid parent, Vector2 pos)
        {
            this.grid = parent;
            this.position = pos;
            this.state = State.Clear;
            this.info = new Info(30, 60, Random.Range(0f,1f));
        }

        public Cell SimulationTick()
        {
            Info newInfo = new Info(info.Temperature,info.IgnPoint,info.Fuel);

            switch (state)
            {
                case State.Clear:

                    foreach (Cell neighbour in GetNeighbours())
                    {
                        Vector2 dir = neighbour.position - position;
                        float windStrength = Vector2.Dot(dir, info.Wind);
                        newInfo.Temperature += ((neighbour.info.Temperature - info.Temperature) / 2f);
                    }

                    if (newInfo.Temperature > info.IgnPoint)
                    {
                        state = State.OnFire;
                    }

                    break;
                case State.OnFire:

                    // Burn fuel
                    newInfo.Fuel -= .05f;
                    newInfo.Temperature += 50f;
                    // Check if there is fuel left
                    if (newInfo.Fuel <= 0)
                    {
                        newInfo.Fuel = 0;
                        state = State.Burnt;
                    }

                    break;
            }


            return new Cell(this, newInfo);
        }
        

        public Color DebugColor()
        {
            switch (state)
            {
                case State.Clear:
                    return Color.Lerp(Color.green, new Color(0, .5f, 0), info.Fuel);
                case State.OnFire:
                    return Color.red;
                case State.Burnt:
                    return Color.black;
                default:
                    return Color.cyan;
            }
        }

        public Cell[] GetNeighbours()
        {
            List<Cell> neighbours = new List<Cell>();

            if (position.x > 0)
                neighbours.Add(grid.cellGrid[(int)position.x - 1, (int)position.y]);
            if (position.x < grid.cellGrid.GetLength(0)-1)
                neighbours.Add(grid.cellGrid[(int)position.x + 1, (int)position.y]);
            if (position.y > 0)
                neighbours.Add(grid.cellGrid[(int)position.x, (int)position.y - 1]);
            if (position.y < grid.cellGrid.GetLength(1)-1)
                neighbours.Add(grid.cellGrid[(int)position.x, (int)position.y + 1]);

            return neighbours.ToArray();
        }


        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}