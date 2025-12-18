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


    ///* STATE MACHINE INT *///

   
    public bool isPlayerDeath = false;
    public bool isVictory = false;
    public bool setEpicState = false;
    public bool setAnxietyState = false;

    public float combatCooldown = 4f;
    public float lastCombatTriggerTimer = -99f;

    public float currentMagnitude = 0f;

    public EventInstance loopInstance;
    public float idleTimer = 0f;
    public float idleDelay = 16f;
    public bool isTransitioning = false; 
    

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
        if (isTransitioning) return;
        queuedState = SetTheNextState();

        if (queuedState != currentState && !string.IsNullOrEmpty(queuedState))  

        {
            
               
            StartCoroutine(ApplyChangeState(queuedState));
            currentState = queuedState;
            Debug.Log("Current State is "+currentState);
        }
        
    }

    public string SetTheNextState()
    {

        if (isPlayerDeath) return "Die";
        if (isVictory) return "Win";
        if (setEpicState) return "Epic";
        bool isCombatActive = Time.time < lastCombatTriggerTimer + combatCooldown;
        if (isCombatActive) return "Combat";
        if (setAnxietyState) return "Anxiety";
        if (currentMagnitude > 0.1f) return "Explore";

        return "Idle";        
    }

    public void CombatTimer()
    {
        lastCombatTriggerTimer = Time.time;
    }


    public IEnumerator ApplyChangeState(string targetState)

    {
        isTransitioning= true;
        int timeLinePos;
        loopInstance.getTimelinePosition(out timeLinePos);
        int currentPosInBar = timeLinePos % barDurationMS;
        float timeToNextBar = (barDurationMS - currentPosInBar) / 1000f;
        
        if (timeToNextBar > 0.05f)
        {
            yield return new WaitForSeconds(timeToNextBar - 0.05f);
        }

        loopInstance.setParameterByNameWithLabel("States", targetState);


       
        Debug.Log("Transition " + targetState);
        

        yield return new WaitForSeconds(4);

        loopInstance.setParameterByNameWithLabel("prevState", targetState);
        Debug.Log("Current State set to " + targetState);
        isTransitioning = false;
        
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

            //Debug.Log(surfaceLabel);



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