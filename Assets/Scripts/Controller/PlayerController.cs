using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using RPG.Attributes;
using RPG.Tool;
using UnityEngine.AI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace RPG.Control
{

    public class PlayerController : MonoBehaviour, ICharacter
    {
        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 1f;
        [SerializeField][Range(0,1)] float rotateSpeed = 0.1f;
        [SerializeField][Range(0,100)] float stopSpeed = 20f;
        [SerializeField][Range(0,10)] float moveSpeed =2.5f;
        [SerializeField][Range(0,10)] float blockingMoveSpeed =1f;
        [SerializeField][Range(10,30)] float rollSpeed =10f;
        [SerializeField][Range(1,2)] float sprintMultiplier = 1.4f;
        [SerializeField] UnityEvent walkEvent;
        [SerializeField] UnityEvent onAttack;
        [SerializeField] float gravityTemp = 10f;
        [SerializeField] float toGroundDistance = 1f;
        [SerializeField] float maxTimeGapBetweenAttack = 0f;


        Health health;
        Animator anim = null;
        int layerMask = 0x01ff;
        Fighter fighter = null;
        float attackTime = 0f;
        NavMeshAgent agent = null;
        Rigidbody myRigidBody = null;
        Vector3 moveInput;
        Vector3 moveVelocity;
        [SerializeField] bool isAttacking = false;       //set in attack animation
        private bool isRolling = false;
        private float maxForwardSpeed = 6f;
        float minForwardSpeed = 6f;
        private float walkVoiceInterval = 2.426f;
        float nextWalkVoice = 0f;
        Stamina stamina = null;
        bool canSprint = true;
        bool canRoll = true;
        bool canMove = true;
        bool isBlocking = false;
        float rollTime = 0f;
        float moveAnimationSpeedMultiplier = 1.2f;
        float walkSpeed =2.5f;
        float gravity = 0;
        float timeSinceAttack = Mathf.Infinity;
        int numberOfClicks = 0;
        bool isOverrideRotation;
        Vector3 overrideRotation;

        // Start is called before the first frame update
        void Start()
        {
            anim = GetComponent<Animator>();
            health = GetComponent<Health>();
            fighter = GetComponent<Fighter>();
            agent = GetComponent<NavMeshAgent>();
            myRigidBody = GetComponent<Rigidbody>();
            stamina = GetComponent<Stamina>();
            // this.gameObject.GetComponent<NavMeshAgent>().enabled=true;
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
            DrawMousePos();
            // anim.SetFloat("movingSpeed",5f);
            // print("Horizontal: " + Input.GetAxisRaw("Horizontal"));
            // print("Vertical: " + Input.GetAxisRaw("Vertical"));

            // if(Input.GetKeyDown(KeyCode.Escape)){
            //     Canvas canvas = GameObject.Find("Menu").GetComponent<Canvas>();
            //     canvas.enabled = !canvas.enabled;
            // }
            if (!IsGrounded())
            {
                gravity = gravityTemp;
            }
            else
            {
                gravity = 0f;
            }
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
            // if (InteractWithUI())
            // {
            //     return;
            // }
            // if (health.IsDead())
            // {
            //     SetCursor(CursorType.None);
            //     return;
            // }
            // if (InteractWithComponent())
            // {
            //     return;
            // }
            // if (!InteractWithMovement())
            // {
            //     SetCursor(CursorType.None);
            //     return;
            // }

        }

        void DrawMousePos(){
            // Vector3 mousePos = Input.mousePosition;
            // mousePos.z = transform.position.y;
            // Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);
            // pos.y = transform.position.y;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit,100f);
            // Debug.DrawRay(Camera.main.transform.position, ray.direction*10, Color.red);
            Debug.DrawRay(Camera.main.transform.position, (hit.point-Camera.main.transform.position)*20, Color.red);
            
            // Debug.DrawRay (GetMouseRay().origin, GetMouseRay().direction * 500, Color.red);
            // if(!Physics.Raycast(GetMouseRay(), out hit,100f, layerMask)) return false;
        }

        void FixedUpdate()
        {
            if (health.IsDead()) return;
            HandleInputs();

            OverrideRotation();
            if (canMove && !isRolling && !isOverrideRotation)
            {
                HandleMovement();
                myRigidBody.AddRelativeForce(Vector3.down * gravity);
                maxForwardSpeed = Mathf.Clamp(maxForwardSpeed - Time.deltaTime * stopSpeed, minForwardSpeed, 10);
            }

            rollTime = Mathf.Max(rollTime - Time.deltaTime, 0);
            if (rollTime == 0)
            {
                canRoll = true;
            }

            // print("maxForwardSpeed: " + maxForwardSpeed); 
        }

        private void OverrideRotation()
        {
            if (isOverrideRotation)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(overrideRotation), rotateSpeed);
            }
            if (!isAttacking &&( Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
            {
                isOverrideRotation = false;
            }
        }

        private void HandleInputs()
        {
            if(Input.GetKeyDown(KeyCode.E)){
                PickupManager.Pickup();
            }
            if (!EventSystem.current.IsPointerOverGameObject()&&!isRolling && Input.GetMouseButtonDown(0) && !isAttacking && stamina.HasStaminaLeft())
            {
                HandleAttacking();
            }
            else if (canMove && canRoll && Input.GetKeyDown(KeyCode.Space) && !isAttacking && !isRolling && stamina.HasStaminaLeft())
            {
                HandleRolling();
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                HandleSprinting();
            }
            if (Input.GetMouseButton(1) && !isAttacking)
            {
                isBlocking = true;
                moveAnimationSpeedMultiplier = 1.2f;
            }
            else
            {
                isBlocking = false;
            }
        }

        private void HandleSprinting()
        {
            anim.SetFloat("sprintMultiplier", 1);
            minForwardSpeed = 6f;
        }

        private void HandleRolling()
        {
            anim.SetTrigger("roll");
            canRoll = false;
            StartCoroutine("StartRoll");
            rollTime = Util.GetCurrentAnimationTime(rollTime, "roll", anim) * .7f;
            stamina.ConsumeStaminaOnce(15f, Stamina.StaminaType.roll);
            moveVelocity = (this.transform.forward) * rollSpeed;
            Debug.Log("x: " + myRigidBody.velocity.x + " z: " + myRigidBody.velocity.z);
            // myRigidBody.AddForce(this.transform.forward * rollSpeed * rollMultiplier);
            moveVelocity.y = 0;
            myRigidBody.velocity = moveVelocity;
        }

        private void HandleAttacking()
        {
            onAttack.Invoke();
            if(!isBlocking){
                fighter.Attack(null);
            }else{
                fighter.SpecialAttack();
                // print("test");
                // anim.SetBool("test",true);
                // stamina.ConsumeStaminaOnce(25f, Stamina.StaminaType.attack);
                // SpecialAttack();
            }
        }

        private void HandleBlocking()
        {
            if(isBlocking){
                minForwardSpeed = 3f;
                walkSpeed = blockingMoveSpeed;
            }else{
                minForwardSpeed = 6f;
                walkSpeed = moveSpeed;
            }
            anim.SetBool("isBlocking", isBlocking);
        }

        private void HandleMovement()
        {
            if (health.IsDead()) return;
            if (!IsGrounded())
            {
                gravity = gravityTemp;
            }
            else
            {
                gravity = 0f;
            }
            float x = Input.GetAxisRaw("Vertical") == 0 ? Input.GetAxisRaw("Horizontal") * 1.5f : Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Horizontal") == 0 ? Input.GetAxisRaw("Vertical") * 1.5f : Input.GetAxisRaw("Vertical");
            // float x = Input.GetAxisRaw("Vertical");
            // float z = Input.GetAxisRaw("Horizontal");

            moveInput = Vector3.zero;
            moveInput.x = x;
            moveInput.z = z;

            // Vector3 camDir = Camera.main.transform.rotation * moveInput;
            Vector3 targetDirection = GetFacingDirection();

            // if (moveInput != Vector3.zero)
            // {
                // transform.rotation = Quaternion.Slerp(
                //     transform.rotation, 
                //     Quaternion.LookRotation(targetDirection),
                //     Time.deltaTime * rotateSpeed
                // );
            // }
            // moveVelocity = moveInput * moveSpeed;
            walkSpeed = moveSpeed;
            HandleBlocking();
            moveInput = targetDirection.normalized;
            moveVelocity = moveInput * walkSpeed;
            moveAnimationSpeedMultiplier = 1.2f;

            if (x != 0 || z != 0)
            {
                // anim.SetBool("isMoving", true);
                anim.SetFloat("forwardSpeed"
                            , Mathf.Clamp(anim.GetFloat("forwardSpeed")
                                + Mathf.Abs(Input.GetAxisRaw("Horizontal"))
                                + Mathf.Abs(Input.GetAxisRaw("Vertical"))
                            , 0
                            , maxForwardSpeed));
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(moveInput), rotateSpeed);
            }
            else
            {
                // anim.SetBool("isMoving", false);
                anim.SetFloat("forwardSpeed"
                                , Mathf.Clamp(anim.GetFloat("forwardSpeed") - Time.deltaTime * stopSpeed
                                , 0
                                , maxForwardSpeed));
            }
            if (Input.GetKey(KeyCode.LeftShift) && stamina.HasStaminaLeft() && canSprint)
            {
                isBlocking = false;
                moveVelocity = moveVelocity * sprintMultiplier;
                moveAnimationSpeedMultiplier = sprintMultiplier;
                stamina.ConsumeStaminaOnce(.2f, Stamina.StaminaType.sprint);
                maxForwardSpeed = 10f;
                if (Time.time >= nextWalkVoice)
                {

                    walkEvent.Invoke();
                    nextWalkVoice = Time.time + walkVoiceInterval;
                }
            }
            anim.SetFloat("moveAnimationSpeedMultiplier", moveAnimationSpeedMultiplier);
            if (isAttacking)
            {
                anim.SetFloat("forwardSpeed", 0);
                myRigidBody.velocity = new Vector3(0, 0, 0);
            }else{
                myRigidBody.velocity = moveVelocity;
            }
        }

        private Vector3 GetFacingDirection()
        {
            Vector3 camDir = Camera.main.transform.TransformDirection(moveInput);
            Vector3 targetDirection = new Vector3(camDir.x, 0, camDir.z);
            return targetDirection;
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
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
            // Debug.DrawRay (GetMouseRay().origin, GetMouseRay().direction * 50000000, Color.green);
            // if(!Physics.Raycast(GetMouseRay(), out hit)) return false;
            if(!Physics.Raycast(GetMouseRay(), out hit,100f, layerMask)) return false;
            if(!NavMesh.SamplePosition(hit.point, out navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas)){
                return false;
            }
            target = navMeshHit.position;

            // NavMeshPath path = new NavMeshPath();
            // if(!NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path)) return false;
            // if(path.status != NavMeshPathStatus.PathComplete) return false;
            // if(GetPathLength(path) > maxNavPathLength) return false;

            return GetComponent<Mover>().CanMoveTo(target);
        }

        

        private bool MoveToCursor()
        {
            RaycastHit hitInfo;
            // bool hasHit = Physics.Raycast(GetMouseRay(), out hitInfo);
            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);
            if (hasHit)
            {
                if(!GetComponent<Mover>().CanMoveTo(target)) return false;

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
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), raycastRadius);
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


        public void SpecialAttack(){
            anim.ResetTrigger("stopAttack");
            anim.SetTrigger("attack1");
        }
        
        // public IEnumerator StartAttack(){
        //     // fighter.EnableTrigger();
        //     anim.ResetTrigger("stopAttack");
        //     anim.SetTrigger("attack");
        //     isAttacking=true;
        //     yield return new WaitForSeconds(attackTime/4*3);
        //     isAttacking=false;
        //     // fighter.DisableTrigger();
        // }

        public IEnumerator StartRoll(){
            isRolling = true;
            yield return new WaitForSeconds(Util.GetCurrentAnimationTime(0,"roll", anim)*0.7f);
            isRolling=false;
        }

        public bool IsBlocking(){
            return isBlocking;
        }

        public void EnableMove(){
            canMove = true;
            canRoll = true;
        }

        public void DisableMove(){
            canMove = false;
            canRoll = false;
        }

        public void StartAttacking(){
            isAttacking = true;
            fighter.StartAttack();
            if(fighter.HasProjectile()){
                
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, 30f);
                Vector3 targetPos = hit.point;
                targetPos.y = transform.position.y;
                fighter.SetShootDirection(targetPos);
                isOverrideRotation = true;
                Vector3 direction = new Vector3(
                    targetPos.x - transform.position.x,
                    0f,
                    targetPos.z - transform.position.z
                );
                overrideRotation = direction;
                // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), rotateSpeed);
            }
        }

        public void FinishAttacking(){
            isAttacking = false;
            fighter.IncreaseMaxNumberOfAttack(1);
        }

        public void EnableInvulnerable(){
            health.SetInvulnerable(true);
        }

        public void DisableInvulnerable(){
            health.SetInvulnerable(false);
        }

        public bool IsGrounded() {
            return Physics.Raycast(transform.position+(Vector3.up), -Vector3.up, toGroundDistance);
        }

        public Health GetHealthComponent()
        {
            return health;
        }

        public Fighter GetFighterComponent()
        {
            return fighter;
        }

        void ICharacter.HandleMovement()
        {
            throw new NotImplementedException();
        }

        public void HandleCombat()
        {
            throw new NotImplementedException();
        }

        public void HandleRotation()
        {
            throw new NotImplementedException();
        }

        void ICharacter.InteractWithComponent()
        {
            throw new NotImplementedException();
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public string GetTag()
        {
            return gameObject.tag;
        }
    }
}









