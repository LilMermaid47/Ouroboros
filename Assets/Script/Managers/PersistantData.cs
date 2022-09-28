using UnityEngine;

public class PersistantData : MonoBehaviour
{
    public static PersistantData DataPerserver;

    private float CurrentVolume = 1;
    private BlindnessTypes BlindnessSelected;

    void Awake()
    {
        // If the instance reference has not been set, yet, 
        if (DataPerserver == null)
        {
            // Set this instance as the instance reference.
            DataPerserver = this;
        }
        else if (DataPerserver != this)
        {
            // If the instance reference has already been set, and this is not the
            // the instance reference, destroy this game object.
            Destroy(gameObject);
        }

        // Do not destroy this object, when we load a new scene.
        DontDestroyOnLoad(gameObject);
    }

    public void SetCurrentVolume(float volume)
    {
        CurrentVolume = volume;
    }

    public float GetCurrentVolume()
    {
        return CurrentVolume;
    }

    public void SetBlindnessSelected(BlindnessTypes blindnessTypes)
    {
        BlindnessSelected = blindnessTypes;
    }

    public BlindnessTypes GetBlindnessSelected()
    { 
        return BlindnessSelected; 
    }
}
