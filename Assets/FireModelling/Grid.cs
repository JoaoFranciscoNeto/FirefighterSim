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

        public int gridSize = 100;

        // Use this for initialization
        void Start()
        {
            InitValues();
            cellGrid = new GridCell[gridSize, gridSize];
            visTexture = new Texture2D(gridSize, gridSize);
            visualizer.material.mainTexture = visTexture;
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    
                    cellGrid[x, y] = Instantiate(cellPrefab, new Vector3(x, 0, y), Quaternion.identity, transform).GetComponent<GridCell>();
                    cellGrid[x, y].grid = this;
                    cellGrid[x, y].position = new Vector2(x, y);
                }
            }

            cellGrid[Random.Range(0, gridSize), Random.Range(0, gridSize)].state = GridCell.State.Burning;
        }

        float timeToStep = .125f;
        
        public static float[] Kphi;

        public float deltaT;

        public Vector2 GlobalWind;

        float m = .125f;
        public float L = 30f;
        float Rmax = 50f; 

        void InitValues()
        {
            deltaT = m * L / Rmax;
            GlobalWind = Vector2.up.normalized;
        }

        // Update is called once per frame
        void Update()
        {
            SimulationTick();
            /*
            timeToStep -= Time.deltaTime;
            if (timeToStep<=0)
            {
                SimulationTick();
                timeToStep = .125f;
            }*/
        }

        void SimulationTick()
        {
            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    cellGrid[x, y].SimulationTick();
                    visTexture.SetPixel(x, y, cellGrid[x, y].CellColor());
                }
            }

            visTexture.Apply();

            for (int x = 0; x < gridSize; x++)
            {
                for (int y = 0; y < gridSize; y++)
                {
                    cellGrid[x, y].UpdateInfo();
                }
            }
        }
        
        public void OnVisualizationChange(int newMode)
        {
            Debug.Log("Changed visualization to " + newMode);
            VisualizationMode = newMode;
        }

    }
    
}