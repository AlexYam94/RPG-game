using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Resources;
using UnityEngine.AI;
using System;

namespace RPG.Control
{

    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        CursorMapping[] cursorMappings = null;

        Health health;

        enum CursorType{
            None,
            Movement,
            Combat
        }

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
            if(health.IsDead()) return;
            if (!InteractWithCombat()){
                // print("interact with combat return false");
                if (!InteractWithMovement()){
                    // print("interact with movement return false");
                    SetCursor(CursorType.None);
                    return;
                }
            }
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;
                if (target == null && !GetComponent<Fighter>().CanAttack(target.gameObject))
                    continue;
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                SetCursor(CursorType.Combat);
                return true;
            }
            return false;
        }


        private bool InteractWithMovement()
        {
            return MoveToCursor();
        }

        private bool MoveToCursor()
        {
            RaycastHit hitInfo;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hitInfo);
            if (hasHit)
            {
            
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hitInfo.point, 1f);
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
    }
}









