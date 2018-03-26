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
            Normal,
            Warming,
            Burning,
            Inactive
        }

        public class CellData
        {
            public float HeatThreshold;
            public float IgnitionThreshold;
            public float FuelReserves;
        }


        public State state;
        State newState;

        public CellData cellData;
        public Vector2 position;

        public float Heat = 20;
        float newHeat;

        private void Awake()
        {
            state = State.Normal;
            newState = State.Normal;

            cellData = new CellData();

            cellData.HeatThreshold = .06f;
            cellData.IgnitionThreshold = 15;
            cellData.FuelReserves = 19;

            Heat = 0;
        }

        GridCell[] GetNeighbours()
        {
            GridCell[] neigh = new GridCell[8];

            // WW
            if (position.x > 0)
                neigh[0] = grid.cellGrid[(int)position.x - 1, (int)position.y];
            // EE
            if (position.x < grid.cellGrid.GetLength(0) - 1)
                neigh[1] = grid.cellGrid[(int)position.x + 1, (int)position.y];
            // NN
            if (position.y > 0)
                neigh[2] = grid.cellGrid[(int)position.x, (int)position.y - 1];
            // SS
            if (position.y < grid.cellGrid.GetLength(1) - 1)
                neigh[3] = grid.cellGrid[(int)position.x, (int)position.y + 1];
            // NW
            if (position.x > 0 && position.y > 0)
                neigh[4] = grid.cellGrid[(int)position.x - 1, (int)position.y - 1];
            // SW
            if (position.x > 0 && position.y < grid.cellGrid.GetLength(1) - 1)
                neigh[5] = grid.cellGrid[(int)position.x - 1, (int)position.y + 1];
            // NE
            if (position.x < grid.cellGrid.GetLength(0) - 1 && position.y > 0)
                neigh[6] = grid.cellGrid[(int)position.x + 1, (int)position.y - 1];
            // SE
            if (position.x < grid.cellGrid.GetLength(0) - 1 && position.y < grid.cellGrid.GetLength(1) - 1)
                neigh[7] = grid.cellGrid[(int)position.x + 1, (int)position.y + 1];

            return neigh;
        }

        static float alpha = 1;
        static float beta = Mathf.Sqrt(2);

        static float temperature = 1;
        static float humidity = 1;

        public GridCell SimulationTick()
        {
            GridCell[] neigh = GetNeighbours();

            if (state!= State.Inactive)
            {
                newState = state;


                float A = alpha * (
                    ((neigh[0] && neigh[0].state == State.Burning) ? neigh[0].Heat : 0) +
                    ((neigh[1] && neigh[1].state == State.Burning) ? neigh[1].Heat : 0) +
                    ((neigh[2] && neigh[2].state == State.Burning) ? neigh[2].Heat : 0) +
                    ((neigh[3] && neigh[3].state == State.Burning) ? neigh[3].Heat : 0));

                float B = beta * (
                    ((neigh[4] && neigh[4].state == State.Burning) ? neigh[4].Heat : 0) +
                    ((neigh[5] && neigh[5].state == State.Burning) ? neigh[5].Heat : 0) +
                    ((neigh[6] && neigh[6].state == State.Burning) ? neigh[6].Heat : 0) +
                    ((neigh[7] && neigh[7].state == State.Burning) ? neigh[7].Heat : 0));


                newHeat = temperature * humidity * (Heat + A + B);
                if (position == Vector2.zero)
                {
                    Debug.Log(newHeat);
                }


                if (state == State.Normal && newHeat >= cellData.HeatThreshold && newHeat < cellData.IgnitionThreshold)
                    newState = State.Warming;
                else if (state == State.Warming && newHeat >= cellData.IgnitionThreshold)
                    newState = State.Burning;

                if (state == State.Burning)
                {
                    newHeat += 1;
                    cellData.FuelReserves -= 1;
                    if (cellData.FuelReserves <= 0)
                        newState = State.Inactive;
                }


            } else
            {
                newHeat = Heat;
                newState = State.Inactive;
            }


            return this;
        }


        public void UpdateInfo()
        {
            Heat = newHeat;


            state = newState;
        }

        public Color CellColor()
        {
            switch (grid.VisualizationMode)
            {
                case 0:
                    switch (state)
                    {
                        case State.Normal:
                            return Color.green;
                        case State.Warming:
                            return Color.yellow;
                        case State.Burning:
                            return Color.red;
                        case State.Inactive:
                            return Color.black;
                    }
                    break;
                case 1:
                    return Color.Lerp(Color.blue, Color.red, Heat / 100f);
                default:
                    return Color.white;
            }
            return Color.white;
        }

        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}