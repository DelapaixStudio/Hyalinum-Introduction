using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


namespace UnityStandardAssets.Cameras
{
    public class FreeLookCam : PivotBasedCameraRig
    {
        // CAMERA 3EME PERSONNE
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        // 		Pivot
        // 			Camera


        [Header("Camera")]
        [SerializeField] private float m_MoveSpeed = 1f;                      // How fast the rig will move to keep up with the target's position.
        [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
        [SerializeField] private float m_TurnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
        [SerializeField] private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
        [SerializeField] private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.


        private float m_LookAngle;                    // The rig's y axis rotation.
        private float m_TiltAngle;                    // The pivot's x axis rotation.
        private const float k_LookDistance = 100f;    // How far in front of the pivot the character's look target is.
		private Vector3 m_PivotEulers;
		private Quaternion m_PivotTargetRot;
		private Quaternion m_TransformTargetRot;
                           

        private float desiredRot; // valeur Rotation y

        public bool _lookAtSomething = false;
        [SerializeField]
        private Transform _targetSomething;
        public bool LookAtPlayer;
        public float rotSpeed_Lat; // Force Rot Horizontale.
        [Range(0f, 5f)]
        public float rotSpeed_Verticale; // Force                   
        public float damping; // Lerp Vitesse Rotation Horizontale (y)

        public Vector3 LocalPosCamera; // Position caméra début.
        private Vector3 Pos; // Position du joueur pour vérifier mouvement.

        private bool isMoving;
        private bool _previouslyMoving;
        

        [Header("Collision Camera")]
        private Vector3 desiredPosition; // Vector 3 position Ensemble caméra.        
        private bool CamTooClose;
        public float MinHeightCam;
        
        [SerializeField]
        private float HauteurCam;
        [SerializeField]
        private float _HauteurSpeed;
        [SerializeField]
        private float intervalCam;              
        
        // Detections Raycast
        private int layerMask;  
        private bool _LatCol; 
        private bool _ForwardCol;

        public float DistRaycast = 1f;
        public float DistRaycast_Lat = 1f;
        private float _MesureRaycast;
        RaycastHit hit;
        RaycastHit hit_Lat;
        RaycastHit hit_Forward;
        private SetPosToGround _ObjectToGround = new SetPosToGround();

        [SerializeField]
        private Transform _heightCalculObject;



        protected override void Awake()
        {
            base.Awake();
            Cursor.visible = false;
			m_PivotEulers = m_Pivot.rotation.eulerAngles;

	        m_PivotTargetRot = m_Pivot.transform.localRotation;
			m_TransformTargetRot = transform.localRotation;

            m_Cam.localPosition = LocalPosCamera;
            
            transform.position  += new Vector3 (0f, HauteurCam, 0f); // Empeche la camera de passer sous le sol au démarrage.
           
            layerMask = LayerMask.GetMask("Ground");         
        }    
       

        private void Update()
        {
            if (_lookAtSomething)
            {
               // m_Cam.transform.rotation = Quaternion.Slerp(m_Cam.transform.rotation, Quaternion.LookRotation(_targetSomething.position, _targetSomething.position), 1f / 5f);
                m_Cam.transform.LookAt(_targetSomething.position);
                return;
            }

            HandleRotationMovement();
            float dist = 100 * Vector3.Distance(transform.position, Pos);
            if (dist > 1f) isMoving = true;
            else
            {
                // Ne bouge pas.
                // Audio.STOP à ajouter
                isMoving = false;
                _previouslyMoving = false;
            }
            if (isMoving && !_previouslyMoving) // Si il bouge et qu'il ne bougeait pas avant.
            {
                _previouslyMoving = true;
            }

            Pos = transform.position; // = Last Position.
        }

        /*

        void FixedUpdate()
        {          
            if (isMoving) // Prevoit col mouvement vers l avant
            {
                #region RaycastLatCol
                Vector3 dir = m_Cam.transform.forward;
                dir.y = 0;
                Debug.DrawRay(m_Cam.position, dir * DistRaycast_Lat, Color.cyan);
                if (Physics.Raycast(m_Cam.position, dir, out hit_Forward, DistRaycast_Lat, layerMask))
                {
                   // _LatCol = true;
                }
                #endregion                              
            }           
        }
             */ //FixedUpdate          

        // Follow Target
        void LateUpdate()
        {

            if (_lookAtSomething)
            {
                return;
            }
            
            Transform t = _heightCalculObject;
            t.position = m_Cam.position;
            t.position += (Vector3.up * 1000f);
                        
            if (Physics.Raycast(origin: t.position, -t.up, out hit, Mathf.Infinity, layerMask))
            {
                t.position -= new Vector3(0f, hit.distance - HauteurCam, 0f);  /// On soustrait la distance qui le separe du sol.
                t.position += new Vector3(0f, HauteurCam, 0f);  
            }
            
            // CALCUL POSITION CAM  => Déplace le gameobject parent vers la position du joueur
                       

            float xPos = Mathf.Lerp(transform.position.x, m_Target.position.x, 0.75f * Time.deltaTime);
            float zPos = Mathf.Lerp(transform.position.z, m_Target.position.z, 2f * Time.deltaTime);
                                   

            desiredPosition = new Vector3( xPos, 0f, zPos); // On fait une transition sur la position latérale pour éviter les tremblements.
            
                                    
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, _heightCalculObject.position, 2f * Time.deltaTime);
           
            transform.position = new Vector3(m_Target.position.x, t.position.y, m_Target.position.z);           
           
            if (LookAtPlayer) {m_Cam.transform.LookAt(m_Target);}                                  
        }

        void RaycastsLaterals(float x)
        {            
            // Positif à gauche  et négatif à droite
            // DistRaycast_Lat *= x;
            float DistDiag = DistRaycast_Lat / 1.7f;

            if(x > 0)  // Rotation vers la GAUCHE.
            {
                #region Raycast
                Debug.DrawRay(m_Cam.position, -m_Cam.transform.right * DistRaycast_Lat, Color.green);

                if (Physics.Raycast(m_Cam.position, -m_Cam.transform.right, out hit_Lat, DistRaycast_Lat, layerMask))
                {
                    _LatCol = true;
                }

                //Raycast Diagonale
                Vector3 Diag = -m_Cam.transform.right + m_Cam.transform.forward;
                Diag.y = 0f;
                Debug.DrawRay(m_Cam.position, Diag * DistDiag, Color.red);
                if (Physics.Raycast(m_Cam.position, Diag, out hit_Lat, DistDiag, layerMask))
                {
                    _LatCol = true;
                }
                               
                #endregion
            }

            if (x < 0) // Rotation vers la DROITE.
            {
                #region Raycast
                Debug.DrawRay(m_Cam.position, m_Cam.transform.right * DistRaycast_Lat, Color.green);

                if (Physics.Raycast(m_Cam.position, m_Cam.transform.right, out hit_Lat, DistRaycast_Lat, layerMask))
                {
                    _LatCol = true;
                }


                // Raycast Diagonale.
                Vector3 Diag = m_Cam.transform.right + m_Cam.transform.forward;
                Diag.y = 0f;
                Debug.DrawRay(m_Cam.position, Diag * DistDiag, Color.red);
                if (Physics.Raycast(m_Cam.position, Diag, out hit_Lat, DistDiag, layerMask))
                {
                    _LatCol = true;
                }

                #endregion
            }
        }
        
        private void HandleRotationMovement()
        {
            if (Time.timeScale < float.Epsilon)
                return;

            // Read the user input
            var x = CrossPlatformInputManager.GetAxis("Mouse X");
            var y = CrossPlatformInputManager.GetAxis("Mouse Y");

            //  if (x != 0) RaycastsLaterals(x); // On créer des Raycast sur les côtés pour eviter collision  
            

            
            //               *** VERTICALE ***

            // Y est positif vers le haut
            // Négatif vers le bas
            if (y != 0)
            {
                // On monte
                if (y > 0) // y est négatif quand on monte vers le haut
                {     //if (-m_TiltMax < m_TiltAngle) m_TiltAngle -= _rotFactorCam;
                    m_TiltAngle = Mathf.Lerp(m_TiltAngle, -m_TiltMax, Time.deltaTime * rotSpeed_Verticale); // angle max est négatif car je souhaite incliner la caméra vers le haut, x négatif
                }
                if (y < 0)
                // On descend
                {                         ///if (m_TiltMin > m_TiltAngle) m_TiltAngle += _rotFactorCam;      
                    m_TiltAngle = Mathf.Lerp(m_TiltAngle, m_TiltMin, Time.deltaTime * rotSpeed_Verticale * 100f);
                }
            }

            var _camRot = Quaternion.Euler(m_TiltAngle, m_Cam.transform.eulerAngles.y, m_Cam.transform.eulerAngles.z);
            m_Cam.transform.rotation = Quaternion.Lerp(m_Cam.transform.rotation, _camRot, Time.deltaTime);
                       
            //              *** HORIZONTALE ***

            if (x > 0.2) desiredRot += rotSpeed_Lat;
            if (x < -0.2) desiredRot -= rotSpeed_Lat;

            var desiredRotQ = Quaternion.Euler(m_Pivot.transform.eulerAngles.x, desiredRot, m_Pivot.transform.eulerAngles.z);

            m_Pivot.transform.rotation = Quaternion.Lerp(m_Pivot.transform.rotation, desiredRotQ, Time.deltaTime * damping);
            return;

            // Fait tourner le pivot
        }      
    }
   
    
}
