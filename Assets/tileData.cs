using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileData : MonoBehaviour
{
    public int id;
    public GameObject dummy;
    public Vector2 coords;
    public int type;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void reload()
    {
        changeType(0);
        dummy = null;
    }
    public void setTile(int l_id,Vector2 cds)
    {
        id = l_id;
        coords = cds;
        

    }
    public void highlight(bool state)
    {
        if (state)
        {
            GetComponent<MeshRenderer>().material = GameManager.inst.tileSkins[4]; //highlight colour
            return;
        }

        changeType(type);
    }
    public void changeType(int l_type)
    {
        type = l_type;
        Material mat=null;
        GameManager gm = GameManager.inst;
        switch (l_type)
        { 
            case 0:
                mat = gm.tileSkins[0];
                    break;
            case 1:
                mat = gm.tileSkins[1]; //red
                break;
            case 2:
                mat = gm.tileSkins[2];//yellow
                break;
            case 3:
                mat = gm.tileSkins[3]; //Green
                break;
        }
        GetComponent<MeshRenderer>().material =mat;
    }
    public void addDummy(GameObject dumdum)
    {
        dummy = dumdum;
       // GetComponent<SpriteRenderer>().color = Color.cyan;
    } 
    // Update is called once per frame
    void Update()
    {
        
    }
}
