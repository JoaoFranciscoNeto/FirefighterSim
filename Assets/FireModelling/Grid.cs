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
            cellGrid = new Cell[10, 10];

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    cellGrid[x, y] = new Cell(this, new Vector2(x, y));
                }
            }

            cellGrid[Random.Range(0, 10), Random.Range(0, 10)].state = Cell.State.OnFire;
        }

        // Update is called once per frame
        void Update()
        {
            SimulationTick();
        }

        void SimulationTick()
        {
            Cell[,] newGrid = new Cell[10, 10];
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
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

            public Info(float Temperature, float IgnPoint, Vector2 Wind, float Fuel)
            {
                this.Temperature = Temperature;
                this.IgnPoint = IgnPoint;
                this.Wind = Wind;
                this.Fuel = Fuel;
            }
            
        }

        Info info;
        Info newInfo;

        public State state;
        public Vector2 position;



        public Cell(Grid parent, Vector2 pos)
        {
            this.grid = parent;
            this.position = pos;
            this.state = State.Clear;
        }

        public Cell SimulationTick()
        {
            Cell newMe = null;
            foreach (Cell cell in GetNeighbours())
            {

            }

            return newMe;
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
            if (position.x < grid.cellGrid.GetLength(0))
                neighbours.Add(grid.cellGrid[(int)position.x + 1, (int)position.y]);
            if (position.y > 0)
                neighbours.Add(grid.cellGrid[(int)position.x, (int)position.y - 1]);
            if (position.y < grid.cellGrid.GetLength(1))
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