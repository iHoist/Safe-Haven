using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

public class WorldHandler : MonoBehaviour {
    public static WorldHandler instance;
    public int size = 100;
    public int safetySize = 10;
    public float scale = .1f;

    public GroundCell[,] groundGrid;
    public SoilCell[,] soilGrid;
    public PlantCell[,] plantGrid;

    [Header("Tree Variables")]
    public GameObject treeManager;
    public GameObject[] treePrefabs;
    private List<Vector3> treePositions = new();
    public float treeNoiseScale = .05f;
    public float treeDensity = .5f;

    [Header("Water Variables")]
    public float waterLevel = .4f;

    [Header("Plant Variables")]
    public int randomTickRate = 20000;

    [Header("Tilemap Variables")]
    public Tilemap groundTilemap;
    public Tilemap soilTilemap;
    public Tilemap plantsTilemap;

    [Header("Tile Variables")]
    public Tile landTile;
    public Tile waterTile;

    public Tile upperEdgeTile;
    public Tile lowerEdgeTile;
    public Tile leftEdgeTile;
    public Tile rightEdgeTile;

    public Tile upperLeftCornerTile;
    public Tile lowerLeftCornerTile;
    public Tile upperRightCornerTile;
    public Tile lowerRightCornerTile;

    public Tile upperLeftInverseCornerTile;
    public Tile lowerLeftInverseCornerTile;
    public Tile upperRightInverseCornerTile;
    public Tile lowerRightInverseCornerTile;

    public Tile upperLeftAndLowerRightCornerTile;
    public Tile lowerLeftAndUpperRightCornerTile;

    public Tile[] soilPlotTiles;

    private void Awake() { if (instance == null) instance = this; }

    private void Start() {
        transform.position -= new Vector3(size / 2, size / 2);
        GenerateWorld();

    }

    private void Update() {
        UpdatePlants();
    }

    [ContextMenu("Generate World")]
    public void GenerateWorld() {
        GenerateGrid();
        GenerateBasicTerrain();
        SmoothLandEdges();
        SetLandColliders();
        GenerateTrees();
    }

    [ContextMenu("Generate World Around Safe Zone")]
    public void GenerateWorldAroundSafeZone() {
        GenerateGridAroundSafeZone();
        GenerateBasicTerrain();
        SmoothLandEdges();
        SetLandColliders();
        GenerateTreesAroundSafeZone();

    }

    private void GenerateGrid() {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        float[,] falloffMap = new float[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        groundGrid = new GroundCell[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float noiseValue = noiseMap[x, y];
                noiseValue -= falloffMap[x, y];
                GroundCell cell = new() { position = (x, y) };
                if (noiseValue < waterLevel) cell.type = GroundCell.Type.Water;
                groundGrid[x, y] = cell;
            }
        }

        //Initializes SoilGrid
        soilGrid = new SoilCell[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                SoilCell cell = new() { position = (x, y) }; soilGrid[x, y] = cell;
            }
        }

        //Initializes PlantGrid
        plantGrid = new PlantCell[size, size];
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                PlantCell cell = new() { position = (x, y) }; plantGrid[x, y] = cell;
            }
        }
    }

    private void GenerateGridAroundSafeZone()
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * scale + xOffset, y * scale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        float[,] falloffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (!(x >= size / 2 - safetySize / 2 && x <= size / 2 + safetySize / 2 - 1 && y >= size / 2 - safetySize / 2 && y <= size / 2 + safetySize / 2 - 1))
                {
                    float noiseValue = noiseMap[x, y];
                    noiseValue -= falloffMap[x, y];
                    GroundCell cell = new() { position = (x, y) };
                    if (noiseValue < waterLevel) cell.type = GroundCell.Type.Water;
                    groundGrid[x, y] = cell;
                }
                else
                {
                    if (!groundGrid[x, y].IsType(GroundCell.Type.Water))
                    {
                        groundGrid[x, y].type = GroundCell.Type.Land;
                    }
                }
            }
        }

        //Initializes SoilGrid
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (!(x >= size / 2 - safetySize / 2 && x <= size / 2 + safetySize / 2 - 1 && y >= size / 2 - safetySize / 2 && y <= size / 2 + safetySize / 2 - 1))
                {
                    SoilCell cell = new() { position = (x, y) }; soilGrid[x, y] = cell;
                }
            }
        }

        //Initializes PlantGrid
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (!(x >= size / 2 - safetySize / 2 && x <= size / 2 + safetySize / 2 - 1 && y >= size / 2 - safetySize / 2 && y <= size / 2 + safetySize / 2 - 1))
                {
                    PlantCell cell = new() { position = (x, y) }; plantGrid[x, y] = cell;
                }
            }
        }
    }

    private void GenerateBasicTerrain() {
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                GroundCell cell = groundGrid[x, y];
                if (cell.type == GroundCell.Type.Water) groundTilemap.SetTile(new Vector3Int(x, y), waterTile);
                else groundTilemap.SetTile(new Vector3Int(x, y), landTile);
            }
        }
    }

    private void SmoothLandEdges() {
        while (true) {
            //Creates Corners
            for (int y = 1; y < size - 1; y++) {
                for (int x = 1; x < size - 1; x++) {
                    if (groundGrid[x, y].IsType(GroundCell.Type.Land)) {
                        if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, false, true, false, false }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, false, true, false, true, false, false }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, false, false, false, false }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, false, true, false, false, false, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), upperLeftCornerTile); groundGrid[x, y].type = GroundCell.Type.UpperLeftCorner; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, true, false, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, true, false, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, true, false, true, true, false }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, true, false, true, true, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), lowerLeftCornerTile); groundGrid[x, y].type = GroundCell.Type.LowerLeftCorner; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, false, true, false, false, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, true, true, false, true, false, false, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, false, true, false, false, false }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, true, true, false, true, false, false, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), upperRightCornerTile); groundGrid[x, y].type = GroundCell.Type.UpperRightCorner; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, false, true, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, false, true, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, false, true, false, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, false, true, false, true, true })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), lowerRightCornerTile); groundGrid[x, y].type = GroundCell.Type.LowerRightCorner; }
                    }
                }
            }

            //Creates Inverse Corners
            for (int y = 1; y < size - 1; y++) {
                for (int x = 1; x < size - 1; x++) {
                    if (groundGrid[x, y].IsType(GroundCell.Type.Land)) {
                        if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, false, false, false, false, true })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), upperLeftInverseCornerTile); groundGrid[x, y].type = GroundCell.Type.UpperLeftInverseCorner; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, false, false, false, false, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), lowerLeftInverseCornerTile); groundGrid[x, y].type = GroundCell.Type.LowerLeftInverseCorner; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, false, false, true, false, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), upperRightInverseCornerTile); groundGrid[x, y].type = GroundCell.Type.UpperRightInverseCorner; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, false, false, false, false, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), lowerRightInverseCornerTile); groundGrid[x, y].type = GroundCell.Type.LowerRightInverseCorner; }
                    }
                }
            }

            //Creates Edges
            for (int y = 1; y < size - 1; y++) {
                for (int x = 1; x < size - 1; x++) {
                    if (groundGrid[x, y].IsType(GroundCell.Type.Land)) {
                        if (groundGrid[x + 1, y].IsType(GroundCell.Type.Water) && !groundGrid[x - 1, y].IsType(GroundCell.Type.Water) && !groundGrid[x, y + 1].IsType(GroundCell.Type.Water) && !groundGrid[x, y - 1].IsType(GroundCell.Type.Water)) {
                            groundTilemap.SetTile(new Vector3Int(x, y), rightEdgeTile); groundGrid[x, y].type = GroundCell.Type.RightEdge; }
                        else if (!groundGrid[x + 1, y].IsType(GroundCell.Type.Water) && groundGrid[x - 1, y].IsType(GroundCell.Type.Water) && !groundGrid[x, y + 1].IsType(GroundCell.Type.Water) && !groundGrid[x, y - 1].IsType(GroundCell.Type.Water)) {
                            groundTilemap.SetTile(new Vector3Int(x, y), leftEdgeTile); groundGrid[x, y].type = GroundCell.Type.LeftEdge; }
                        else if (!groundGrid[x + 1, y].IsType(GroundCell.Type.Water) && !groundGrid[x - 1, y].IsType(GroundCell.Type.Water) && groundGrid[x, y + 1].IsType(GroundCell.Type.Water) && !groundGrid[x, y - 1].IsType(GroundCell.Type.Water)) {
                            groundTilemap.SetTile(new Vector3Int(x, y), upperEdgeTile); groundGrid[x, y].type = GroundCell.Type.UpperEdge; }
                        else if (!groundGrid[x + 1, y].IsType(GroundCell.Type.Water) && !groundGrid[x - 1, y].IsType(GroundCell.Type.Water) && !groundGrid[x, y + 1].IsType(GroundCell.Type.Water) && groundGrid[x, y - 1].IsType(GroundCell.Type.Water)) {
                            groundTilemap.SetTile(new Vector3Int(x, y), lowerEdgeTile); groundGrid[x, y].type = GroundCell.Type.LowerEdge; }
                    }
                }
            }

            //Creates Double Corners
            for (int y = 1; y < size - 1; y++) {
                for (int x = 1; x < size - 1; x++) {
                    if (groundGrid[x, y].IsType(GroundCell.Type.Land))
                        if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, false, false, true, false, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), upperLeftAndLowerRightCornerTile); groundGrid[x, y].type = GroundCell.Type.UpperLeftAndLowerRightCorner; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, false, false, false, false, true })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), lowerLeftAndUpperRightCornerTile); groundGrid[x, y].type = GroundCell.Type.LowerLeftAndUpperRightCorner; }
                }
            }

            //Removes Protruding Land Stumps (And Smooths Again if Needed) [UNFINISHED]
            bool landFound = false;
            for (int y = 1; y < size - 1; y++) {
                for (int x = 1; x < size - 1; x++) {
                    if (groundGrid[x, y].IsType(GroundCell.Type.Land))
                        if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, true, false, false, false }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, true, true, false, true, false, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, true, true, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, false, true, false, true, true, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), waterTile); groundGrid[x, y].type = GroundCell.Type.Water; landFound = true; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, true, true, false, false }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, false, true, false, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, true, true, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, false, true, true, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), waterTile); groundGrid[x, y].type = GroundCell.Type.Water; landFound = true; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, true, false, false, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, true, true, false, true, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, true, true, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, false, true, false, true, true, true })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), waterTile); groundGrid[x, y].type = GroundCell.Type.Water; landFound = true; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, true, true, false, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, false, true, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, true, true, true, true, true, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, false, true, true, true })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), waterTile); groundGrid[x, y].type = GroundCell.Type.Water; landFound = true; }
                        else if (CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, false, false, false, false, true }) ||
                            CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, false, false, true, false, false })) {
                            groundTilemap.SetTile(new Vector3Int(x, y), waterTile); groundGrid[x, y].type = GroundCell.Type.Water; landFound = true; }
                }
            }
            if (landFound) continue;

            //Removes Any Land Pieces Still Exposed to Water
            landFound = false;
            for (int y = 1; y < size - 1; y++) {
                for (int x = 1; x < size - 1; x++) {
                    if (groundGrid[x, y].IsType(GroundCell.Type.Land) && groundGrid[x, y].NearbyTileTypeDetected(groundGrid, GroundCell.Type.Water)) {
                        groundTilemap.SetTile(new Vector3Int(x, y), waterTile); groundGrid[x, y].type = GroundCell.Type.Water; landFound = true; }
                }
            }
            if (landFound) continue;

            //Resets All Disconnected Edges And Corners
            landFound = false;
            for (int y = 1; y < size - 1; y++) {
                for (int x = 1; x < size - 1; x++) {
                    if (groundGrid[x, y].IsType(GroundCell.Type.LeftEdge) && (groundGrid[x, y + 1].IsType(GroundCell.Type.Water) || groundGrid[x, y - 1].IsType(GroundCell.Type.Water))) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.RightEdge) && (groundGrid[x, y + 1].IsType(GroundCell.Type.Water) || groundGrid[x, y - 1].IsType(GroundCell.Type.Water))) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.UpperEdge) && (groundGrid[x + 1, y].IsType(GroundCell.Type.Water) || groundGrid[x - 1, y].IsType(GroundCell.Type.Water))) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.LowerEdge) && (groundGrid[x + 1, y].IsType(GroundCell.Type.Water) || groundGrid[x - 1, y].IsType(GroundCell.Type.Water))) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }

                    if (groundGrid[x, y].IsType(GroundCell.Type.UpperLeftCorner) &&
                        !(CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, false, true, false, false }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, false, true, false, true, false, false }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, true, false, false, false, false }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, false, true, false, false, false, false }))) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.LowerLeftCorner) &&
                        !(CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, true, false, true, true, true }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, true, false, true, true, true }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, true, false, true, true, false }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, true, false, true, true, false }))) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.UpperRightCorner) &&
                        !(CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, false, true, false, false, true }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, true, true, false, true, false, false, true }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, true, true, false, true, false, false, false }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, true, true, false, true, false, false, false }))) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.LowerRightCorner) &&
                        !(CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, false, true, true, true, true }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, false, true, true, true, true }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, false, true, false, true, true }) ||
                        CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, false, true, false, true, true }))) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }

                    if (groundGrid[x, y].IsType(GroundCell.Type.UpperLeftInverseCorner) && !CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, false, false, false, false, true })) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.LowerLeftInverseCorner) && !CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, true, false, false, false, false, false })) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.UpperRightInverseCorner) && !CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { false, false, false, false, false, true, false, false })) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.LowerRightInverseCorner) && !CompareBoolArrays(groundGrid[x, y].NearbyTileTypes(groundGrid, GroundCell.Type.Water), new bool[] { true, false, false, false, false, false, false, false })) {
                        groundTilemap.SetTile(new Vector3Int(x, y), landTile); groundGrid[x, y].type = GroundCell.Type.Land; landFound = true; }
                }
            }
            if (landFound) continue;

            //Fixes "Inverse Edge Connections"
            for (int y = 1; y < size - 1; y++) {
                for (int x = 1; x < size - 1; x++) {
                    if (groundGrid[x, y].IsType(GroundCell.Type.LowerEdge) && groundGrid[x + 1, y].IsType(GroundCell.Type.UpperEdge)) {
                        groundGrid[x, y].type = GroundCell.Type.LowerRightCorner; groundTilemap.SetTile(new Vector3Int(x, y), lowerRightCornerTile); groundGrid[x + 1, y].type = GroundCell.Type.UpperLeftCorner; groundTilemap.SetTile(new Vector3Int(x + 1, y), upperLeftCornerTile); }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.UpperEdge) && groundGrid[x + 1, y].IsType(GroundCell.Type.LowerEdge)) {
                        groundGrid[x, y].type = GroundCell.Type.UpperRightCorner; groundTilemap.SetTile(new Vector3Int(x, y), upperRightCornerTile); groundGrid[x + 1, y].type = GroundCell.Type.LowerLeftCorner; groundTilemap.SetTile(new Vector3Int(x + 1, y), lowerLeftCornerTile); }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.LeftEdge) && groundGrid[x, y + 1].IsType(GroundCell.Type.RightEdge)) {
                        groundGrid[x, y].type = GroundCell.Type.UpperLeftCorner; groundTilemap.SetTile(new Vector3Int(x, y), upperLeftCornerTile); groundGrid[x, y + 1].type = GroundCell.Type.LowerRightCorner; groundTilemap.SetTile(new Vector3Int(x, y + 1), lowerRightCornerTile); }
                    else if (groundGrid[x, y].IsType(GroundCell.Type.RightEdge) && groundGrid[x, y + 1].IsType(GroundCell.Type.LeftEdge)) {
                        groundGrid[x, y].type = GroundCell.Type.UpperRightCorner; groundTilemap.SetTile(new Vector3Int(x, y), upperRightCornerTile); groundGrid[x, y + 1].type = GroundCell.Type.LowerLeftCorner; groundTilemap.SetTile(new Vector3Int(x, y + 1), lowerLeftCornerTile); }
                }
            }

            break;
        }
    }

    private void SetLandColliders() {
        for (int x = 0; x < size; x++) {
            for (int y = 0; y < size; y++) {
                Tilemap tilemap = transform.GetChild(0).GetComponent<Tilemap>();
                switch (groundGrid[x, y].type) {
                    case GroundCell.Type.Land: tilemap.SetColliderType(new Vector3Int(x, y), Tile.ColliderType.None); break;
                    case GroundCell.Type.Water: tilemap.SetColliderType(new Vector3Int(x, y), Tile.ColliderType.None); break;
                    default: break;
                }
            }
        }
    }

    private void GenerateTrees() {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        foreach (Transform child in treeManager.transform)
        {
            Destroy(child.gameObject);
        }

        for (int y = 1; y < size - 1; y++)
        {
            for (int x = 1; x < size - 1; x++)
            {
                GroundCell cell = groundGrid[x, y];
                if (cell.IsType(GroundCell.Type.Land) && cell.NearbyTileTypeDetected(groundGrid, GroundCell.Type.Land, true))
                {
                    float v = Random.Range(0f, treeDensity);
                    if (noiseMap[x, y] < v)
                    {
                        bool collidingWithTree = false;
                        foreach (Vector3 treePosition in treePositions)
                        {
                            if (treePosition.x > x - size / 2 - 2 && treePosition.x < x - size / 2 + 2 && treePosition.y > y - size / 2 - 2 && treePosition.y < y - size / 2 + 2)
                            {
                                collidingWithTree = true;
                                break;
                            }
                        }
                        if (collidingWithTree) continue;

                        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                        GameObject tree = Instantiate(prefab, treeManager.transform);
                        tree.transform.position = new(x - size / 2, y - size / 2, 0);
                        treePositions.Add(tree.transform.position);
                    }
                }
            }
        }

        foreach (Transform tree in treeManager.transform) tree.position += new Vector3(0.5f, 1f, 0f);
    }

    private void GenerateTreesAroundSafeZone() {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        foreach (Transform child in treeManager.transform)
        {
            Destroy(child.gameObject);
        }

        //Delete All Trees Outside Safe Zone
        /*foreach (Vector3 treePosition in treePositions)
        {
            if ((treePosition.x + size / 2 < size / 2 - safetySize / 2 && treePosition.x + size / 2 > size / 2 + safetySize / 2 - 1) && (treePosition.y + size / 2 < size / 2 - safetySize / 2 && treePosition.y + size / 2 > size / 2 + safetySize / 2 - 1)) {
                foreach (Transform child in treeManager.transform)
                {
                    if (child.position == treePosition) Destroy(child.gameObject);
                }
            }
        }*/


        for (int y = 1; y < size - 1; y++) {
            for (int x = 1; x < size - 1; x++) {
                if ((x < 45 || x > 54) && (y < 45 || y > 54)) {
                    GroundCell cell = groundGrid[x, y];
                    if (cell.IsType(GroundCell.Type.Land) && cell.NearbyTileTypeDetected(groundGrid, GroundCell.Type.Land, true))
                    {
                        float v = Random.Range(0f, treeDensity);
                        if (noiseMap[x, y] < v)
                        {
                            bool collidingWithTree = false;
                            foreach (Vector3 treePosition in treePositions)
                            {
                                if (treePosition.x > x - size / 2 - 2 && treePosition.x < x - size / 2 + 2 && treePosition.y > y - size / 2 - 2 && treePosition.y < y - size / 2 + 2)
                                {
                                    collidingWithTree = true;
                                    break;
                                }
                            }
                            if (collidingWithTree) continue;

                            GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                            GameObject tree = Instantiate(prefab, treeManager.transform);
                            tree.transform.position = new(x - size / 2, y - size / 2, 0);
                            treePositions.Add(tree.transform.position);
                        }
                    }
                }
            }
        }

        foreach (Transform tree in treeManager.transform) tree.position += new Vector3(0.5f, 1f, 0f);
    }

    public void AddSoilPlot(int x, int y) {
        if (soilGrid[x, y].IsType(SoilCell.Type.None)) {
            soilTilemap.SetTile(new Vector3Int(x, y), soilPlotTiles[Random.Range(0,4)]);
            soilGrid[x, y].type = SoilCell.Type.Soil;
        }
    }

    private void UpdatePlants() {
        for (int y = 1; y < size - 1; y++) {
            for (int x = 1; x < size - 1; x++) {
                bool plantIsGrowing = plantGrid[x, y].growthStage > 0 && plantGrid[x, y].growthStage < 5;
                int randNum = Random.Range(1, randomTickRate + 1);
                if (plantIsGrowing && randNum == 1) plantGrid[x, y].GrowPlant();
            }
        }
    }

    public static bool CompareBoolArrays(bool[] a, bool[] b) {
        if (a.Length != b.Length) return false;
        for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;
        return true;
    }

    public bool CheckCellAtPos(Cell[,] grid, int x, int y, Cell.Type type) { return grid[x, y].IsType(type); }

    public void PlantSeedAtPos(int x, int y, string spriteName) { plantGrid[x, y].PlantSeed(spriteName); }
    public void HarvestPlantAtPos(int x, int y) { plantGrid[x, y].HarvestPlant(); }


    public Vector3Int WorldPosToCellPos(Vector3 worldPos) {
        int cellPosX; if (worldPos.x >= 0) cellPosX = (int)worldPos.x; else cellPosX = (int)worldPos.x - 1;
        int cellPosY; if (worldPos.y >= 0) cellPosY = (int)worldPos.y; else cellPosY = (int)worldPos.y - 1;
        return new(cellPosX + size/2, cellPosY + size/2, 0);
    }

    [ContextMenu("Debug All Ground Cells")]
    public void DebugAllGroundCells()
    {
        foreach (GroundCell groundCell in groundGrid)
        {
            Debug.Log(groundCell.GetInfo());
        }
    }
}

//Make a super class called Cell, make the other cells inherit Cell class, and win?
public class Cell {
    public enum Type {
        None, Land, Water, UpperEdge, LowerEdge, LeftEdge, RightEdge, UpperLeftCorner, LowerLeftCorner, UpperRightCorner, LowerRightCorner, UpperLeftInverseCorner, LowerLeftInverseCorner, UpperRightInverseCorner, LowerRightInverseCorner, UpperLeftAndLowerRightCorner, LowerLeftAndUpperRightCorner,
        Soil,
        Temp
    }

    public Type type;
    public (int, int) position;

    public Cell() { type = Type.None; }
    public Cell(Type type) { this.type = type; }

    public bool IsType(Type referenceType) => type == referenceType;

    //Returns an array of bools representing all nearby cells and whether they are the type that the parameter method determines. (STARTS FROM THE UPPER-LEFT, MOVES LEFT TO RIGHT, DESCENDING ROWS, EXCLUDING CENTER)
    public bool[] NearbyTileTypes(Cell[,] grid, Type type) {
        bool[] nearbyTileTypes = new bool[8];
        nearbyTileTypes[0] = grid[position.Item1 - 1, position.Item2 + 1].type == type;
        nearbyTileTypes[1] = grid[position.Item1 + 0, position.Item2 + 1].type == type;
        nearbyTileTypes[2] = grid[position.Item1 + 1, position.Item2 + 1].type == type;
        nearbyTileTypes[3] = grid[position.Item1 - 1, position.Item2 + 0].type == type;
        nearbyTileTypes[4] = grid[position.Item1 + 1, position.Item2 + 0].type == type;
        nearbyTileTypes[5] = grid[position.Item1 - 1, position.Item2 - 1].type == type;
        nearbyTileTypes[6] = grid[position.Item1 + 0, position.Item2 - 1].type == type;
        nearbyTileTypes[7] = grid[position.Item1 + 1, position.Item2 - 1].type == type;
        return nearbyTileTypes;
    }

    //Returns if all nearby tiles are of a certain type, or if any nearby tiles are of a certain type, based on the default bool parameter checkAllTiles.
    public bool NearbyTileTypeDetected(Cell[,] grid, Type type, bool checkAllTiles = false) {
        if (checkAllTiles) return WorldHandler.CompareBoolArrays(NearbyTileTypes(grid, type), new bool[] { true, true, true, true, true, true, true, true });
        return !WorldHandler.CompareBoolArrays(NearbyTileTypes(grid, type), new bool[] { false, false, false, false, false, false, false, false });
    }

    public string GetInfo()
    {
        (int, int) pos = position; pos.Item1 -= 50; pos.Item2 -= 50;
        return "Cell Position: " + pos.ToString() + "\nCell Type: " + type.ToString();
    }
}

public class GroundCell : Cell {
    public GroundCell() : base(Type.Land) {}
    public GroundCell(Type type) : base(type) {}
}

public class SoilCell : Cell {
    public SoilCell() : base(Type.None) { }
    public SoilCell(Type type) : base(type) { }
}

public class PlantCell : Cell {
    public int growthStage = 0;
    public string spriteName = null;

    public PlantCell() : base(Type.None) { }
    public PlantCell(Type type) : base(type) { }

    public void PlantSeed(string spriteName) {
        if (growthStage != 0) return;
        spriteName = spriteName.Substring(0, 10) + (int.Parse(spriteName[10..]) + 8).ToString();
        WorldHandler.instance.plantsTilemap.SetTile(new Vector3Int(position.Item1, position.Item2), AssetFinder.FindTile(spriteName));
        growthStage++; this.spriteName = spriteName; type = Type.Temp;
    }

    public void GrowPlant() {
        if (growthStage <= 0 || growthStage >= 5) return;
        spriteName = spriteName.Substring(0, 10) + (int.Parse(spriteName[10..]) + 1).ToString();
        WorldHandler.instance.plantsTilemap.SetTile(new Vector3Int(position.Item1, position.Item2), AssetFinder.FindTile(spriteName));
        growthStage++;
    }

    public void HarvestPlant() {
        if (growthStage != 5) return;
        string sriteNameForHarvest = "item_carry_" + (int.Parse(spriteName[10..]) + 88).ToString();
        PlayerHandler.instance.inventory.AddItem(AssetFinder.FindPrefabBySpriteName(sriteNameForHarvest).GetComponent<Collectable>());
        WorldHandler.instance.plantsTilemap.SetTile(new Vector3Int(position.Item1, position.Item2), null);
        growthStage = 0; this.spriteName = null; type = Type.None;
    }
}