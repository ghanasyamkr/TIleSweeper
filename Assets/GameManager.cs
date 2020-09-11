using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    
    [SerializeField] GameObject Tile,GameOverPanel;
    [SerializeField] int GridSize,mineCount,maxAttempts=20;
    [SerializeField] float spacing;
    [SerializeField] List<tileData> tiles;
    [SerializeField] GameObject startPos;
    [SerializeField] int proximity = 2;
    public List<Material> tileSkins;
    int selectedTile = 0,prevTileType, attempts;
    bool isGameover;
    List<int> mineTiles;
    private void Awake()
    {
        if (inst==null)
        {
            inst = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
        tiles = new List<tileData>();
        mineTiles = new List<int>();
        createGrid(GridSize);
        placeMines(mineCount);
       
    }
    void reload()
    {
        foreach (tileData item in tiles)
        {
            item.reload();
        }
        mineTiles.Clear();
        placeMines(mineCount);
        prevTileType = 0;
        attempts = 0;
        selectedTile = 0;
        GameOverPanel.SetActive(false);
        isGameover = false;
        keyboardPos = Vector2.zero;
    }
    
    void createGrid(int size)
    {
        Vector2 nextPos = startPos.transform.position;
        GameObject newTile;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
            newTile = Instantiate(Tile);
            newTile.transform.position = nextPos;
            tiles.Add(newTile.GetComponent<tileData>());
            tiles[i * size + j].setTile(i * size + j,new Vector2(j,i));
            nextPos.x += spacing;

            }
            nextPos.y += spacing;
            nextPos.x -= size * spacing;
        }
    }
    void placeMines(int count)
    {
        GameObject dummy= new GameObject();       
        for (int i = 0; i < count; i++)
        {
            int newPick = Random.Range(0, tiles.Count);
            if (!mineTiles.Contains(newPick))
            {
                mineTiles.Add(newPick);
            }
            else
            {
                i -= 1;
                continue;
            }
        }
        foreach (int item in mineTiles)
        {
            tiles[item].addDummy(dummy);
        }
    }
    Vector2 clickPos;
    void onTileSelect(int id)
    {
        if (tiles[id].type!=0)
        {
            return;
        }
        attempts++;
        if (id == -1)
        {
            return;
        }
        checkProximity(id);
        if (attempts >= maxAttempts)
        {
            isGameover = true;
            GameOverPanel.SetActive(true);
            foreach (int item in mineTiles)
            {
                tiles[item].changeType(1);
            }
        }
    }
    void checkProximity(int id)  //function to check if tile is mine or if there is any mine in range
    {
        if (tiles[id].dummy!=null) // usage of dummy can be avoided and the mines list can be used. Done this according to requirement
        {
            Debug.Log("Mine!!!");
            tiles[id].changeType(1);
            return;
        }
        Vector2 cords=tiles[id].coords;
        for (int i = -proximity-1; i < proximity; i++) //iteration from left to right and bottom to top 
        {
            if (cords.x - (i + 1)<0|| cords.x - (i + 1)>GridSize-1)// to detect if selected tile is in the edge of the grid
            {
                Debug.Log("Edge");
                
            }

           else if (tiles[getIdFormCoords(new Vector2(cords.x - (i + 1), cords.y))].dummy != null) //to check for nearby mines left and right
            {
                tiles[id].changeType(2);
                break;
            }
            if (cords.y - (i + 1)<0|| cords.y - (i + 1)>GridSize-1)
            {
                Debug.Log("Edge");
               
            }
           else  if (tiles[getIdFormCoords(new Vector2(cords.x, cords.y - (i + 1)))].dummy != null)//to check for nearby mines bottom and top
            {
                tiles[id].changeType(2);
                break;
            }
            else
            {        
                tiles[id].changeType(3);
                
            }
        }
    }
    int getIdFormCoords(Vector2 cds)  //convert row and coloumn index of a tile into index of the tile in tiles list
    {
        if (cds.x<0||cds.y<0||cds.x>GridSize-1||cds.y>GridSize-1)
        {
            Debug.Log("edgeDetected");
            return -1;
        }
        return Mathf.FloorToInt(GridSize * cds.y + cds.x);
    }
   int getGridIdFromPos(Vector2 pos) //convert mouse Click coordinates of a tile into index of the tile in tiles list {this method helps to avoid the dependency of colliders on tiles}
    {
        pos.x -= startPos.transform.position.x+ Tile.transform.localScale.x / 2+spacing/4;
        pos.y -= startPos.transform.position.y + Tile.transform.localScale.y / 2 + spacing / 4;
        pos /= spacing;
        int row = (Mathf.FloorToInt(pos.x + Tile.transform.localScale.x / 2))+1;
        int coloumn = (Mathf.FloorToInt(pos.y + Tile.transform.localScale.y / 2))+1;
        if (row < 0 || row > GridSize - 1 || coloumn < 0 || coloumn> GridSize - 1)
        {
            return-1;
        }
        int gridId = (coloumn * GridSize) + row;
        return gridId;
    }
    Vector2 keyboardPos;
    #region INPUT_PROCESSING
    void mouseClickDetection()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isGameover)
            {
                return;
            }
            tiles[selectedTile].highlight(false);
            clickPos =Camera.main.ScreenToWorldPoint(Input.mousePosition);
           onTileSelect(getGridIdFromPos(clickPos));
        }
    }
    void keyBoardDetection()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            reload();
            return;
        }
        if (isGameover)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            onTileSelect(selectedTile);
            //prevTileType= tiles[selectedTile].type;
            return;
        }
        if (Input.anyKeyDown)
        {
            if (Input.GetAxis("Horizontal")>0)
            {
                if (keyboardPos.x < GridSize-1)
                {
                    keyboardPos.x += 1;

                }

            }
           else if (Input.GetAxis("Horizontal")<0)
            {
                if (keyboardPos.x > 0)
                {
                    keyboardPos.x -= 1;

                }

            }
          else  if (Input.GetAxis("Vertical") >0)
            {
                if (keyboardPos.y < GridSize-1)
                {
                    keyboardPos.y+= 1;

                }

            }
           else if (Input.GetAxis("Vertical") <0)
            {
                if (keyboardPos.y>0)
                {
                    keyboardPos.y-= 1;

                }

            }
            tiles[selectedTile].highlight(false);
            selectedTile=getIdFormCoords(keyboardPos);
            prevTileType = tiles[selectedTile].type;
            tiles[selectedTile].highlight(true);
        }
        
    }
    #endregion INPUT_PROCESSING
    void Update()
    {
        keyBoardDetection();
        mouseClickDetection();
    }
}
