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

        Texture2D visTexture;

        public MeshRenderer visualizer;

        public int VisualizationMode = 0;

        // Use this for initialization
        void Start()
        {
            cellGrid = new GridCell[10, 10];
            visTexture = new Texture2D(10, 10);
            visualizer.material.mainTexture = visTexture;
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    
                    cellGrid[x, y] = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<GridCell>();
                    cellGrid[x, y].grid = this;
                    cellGrid[x, y].position = new Vector2(x, y);
                }
            }

            cellGrid[2, 2].state = GridCell.State.Burning;
        }

        float timeToStep = .5f;

        // Update is called once per frame
        void Update()
        {
            timeToStep -= Time.deltaTime;
            if (timeToStep<=0)
            {
                SimulationTick();
                timeToStep = .2f;
            }
        }

        void SimulationTick()
        {
            GridCell[,] newGrid = new GridCell[10, 10];
            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    cellGrid[x, y].SimulationTick();
                    visTexture.SetPixel(x, y, cellGrid[x, y].CellColor());
                }
            }

            visTexture.Apply();

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    cellGrid[x, y].UpdateInfo();
                }
            }
        }



        public void OnVisualizationChange(int newState)
        {
            Debug.Log("Changed visualization to " + newState);
            VisualizationMode = newState;
        }

    }
    
}