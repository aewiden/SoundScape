using UnityEngine;
using System.Collections;


public class RDGenerator : MonoBehaviour

{
    public int width = 512;
    public int height = 512;

    public float dA= 1f;

    public float dB =.5f;

    public float f =.055f;

    public float k =.002f;

    private RDCell[,] grid; //multidimensional plane

    private Texture2D texture;


    void Start()
    {
        initTexture();
        initGrid();
    }

    void Update()
    {
        refreshTexture();
        generateNextGrid();
    }


    void initTexture()
    {
        texture = new Texture2D(width, height);
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = texture;

        Debug.Log("Found texture: " + texture);


        for(int i =0; i < width; i++)
        {
            for(int j =0;j< height;j++)
            {
                texture.SetPixel(i, j, Color.green);
            }
        }
        texture.Apply();
    }


    void initGrid()
    {

        grid = new RDCell[width, height];
        Debug.Log("width init : " + texture.width);




        for(int i =0; i <= grid.Length; i++)
        {
            for(int j = 0; j<= grid.Length;j++)
            {
                grid[i, j]= new RDCell(); //not sure about this part
            }
        }

        grid[width/2, height/2].b = 1.0f;
        grid[width/2, (height/2)+1].b = 1.0f;
        grid[(width/2)+1, height/2].b = 1.0f;
        grid[(width/2)+1, (height/2)+1].b = 1.0f;

    }

    void refreshTexture()
    {
        Debug.Log(" texture width : "+ texture.width);
        Debug.Log("width : "+ width);


        for(int i = 0; i <= texture.width; i++)
        {
            for(int j =0; j <=  texture.height; j++)
            {
                Debug.Log("checking for presence of b value");
                Debug.Log(grid[i,j]);
                texture.SetPixel(i, j, Color.Lerp(Color.white, Color.green, 2));


            }
        }
        texture.Apply();
    }

    void generateNextGrid()
    {
        // RDCell[,]nextGrid = new RDCell(width, height);
        // for(int i =0; i < width; i++)
        // {
        //     for(int j =0; j < height; j++)
        //     {
        //         nextGrid[i, j]= new RDCell();
        //     }
        // }
        //
        // Debug.Log("next Grid");
        // for(int i = 1; i <= width-1; i++)
        // {
        //     for(int j = 1; j <= height-1; j++)
        //     {
        //
        //         float a = grid[i, j].a != null? grid[i, j].a : 1.0f;
        //         float b = grid[i, j].b;
        //         Debug.Log(a + " , " + b);
        //         //
        //         nextGrid[i, j].a = a + (dA * LaplaceA(i, j)-(a*b*b)+ (f*(1-a)));
        //         // nextGrid[i, j].b = b + (dB * LaplaceB(i, j)+(a*b*b)- ((k+f)*b));
        //     }
        // }
        //
        // grid = nextGrid;

    }

    float LaplaceA(int x, int y)
    {
        if(x == 1 || x== 0|| x== width|| y == 0|| y ==1||y== height)
        {
          return 0;
        }
        float returnSum = grid[x, y].a * -1f;
        //adjacent cells by .2f
        returnSum += grid[x+1, y].a * .2f;
        returnSum += grid[x, y+1].a * .2f;
        returnSum += grid[x-1, y].a * .2f;
        returnSum += grid[x, y-1].a * .2f;

        //diagonals by .5f

        returnSum += grid[x-1, y-1].a * .5f;
        returnSum += grid[x+1, y+1].a * .5f;
        returnSum += grid[x+1, y-1].a * .5f;
        returnSum += grid[x-1, y+1].a * .5f;

        return returnSum;
    }

    float LaplaceB(int x, int y)
    {
        float returnSum = grid[x, y].b * -1f;
        //adjacent cells by .2f
        returnSum += grid[x+1, y].b * .2f;
        returnSum += grid[x, y+1].b * .2f;
        returnSum += grid[x-1, y].b * .2f;
        returnSum += grid[x, y-1].b * .2f;

        //diagonals by .5f

        returnSum += grid[x-1, y-1].b * .5f;
        returnSum += grid[x+1, y+1].b * .5f;
        returnSum += grid[x+1, y-1].b * .5f;
        returnSum += grid[x-1, y+1].b * .5f;

        return returnSum;
    }


}
