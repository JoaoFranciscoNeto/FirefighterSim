using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace FireModelling
{


    public class Grid : MonoBehaviour
    {

        public GameObject cellPrefab;

        public GridCell[,] cellGrid;

        public int VisualizationMode = 0;

        // Use this for initialization
        void Start()
        {
            cellGrid = new GridCell[100, 100];

            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    
                    cellGrid[x, y] = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<GridCell>();
                    cellGrid[x, y].grid = this;
                }
            }

            cellGrid[Random.Range(0, 100), Random.Range(0, 100)].state = GridCell.State.OnFire;
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
            GridCell[,] newGrid = new GridCell[100, 100];
            for (int x = 0; x < 100; x++)
            {
                for (int y = 0; y < 100; y++)
                {
                    newGrid[x, y] = cellGrid[x, y].SimulationTick();
                }
            }
            cellGrid = newGrid.Clone() as GridCell[,];
        }



        public void OnVisualizationChange(int newState)
        {
            Debug.Log("Changed visualization to " + newState);
            VisualizationMode = newState;
        }

    }
    
}