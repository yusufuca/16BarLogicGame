using FMODUnity;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   public static AudioManager AMInstance { get; private set;}

    private void Awake () 
    {
    
        if(AMInstance != null && AMInstance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            AMInstance = this;
            DontDestroyOnLoad(gameObject);  
        }
    }

    [Header("Surfaces")]
    [SerializeField] private LayerMask surface;
    [Header("FootstepSFX")]
    [SerializeField] private EventReference footstepSFX;
    [Header("SwordSwingSFX")]
    [SerializeField] private EventReference swordSwingSFX;


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

  
}
