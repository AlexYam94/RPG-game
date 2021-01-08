using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeTransparent : MonoBehaviour
{
    private Camera mainCamera = null;
    private GameObject player = null;
    private List<RaycastHit> targets = new List<RaycastHit>();
    private int layerMask = 1 << 9;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(mainCamera.transform.position, player.transform.position-mainCamera.transform.position,Color.red);
        RaycastHit[] hits = Physics.RaycastAll(transform.position, player.transform.position-mainCamera.transform.position, Vector3.Distance(player.transform.position, mainCamera.transform.position), layerMask);
        if(hits.Length>0){
            int count = targets.Count/2;
            for(int i=0;i<count;i++){
                targets[i].collider.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            }
            for(int i = 0; i <count;i++){
                targets.RemoveAt(i);
            }
        }
        foreach (RaycastHit hit in hits)
        {
            HideObject(hit);
            // if(!targets.Contains(hit))
                targets.Add(hit);
        }
        if(hits.Length==0){
            if(hits.Length == 0){
                foreach(RaycastHit hit in targets){
                    hit.collider.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
                }
                targets.Clear();
            }
        }
        // else if(targets.Count >= 10){
        //     print("cp2");
        //     print(targets.Count);
        //     int count = targets.Count/2;
        //     for(int i=0;i<count;i++){
        //         targets[i].collider.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        //     }
        //     for(int i = 0; i <count;i++){
        //         targets.RemoveAt(i);
        //     }

        // }
    }

    private static void HideObject(RaycastHit hit)
    {
        hit.collider.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

}
