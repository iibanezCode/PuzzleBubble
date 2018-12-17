using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PoolObject : MonoBehaviour {

    public GameObject shurikens;
    public int cantidad;
    private List<GameObject> listaShurikens;
    // Use this for initialization
    void Start() {
        listaShurikens = new List<GameObject>();
        for (int i = 0; i < cantidad; i++) {
            listaShurikens.Add(Instantiate(shurikens));
        }
    }
    public void CrearShurikens(Vector3 pos) {
        GameObject shurikensColocar = listaShurikens[0];
        listaShurikens.RemoveAt(0);
        shurikensColocar.transform.position = pos;
        shurikensColocar.SetActive(true);
    }
    public void AnadirShurikens(GameObject shurikensAnadir) {
        listaShurikens.Add(shurikensAnadir);
    }
}