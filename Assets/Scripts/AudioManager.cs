using FMOD.Studio;
using FMODUnity;
using System.Collections;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;



public class AudioManager : MonoBehaviour

{

    public static AudioManager AMInstance { get; private set; }



    private void Awake()

    {



        if (AMInstance != null && AMInstance != this)

        {

            Destroy(gameObject);

        }

        else

        {

            AMInstance = this;

            DontDestroyOnLoad(gameObject);

        }



        loopInstance = RuntimeManager.CreateInstance(gameStateLoop);

    }


    [Header("PlayerTransform")]
    public Transform Player;
    [Header("Surfaces")]

    [SerializeField] private LayerMask surface;

    [Header("FootstepSFX")]

    [SerializeField] private EventReference footstepSFX;

    [Header("SwordSwingSFX")]

    [SerializeField] private EventReference swordSwingSFX;

    [Header("DamageImpactSFX")]

    [SerializeField] private EventReference damageImpactSFX;

    [Header("GameStateLoops")]

    [SerializeField] public EventReference gameStateLoop;





    private EventInstance loopInstance;

    

    public string currentState = "Idle";

    public string queuedState = "Idle";



    public float BPM = 120f;

    private int barDurationMS;


    



    private void Start()

    {
        barDurationMS = (int)((60 / BPM) * 4 * 1000);
        loopInstance.start();

        loopInstance.setParameterByNameWithLabel("States", "Idle");
        loopInstance.setParameterByNameWithLabel("prevState", "Idle");
    }



    private void Update()

    {

        if (queuedState != currentState) ;

        {

            int timeLinePos;

            loopInstance.getTimelinePosition(out timeLinePos);



            int barPos = timeLinePos % barDurationMS;



            if (barPos > (barDurationMS - 5))

            {
                
                    StartCoroutine(ApplyChangeState(queuedState));
                
               
                
                currentState = queuedState;
            }

        }

    }

    public void RequestState(string requestedState)

    {

        queuedState = requestedState;

    }


    public IEnumerator ApplyChangeState(string targetState)

    {
       
        loopInstance.setParameterByNameWithLabel("States", targetState);
        
        Debug.Log("Current state is " + currentState);

        yield return new WaitForSeconds(4);

        loopInstance.setParameterByNameWithLabel("prevState", targetState);
        
    }

    public IEnumerator SetTargetStateIdle(string targetState)
    {
        
        //IDLE

        if (currentState != "Idle")
        {

            if (currentState == "Combat")
            {
                yield return new WaitForSeconds(16);

                currentState = "Idle";
            }

            if(currentState == "Explore")
            {
                yield return new WaitForSeconds(4);
                currentState= "Idle";
            }
        }
        
      
    }

    public void DetectSurface(Transform entitiyTransform)

    {

        RaycastHit hit;

        Debug.DrawRay(entitiyTransform.position + Vector3.up, Vector3.down * 1.5f, Color.red);

        if (Physics.Raycast(entitiyTransform.position + Vector3.up, Vector3.down, out hit, 1.5f, surface))

        {

            Terrain hitTerrain = hit.collider.gameObject.GetComponent<Terrain>();

            string surfaceLabel = "Unkown";

            int layer = hit.transform.gameObject.layer;



            if (layer == LayerMask.NameToLayer("Rock")) surfaceLabel = "Rock";

            else if (layer == LayerMask.NameToLayer("Dirt")) surfaceLabel = "Dirt";

            else if (layer == LayerMask.NameToLayer("Grass")) surfaceLabel = "Grass";

            else if (layer == LayerMask.NameToLayer("Wood")) surfaceLabel = "Wood";

            Debug.Log(surfaceLabel);



            if (!string.IsNullOrEmpty(surfaceLabel))

            {

                PlayFootstepSFX(surfaceLabel, entitiyTransform.position);

            }

        }



    }





    public void PlayFootstepSFX(string surfaceLable, Vector3 entitiyTransform)

    {

        FMOD.Studio.EventInstance footStepInst = RuntimeManager.CreateInstance(footstepSFX);



        footStepInst.setParameterByNameWithLabel("Surface", surfaceLable);

        footStepInst.set3DAttributes(RuntimeUtils.To3DAttributes(entitiyTransform));



        footStepInst.start();

        footStepInst.release();





    }



    public void PlaySwordSwingSFX()

    {

        RuntimeManager.PlayOneShot(swordSwingSFX);

    }



    public void PlayDamageImpactSFX()

    {

        RuntimeManager.PlayOneShot(damageImpactSFX);

    }



}