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
            Unburned,
            EarlyBurning,
            Burning,
            Extinguishing,
            Extinguished
        }

        public class CellData
        {
            public float Altitude;
        }


        public State state;
        State newState;
        float stateVal = 0;

        public CellData cellData;
        public Vector2 position;
        

        private void Awake()
        {
            state = State.Unburned;
            newState = State.Unburned;

            cellData = new CellData();

            cellData.Altitude = 10f;
            
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
        

        public void SimulationTick()
        {
            GridCell[] n = GetNeighbours();

            switch (state)
            {
                case State.Unburned:
                    bool neighbourOnFire = false;
                    foreach (GridCell neighbour in n)
                    {
                        if (neighbour != null && neighbour.state == State.Burning)
                        {
                            neighbourOnFire = true;
                            break;
                        }
                    }

                    if (neighbourOnFire)
                    {
                        stateVal += Eq2(n);

                        state = (State)Mathf.FloorToInt(stateVal);
                    }

                    break;
                case State.EarlyBurning:
                    state = State.Burning;
                    break;
                case State.Burning:
                    bool neighbourBurning = true;
                    foreach (GridCell neighbour in n)
                    {
                        if (neighbour != null && neighbour.state < State.Burning)
                        {
                            neighbourBurning = false;
                            break;
                        }
                    }

                    if (neighbourBurning)
                    {
                        state = State.Extinguishing;
                    }
                    break;
                case State.Extinguishing:
                    state = State.Extinguished;
                    break;
            }
            
        }


        float Eq2(GridCell[] neighbours)
        {

            float rSum = 0;

            for (int i = 0; i < 8; i++)
            {
                GridCell n = neighbours[i];
                if (n!=null)
                {

                    rSum += Eq1(n) * ((n.state == State.Burning) ? 1 : 0);

                }
            }


            return rSum * grid.deltaT / grid.L;
        }

        float a = 0.03f;
        float b = 0.05f;
        float c = 0.01f;
        float d = 0.3f;

        float T = 25;
        float v = 5;
        float RH = 20;

        float Eq1(GridCell n)
        {
            float R0 = a * T + b * W() + c * (100 - RH) - d;
            float k = Kphi(n.position - position);
            
            return R0 * k;
        }

        float W()
        {
            return (int)(v / 0.836f) ^ (2 / 3);
        }

        float Kphi(Vector2 spreadDir)
        {

            return Mathf.Exp(.1783f * v * Mathf.Cos(Vector2.Angle(spreadDir, grid.GlobalWind) * Mathf.Deg2Rad));
        }

        public void UpdateInfo()
        {


            //state = newState;
        }

        public Color CellColor()
        {
            switch (grid.VisualizationMode)
            {
                case 0:
                    switch (state)
                    {
                        case State.Unburned:
                            return Color.green;
                        case State.EarlyBurning:
                            return Color.yellow;
                        case State.Burning:
                            return Color.red;
                        case State.Extinguishing:
                            return Color.gray;
                        case State.Extinguished:
                            return Color.black;
                        default:
                            return Color.blue;
                    }
                case 1:
                    return Color.blue;
                default:
                    return Color.white;
            }
        }

        public static T Clone<T>(T source)
        {
            var serialized = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<T>(serialized);
        }
    }
}