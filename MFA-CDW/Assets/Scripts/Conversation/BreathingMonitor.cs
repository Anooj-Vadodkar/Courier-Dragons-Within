
using UnityEngine;
using PixelCrushers.DialogueSystem;

// Rename this class to the same name that you used for the script file.
// Add the script to your Dialogue Manager. You can optionally make this 
// a static class and remove the inheritance from MonoBehaviour, in which
// case you won't add it to the Dialogue Manager.
//
// This class registers two example functions: 
//
// - DebugLog(string) writes a string to the Console using Unity's Debug.Log().
// - AddOne(double) returns the value plus one.
//
// You can use these functions as models and then replace them with your own.
public class BreathingMonitor : MonoBehaviour // Rename this class.
{
    [Tooltip("Typically leave unticked so temporary Dialogue Managers don't unregister your functions.")]
    
    [SerializeField] GameObject breath;
    public bool unregisterOnDisable = false;
    public bool DeepBreath = false;

    private float currentAirInLungs ;
    [SerializeField] public float sufficientAirInLungs;
    [SerializeField] public DialogueWrapper dialogueWrapper;
    private void Awake()
    {
        currentAirInLungs = (float)breath.GetComponent<TopTextObserver>()._value;
      
    }

    private void Update()
    {
        currentAirInLungs = (float)breath.GetComponent<TopTextObserver>()._value;
        //if the player has sufficent air in their lungs (float/double) then set Deep Breath to "true"
        if (currentAirInLungs >= sufficientAirInLungs)
        {
            DeepBreath = true;
            //dialogueWrapper.SetBreathTaken(DeepBreath);
        }
        else
        {
            DeepBreath = false;
        }

    }
    void OnEnable()
    {
        // Make the functions available to Lua: (Replace these lines with your own.)
        Lua.RegisterFunction(nameof(DebugLog), this, SymbolExtensions.GetMethodInfo(() => DebugLog(string.Empty)));
        Lua.RegisterFunction(nameof(AddOne), this, SymbolExtensions.GetMethodInfo(() => AddOne((double)0)));
        Lua.RegisterFunction(nameof(Reconsider), this, SymbolExtensions.GetMethodInfo(() => AddOne((double)0)));
    }

    void OnDisable()
    {
        if (unregisterOnDisable)
        {
            // Remove the functions from Lua: (Replace these lines with your own.)
            Lua.UnregisterFunction(nameof(DebugLog));
            Lua.UnregisterFunction(nameof(AddOne));
        }
    }

    public void DebugLog(string message)
    {
        Debug.Log(message);
    }

    public double AddOne(double value)
    { // Note: Lua always passes numbers as doubles.
        return value + 1;
    }

    public void Reconsider(double value)
    {
       
        //if DeepBreath is equal to true, show new menu option.
        

    }

    public void RevealCalmResponse()
    {
        DeepBreath= true;
    }

    
}



/**/
