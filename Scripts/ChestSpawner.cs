using UnityEngine;
using UnityEngine.Tilemaps;

public class ChestSpawner : MonoBehaviour
{
    private Tilemap floorTilemap;
    private Tilemap objectsTilemap;
    public GameObject treasureChestPrefab;
    public int numOfChests = 5;

    void Start()
    {

        floorTilemap = GameObject.FindGameObjectWithTag("FloorMap").GetComponent<Tilemap>();
        objectsTilemap = GameObject.FindGameObjectWithTag("ObjectMap").GetComponent<Tilemap>();
        PlaceHealthPotions();
    }

    void PlaceHealthPotions()
    {
        if (floorTilemap != null)
        {
            // Assuming "Objects" is the name of the tilemap where you don't want potions to spawn

            int tileCount = floorTilemap.GetUsedTilesCount();
            if (tileCount > 0)
            {
                BoundsInt bounds = floorTilemap.cellBounds;
                TileBase[] allTiles = floorTilemap.GetTilesBlock(bounds);
                int potionsPlaced = 0;
                while (potionsPlaced != numOfChests)
                {
                    Vector3Int randomPos = new Vector3Int(
                        Random.Range(bounds.min.x, bounds.max.x),
                        Random.Range(bounds.min.y, bounds.max.y),
                        0
                    );

                    TileBase tile = allTiles[randomPos.x - bounds.x + (randomPos.y - bounds.y) * bounds.size.x];

                    if (tile != null && objectsTilemap != null)
                    {
                        Vector3 potionPos = floorTilemap.GetCellCenterWorld(randomPos);
                        Vector3Int tilePosInObjectsMap = objectsTilemap.WorldToCell(potionPos);
                        TileBase objectTile = objectsTilemap.GetTile(tilePosInObjectsMap);

                        if (objectTile == null)
                        {
                            Instantiate(treasureChestPrefab, potionPos, Quaternion.identity);
                            potionsPlaced += 1;
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("Tilemap does not contain any tiles. Add tiles to the Tilemap.");
            }
        }
    }
}
