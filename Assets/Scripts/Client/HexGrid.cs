using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {

    private int width = 8;
    private int height = 9;

    public HexCell cellPrefab;

    HexCell[] cells;

    public void PrepareGrid()
    {
        cells = new HexCell[height * width];

        for (int z = 0, i = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                if (z % 2 == 1 && x == width-1)
                {
                    Debug.Log("z-->" + z + "  x-->" + x);
                    continue;
                }
                CreateCell(x, z, i++);
            }
        }
    }

    void CreateCell(int x, int y, int i)
    {
        Vector3 position;
        position.x = (x + y * 0.5f - y / 2) * (HexMetrics.innerRadius * 2f);
        position.y = y * (HexMetrics.outerRadius * 1.5f);
        position.z = 0f;

        HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
    }
}
