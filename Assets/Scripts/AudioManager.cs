using FMOD.Studio;
using FMODUnity;
using System.Collections;
using TMPro;
using UnityEditor;
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
        linearInstance = RuntimeManager.CreateInstance(lineerStates);
        transitionInstance = RuntimeManager.CreateInstance(lineerStatesTranisitions);
        ambInstance = RuntimeManager.CreateInstance(ambSFX);


    }


    [Header("PlayerTransform")]
    public Transform Player;
    [Header("Surfaces")]

    [SerializeField] private LayerMask surface;

    [Header("SFX")]

    [SerializeField] private EventReference footstepSFX;

   

    [SerializeField] private EventReference swordSwingSFX;

  

    [SerializeField] private EventReference damageImpactSFX;

   
    [SerializeField] public EventReference gameStateLoop;
    [SerializeField] public EventReference poisionBallBreakSFX;
    [SerializeField] public EventReference gunBlastSFX;
    [SerializeField] public EventReference ambSFX;
    [SerializeField] public EventReference lineerStates;
    [SerializeField] public EventReference lineerStatesTranisitions;

    [Header("Text")]
    public TextMeshProUGUI statesText;
    public TextMeshProUGUI timerText;


    ///* STATE MACHINE INT *///

    public bool isLinear = true;


    public bool isPlayerDeath = false;
    public bool isVictory = false;
    public bool setEpicState = false;
    public bool setAnxietyState = false;

    public float combatCooldown = 8f;
    public float lastCombatTriggerTimer = -99f;

    public float currentMagnitude = 0f;

    public EventInstance loopInstance;
    public EventInstance linearInstance;
    public EventInstance transitionInstance;

    public EventInstance ambInstance;
 
    public bool isTransitioning = false;
   
   
    public string currentState = "Explore";

    public string queuedState = "Explore";



    public float BPM = 120f;

    private int barDurationMS;

    public float inactiveTime = 0f;
    public float toIdleWaitTime = 32f;

    public float lastTransitionTime = 0f;
    public float transitionWaitTime = 18;

    public string prevStateText = "Explore";
    public string currentStateText = "Explore";
    public string queuedStateText;

    private float combatTimer = 0f;


    

    private void Start()

    {
        barDurationMS = (int)((60 / BPM) * 4 * 1000);
        if(GameManager.GMInstance.isLinear)
        {
            isLinear = true;
        }
        else
        {
            isLinear = false;
        }
        if (!isLinear)
        {
            loopInstance.start();
        }
        else
        {
            linearInstance.start();
        }
        ambInstance.start();
        loopInstance.setParameterByNameWithLabel("States", "Explore");
        loopInstance.setParameterByNameWithLabel("prevState", "Explore");


    }



    private void Update()

    {
        LastMovementTimer();
        if (isLinear) 
        {
            TransitionTimer();
            statesText.text = $"Queued State is: {currentState} Current State is: {prevStateText} Is Transitionable: {isTransitionable()}";

            if (currentState == "Combat")
            {
                if (!isCombatActive())
                {
                    combatTimer += Time.deltaTime;
                }

            }
            else
            {
                combatTimer = 0f;
            }
            timerText.text = $"Transition Timer: {Mathf.RoundToInt(lastTransitionTime)} Idle Timer: {Mathf.RoundToInt(inactiveTime)} Last Combat Timer: {Mathf.RoundToInt(combatTimer)}";


            if (isTransitioning) return;
            if (isTransitionable())
            {
                queuedState = SetTheNextState();
            }

           
            if (queuedState != currentState && !string.IsNullOrEmpty(queuedState))
            {
                StartCoroutine(ApplyChangeStateLinear(queuedState));
                currentState = queuedState;
            }
        }
        else
        {


            TransitionTimer();
            statesText.text = $"Queued State is: {currentState} Current State is: {prevStateText} Is Transitionable: {isTransitionable()}";

            if (currentState == "Combat")
            {
                if (!isCombatActive())
                {
                    combatTimer += Time.deltaTime;
                }

            }
            else
            {
                combatTimer = 0f;
            }
            timerText.text = $"Transition Timer: {Mathf.RoundToInt(lastTransitionTime)} Idle Timer: {Mathf.RoundToInt(inactiveTime)} Last Combat Timer: {Mathf.RoundToInt(combatTimer)}";
            if (isTransitioning) return;
            if (isTransitionable())
            {
                queuedState = SetTheNextState();
            }


            if (queuedState != currentState && !string.IsNullOrEmpty(queuedState))

            {


                StartCoroutine(ApplyChangeState(queuedState));
                currentState = queuedState;
                
            }
        }
    }

    public string SetTheNextState()
    {
        
        if (isPlayerDeath) return "Die";
        if (isVictory) return "Win";
        if (setEpicState) return "Epic";
        if (setAnxietyState) return "Anxiety";
        if (isCombatActive()) return "Combat";
        if (currentMagnitude > 0.1f) return "Explore";
        if (CanChangeToIdle())  return "Idle";
        return queuedState;
    }
    private bool isCombatActive()
    {

        if(Time.time < lastCombatTriggerTimer + combatCooldown)
        { return true; }
        else return false;
    }
    void LastMovementTimer()
    {
        if (currentMagnitude < 0.1f && !isCombatActive())
        {
            inactiveTime += Time.deltaTime;

        }
        else
        {
            inactiveTime = 0;
        }
    }
    bool CanChangeToIdle()
    {
        
        if (inactiveTime > toIdleWaitTime)
        {
            return true;
        }
        else
        {
            return false;
        }

       
    }
    public void CombatTimer()
    {
        lastCombatTriggerTimer = Time.time;
    }

    bool isTransitionable()
    {
        if(lastTransitionTime > transitionWaitTime)
        {
            return true;
        }
        else
        {
            return false; 
        }
    }

    void TransitionTimer()
    {
        if(!isTransitioning)
        {
            lastTransitionTime += Time.deltaTime;
        }
    }
    public IEnumerator ApplyChangeState(string targetState)

    {
        lastTransitionTime = 0f;
        isTransitioning = true;
       
       

            int timeLinePos;
        loopInstance.getTimelinePosition(out timeLinePos);
        int currentPosInBar = timeLinePos % (barDurationMS*8);
        float timeToNextBar = ((barDurationMS*8) - currentPosInBar) / 1000f;

        if (timeToNextBar > 0.05f)
        {
            yield return new WaitForSeconds(timeToNextBar - 0.05f);
        }

        loopInstance.setParameterByNameWithLabel("States", targetState);
        currentStateText = targetState;


        Debug.Log("Transition " + targetState);

       
        

        yield return new WaitForSeconds(1);
        loopInstance.setParameterByNameWithLabel("prevState", targetState);
        prevStateText = targetState;
        Debug.Log("Current State set to " + targetState);
        isTransitioning = false;

    }

    public IEnumerator ApplyChangeStateLinear(string targetState)

    {
        lastTransitionTime = 0f;
        isTransitioning = true;



        int timeLinePos;
        linearInstance.getTimelinePosition(out timeLinePos);
        int currentPosInBar = timeLinePos % barDurationMS;
        float timeToNextBar = (barDurationMS - currentPosInBar) / 1000f;

        if (timeToNextBar > 0.05f)
        {
            yield return new WaitForSeconds(timeToNextBar - 0.05f);
        }
        linearInstance.setParameterByNameWithLabel("States", targetState);
        transitionInstance.setParameterByNameWithLabel("States", targetState);
        transitionInstance.start();
        currentStateText = targetState;

        yield return new WaitForSeconds(barDurationMS / 1000);
        transitionInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        
        

        transitionInstance.setParameterByNameWithLabel("prevState", targetState);

        prevStateText = targetState;
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

    public void PlayPoisinBallBreakSFX()
    {
        RuntimeManager.PlayOneShot(poisionBallBreakSFX);

    }

    public void PlayGunBlastSFX()
    {
        RuntimeManager.PlayOneShot(gunBlastSFX);
    }

}