using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FireModelling
{

    public class GridCell : MonoBehaviour
    {

        public Grid grid;

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
            public float Fuel;
            

        }

        public State state;
        public Info cellInfo;
        Info newInfo;
        public Vector2 position;

        private void Awake()
        {
            this.state = State.Clear;
            cellInfo = new Info();

            cellInfo.Temperature = 28;
            cellInfo.IgnPoint = 100;
            cellInfo.Fuel = 200;
        }
        
        GridCell[] GetNeighbours()
        {
            List<GridCell> neighbours = new List<GridCell>();

            if (position.x > 0)
                neighbours.Add(grid.cellGrid[(int)position.x - 1, (int)position.y]);
            if (position.x < grid.cellGrid.GetLength(0) - 1)
                neighbours.Add(grid.cellGrid[(int)position.x + 1, (int)position.y]);
            if (position.y > 0)
                neighbours.Add(grid.cellGrid[(int)position.x, (int)position.y - 1]);
            if (position.y < grid.cellGrid.GetLength(1) - 1)
                neighbours.Add(grid.cellGrid[(int)position.x, (int)position.y + 1]);

            return neighbours.ToArray();
        }

        public GridCell SimulationTick()
        {
            newInfo = Clone(cellInfo);

            newInfo.Temperature = UpdateTemperature();

            switch (state)
            {
                case State.Clear:
                    if (newInfo.Temperature >= newInfo.IgnPoint)
                        state = State.OnFire;
                    break;
                case State.OnFire:
                    newInfo.Temperature += 50;

                    newInfo.Fuel -= 10;
                    if (newInfo.Fuel <= 0)
                    {
                        newInfo.Fuel = 0;
                        state = State.Burnt;
                    }
                    break;
            }


            return this;
        }

        public static float StefanBoltzman = 5.6703e-8f;

        float UpdateTemperature()
        {
            float temp = cellInfo.Temperature;
            foreach (GridCell neighbour in GetNeighbours())
            {
                temp += StefanBoltzman * (Mathf.Pow(neighbour.cellInfo.Temperature, 4) - Mathf.Pow(cellInfo.Temperature, 4));
            }
            Debug.Log(temp);
            return temp;
        }

        public void UpdateInfo()
        {
            cellInfo = Clone(newInfo);
        }

        private void OnDrawGizmos()
        {
            switch(grid.VisualizationMode)
            {
                case 0:
                    switch (state)
                    {
                        case State.Clear:
                            Gizmos.color = Color.green;
                            break;
                        case State.Burnt:
                            Gizmos.color = Color.black;
                            break;
                        case State.OnFire:
                            Gizmos.color = Color.red;
                            break;
                    }
                    break;
                case 1:
                    Gizmos.color = Color.Lerp(Color.green, Color.red, cellInfo.Temperature / 100f);
                    break;


            }

            Gizmos.DrawCube(transform.position, Vector3.one * .95f);
        }


        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }

        public float CelsiusToKelvin(float celsius)
        {
            return celsius + 273.15f;
        }
    }
}