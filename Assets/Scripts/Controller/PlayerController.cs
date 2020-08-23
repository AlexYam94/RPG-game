using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Resources;
using UnityEngine.AI;
using System;
using UnityEngine.EventSystems;

namespace RPG.Control
{

    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        CursorMapping[] cursorMappings = null;

        [SerializeField]
        float maxNavMeshProjectionDistance = 1f;

        [SerializeField]
        float maxNavPathLength = 40f;

        Health health;

        // Start is called before the first frame update
        void Start()
        {
            health = GetComponent<Health>();
            this.gameObject.GetComponent<NavMeshAgent>().enabled=true;
        }

        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }

        private CursorMapping GetCursorMapping(CursorType type){
            CursorMapping defaultMapping;
            foreach(CursorMapping mapping in cursorMappings){
                if(mapping.type == CursorType.None){
                    defaultMapping = mapping;
                }
                if(mapping.type == type){
                    return mapping;
                }
            }
            return cursorMappings[0];
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space)){
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            if(InteractWithUI()){
                return;
            }
            if(health.IsDead()){
                SetCursor(CursorType.None);
                return;
            }
            if(InteractWithComponent()){
                return;
            }
            if (!InteractWithMovement()){
                SetCursor(CursorType.None);
                return;
            }
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;

        }

        private bool InteractWithUI()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(CursorType.UI);
                return true;
            }
            return false;
        }

        private bool InteractWithMovement()
        {
            return MoveToCursor();
        }

        private bool RaycastNavMesh(out Vector3 target){
            target = new Vector3();            
            RaycastHit hit;
            NavMeshHit navMeshHit;
            if(!Physics.Raycast(GetMouseRay(), out hit)) return false;
            if(!NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas)){
                
                return false;
            }
            target = navMeshHit.position;

            NavMeshPath path = new NavMeshPath();
            if(!NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path)) return false;
            if(path.status != NavMeshPathStatus.PathComplete) return false;
            if(GetPathLength(path) > maxNavPathLength) return false;
            

            return true;
        }

        private float GetPathLength(NavMeshPath path)
        {
            float total = 0;
            Vector3[] corners = path.corners;
            if(corners.Length<2) return total;
            for(int i=0; i<corners.Length-1;i++){
                total += Vector3.Distance(corners[i],corners[i+1]);
            }
            return total;
        }

        private bool MoveToCursor()
        {
            RaycastHit hitInfo;
            // bool hasHit = Physics.Raycast(GetMouseRay(), out hitInfo);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {

                if (Input.GetMouseButton(0))
                {
                    // GetComponent<Mover>().StartMoveAction(hitInfo.point, 1f);
                    GetComponent<Mover>().StartMoveAction(target, 1f);
                }
                SetCursor(CursorType.Movement);
                return true;
            }
            return false;
        }

        private void SetCursor(CursorType type)
        {
            CursorMapping mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        RaycastHit[] RaycastAllSorted(){
            // get all hits
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            float[] distances = new float[hits.Length];
            // sort by distance
            for(int i=0;i<hits.Length;i++){
                distances[i] = hits[i].distance;
            }
            //sort the hits
            Array.Sort(distances, hits);
            //return 
            return hits;
        }
    }
}









