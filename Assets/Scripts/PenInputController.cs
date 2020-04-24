using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PenInputController : MonoBehaviour
{
    public SteamVR_Action_Boolean m_DrawAction;
    private Interactable interactable;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (interactable.attachedToHand != null)
        {
            SteamVR_Input_Sources source = interactable.attachedToHand.handType;
            
            if (m_DrawAction[source].stateDown) // trigger down
            {
                Draw();
            }
            else if (m_DrawAction[source].stateDown) // trigger hold
            {
                
            }
            else if (m_DrawAction[source].stateDown) // trigger up
            {

            }
       
        }
    }

    void Draw()
    {
        
    }
}
