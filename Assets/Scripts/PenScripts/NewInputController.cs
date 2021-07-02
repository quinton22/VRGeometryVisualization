using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using System.Linq;

public class NewInputController : MonoBehaviour
{
    public SteamVR_Action_Boolean m_DrawAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Draw");
    public SteamVR_Action_Boolean m_SwitchToolAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("SwitchTool");
    [Tooltip("This currently does nothing")]
    public bool isGripToggle = false; // TODO
    public bool shouldReturnToBeltIfDropped = false;
    [
        Tooltip("If shouldReturnToBeltIfDropped is true, then set the height below which it will return to the belt"),
        Range(-1, 10)
    ]
    public float shouldReturnAtHeight = -1;
    public GameObject m_RestrictToPlane;
    public bool m_RestrictToPlaneWithBoundingBox = false;
    private Plane? restrictedPlane
    {
        get { return PlaneFromGameObject(m_RestrictToPlane); }
    }
    private RestrictedRect restrictedRect
    {
        get { return RectFromGameObject(m_RestrictToPlane); }
    }
    private bool isReturned = true;
    private BeltController beltController;
    private Interactable interactable;
    private bool isTriggerDown = false;
    private Tool currentTool = Tool.None;
    private bool shownDrawHint = false;
    private bool shownChangeToolHint = false;
    private Hand hintHand;
    private DisplayToolType m_DisplayToolType;
    private ToolType currentToolType;
    private TriggerPull triggerPull;
    private List<ToolType> toolsList;
    private IntroductionScript introductionScript;
    private bool isIntroFinished = false;

    public class RestrictedRect {
        private Plane plane;
        private Vector3 basisX;
        private Vector3 basisY;
        private Vector3 basisZ;
        private Vector3 center;
        private float det;
        private Vector3[] transformationMat;

        private float calcDeterminant(Vector3 basisX, Vector3 basisY, Vector3 basisZ)
        {
            return basisX.x * basisY.y * basisZ.z
                    + basisY.x * basisZ.y * basisX.z
                    + basisZ.x * basisX.y * basisY.z
                    - basisX.z * basisY.y * basisZ.x
                    - basisY.z * basisZ.y * basisX.x
                    - basisZ.z * basisX.y * basisY.x;
        }

        private Vector3[] calcTransformationMat(Vector3 basisX, Vector3 basisY, Vector3 basisZ)
        {
             return new Vector3[]{
                    new Vector3(basisY.y*basisZ.z - basisY.z*basisZ.y, basisX.z*basisZ.y - basisX.y*basisZ.z, basisX.y*basisY.z - basisX.z*basisY.y) / det,
                    new Vector3(basisY.z*basisZ.x - basisY.x*basisZ.z, basisX.x*basisZ.z - basisX.z*basisZ.x, basisX.z*basisY.x - basisX.x*basisY.z) / det,
                    new Vector3(basisY.x*basisZ.y - basisY.y*basisZ.x, basisX.y*basisZ.x - basisX.x*basisZ.y, basisX.x*basisY.y - basisX.y*basisY.x) / det
                };
        }
        private Vector3 pointInNewBasis(Vector3 point)
        {
            Vector3 v = point - center;
            return new Vector3(Vector3.Dot(transformationMat[0], v), Vector3.Dot(transformationMat[1], v), Vector3.Dot(transformationMat[2], v));
        }

        public RestrictedRect(Vector3 _basisX, Vector3 _basisY, Vector3 _basisZ, Vector3 _center)
        {
            basisX = _basisX;
            basisY = _basisY;
            basisZ = _basisZ;
            center = _center;
            plane = new Plane(basisY, center);
            det = calcDeterminant(basisX, basisY, basisZ);
            transformationMat = calcTransformationMat(basisX, basisY, basisZ);

            
            Debug.DrawLine(center, center + basisX, Color.red, 10000000);
            Debug.DrawLine(center, center + basisY, Color.green, 10000000);
            Debug.DrawLine(center, center + basisZ, Color.blue, 10000000);
        }

        public RestrictedRect(Vector3 one, Vector3 two, Vector3 three)
        {
            plane = new Plane(one, two, three);
            basisX = (two - one) / 2;
            basisY = plane.normal;
            basisZ = (three - two) / 2;
            center = one + basisX + basisZ;

            Debug.DrawLine(center, one, Color.red, 10000000);
            Debug.DrawLine(center, two, Color.green, 10000000);
            Debug.DrawLine(center, three, Color.blue, 10000000);

            det = calcDeterminant(basisX, basisY,  basisZ);
            

            /*
                [ basisX.x  basisY.x  basisZ.x ]
                [ basisX.y  basisY.y  basisZ.y ]
                [ basisX.z  basisY.z  basisZ.z ]


                [ basisY.y*basisZ.z - basisY.z*basisZ.y  basisX.z*basisZ.y - basisX.y*basisZ.z  basisX.y*basisY.z - basisX.z*basisY.y ]
                [ basisY.z*basisZ.x - basisY.x*basisZ.z  basisX.x*basisZ.z - basisX.z*basisZ.x  basisX.z*basisY.x - basisX.x*basisY.z ]
                [ basisY.x*basisZ.y - basisY.y*basisZ.x  basisX.y*basisZ.x - basisX.x*basisZ.y  basisX.x*basisY.y - basisX.y*basisY.x ]

                det = basisX.x * basisY.y * basisZ.z + basisY.x * basisZ.y * basisX.z + basisZ.x * basisX.y * basisY.z - basisX.z * basisY.y * basisZ.x - basisY.z * basisZ.y * basisX.x - basisZ.z * basisX.y * basisY.x

            */

            transformationMat = calcTransformationMat(basisX, basisY, basisZ);
        }

        public bool Contains(Vector3 point, float minDist = 0.05f)
        {
            if (isOnPlane(point, minDist)) {
                Vector3 newPoint = pointInNewBasis(point);

                if (newPoint.x <= 1 && newPoint.x >= -1 && newPoint.z <= 1 && newPoint.z >= -1) {
                    return true;
                }
            }
            return false;
        }


        public Vector3 ClosestPointOnRect(Vector3 point)
        {
            Vector3 pointOnPlane = plane.ClosestPointOnPlane(point);

            if (Contains(pointOnPlane)) {
                return pointOnPlane;
            }
            Vector3 newBasis = pointInNewBasis(pointOnPlane);

            float x = Mathf.Max(-1, Mathf.Min(newBasis.x, 1));
            float z = Mathf.Max(-1, Mathf.Min(newBasis.z, 1));

            return center + x * basisX + z * basisZ;
        }

        public bool isOnPlane(Vector3 point, float minDist = 0.05f)
        {
            return plane.GetDistanceToPoint(point) < minDist;
        }
    }


    void Start()
    {
        interactable = GetComponent<Interactable>();

        m_DrawAction.AddOnStateDownListener(TriggerDown, SteamVR_Input_Sources.LeftHand);
        m_DrawAction.AddOnStateDownListener(TriggerDown, SteamVR_Input_Sources.RightHand);
        m_DrawAction.AddOnStateUpListener(TriggerUp, SteamVR_Input_Sources.LeftHand);
        m_DrawAction.AddOnStateUpListener(TriggerUp, SteamVR_Input_Sources.RightHand);

        m_SwitchToolAction.AddOnStateDownListener(ButtonDown, SteamVR_Input_Sources.LeftHand);
        m_SwitchToolAction.AddOnStateDownListener(ButtonDown, SteamVR_Input_Sources.RightHand);

        m_DisplayToolType = FindObjectOfType<DisplayToolType>();
        toolsList = FindObjectOfType<ToolTypeList>().List;
        SetCurrentTool();

        triggerPull = FindObjectOfType<TriggerPull>();

        beltController = FindObjectOfType<BeltController>();
        introductionScript = FindObjectOfType<IntroductionScript>();
    }

    void SetCurrentTool()
    {
        print($"currentTool: {currentTool}");
        print($"Tools list: {string.Join(",", toolsList.Select(tool => tool.Name))}");
        currentToolType = toolsList.First((tool) => tool.Name == currentTool);
        
        print($"currentToolType: {toolsList.First((tool) => tool.Name == currentTool)}");
    }

    void Update()
    {
        if (isTriggerDown)
        {
            Drawing();
        }

        // TODO: somehow finish mesh

        if (interactable.attachedToHand != null)
        { 
            isReturned = false;
            if (!shownChangeToolHint)
            {
                // TODO: add button to toggle hints
                ControllerButtonHints.ShowButtonHint(interactable.attachedToHand, m_SwitchToolAction);
                ControllerButtonHints.ShowTextHint(interactable.attachedToHand, m_SwitchToolAction, "Switch Tool");
                hintHand = interactable.attachedToHand;
            }
            else if (!shownDrawHint)
            {
                ControllerButtonHints.ShowButtonHint(interactable.attachedToHand, m_DrawAction);
                ControllerButtonHints.ShowTextHint(interactable.attachedToHand, m_DrawAction, "Draw");
                hintHand = interactable.attachedToHand;
            }
        }
        else if (hintHand != null)
        {
            ControllerButtonHints.HideAllButtonHints(hintHand);
            ControllerButtonHints.HideAllTextHints(hintHand);
            hintHand = null;
        }

        if (shouldReturnToBeltIfDropped && interactable.attachedToHand == null && !isReturned)
        {
            if (transform.position.y < shouldReturnAtHeight) {
                beltController.goToLastCollider(gameObject);
                isReturned = true;
            }
        }

        if (!isIntroFinished && introductionScript.isFinished())
        {
            m_RestrictToPlane = null;
            isIntroFinished = true;
        }
    }

    public void TriggerDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (interactable.attachedToHand == null) return;

        if (shownChangeToolHint && !shownDrawHint)
        {
            shownDrawHint = true;
            ControllerButtonHints.HideButtonHint(interactable.attachedToHand, m_DrawAction);
            ControllerButtonHints.HideTextHint(interactable.attachedToHand, m_DrawAction);
        }

        triggerPull.PullTrigger();

        print($"currentToolType: {currentToolType}");

        StartDraw();
    }

    public void TriggerUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (interactable.attachedToHand == null) return;

        triggerPull.ReleaseTrigger();

        StopDraw();
    }
    
    public void ButtonDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        if (interactable.attachedToHand == null) return;

        if (interactable.attachedToHand.handType != fromSource) return; // if not the same hand, don't do anything

        // TODO: don't switch tools if drawing

        if (!shownChangeToolHint)
        {
            shownChangeToolHint = true;
            ControllerButtonHints.HideButtonHint(interactable.attachedToHand, m_SwitchToolAction);
            ControllerButtonHints.HideTextHint(interactable.attachedToHand, m_SwitchToolAction);
        }

        NextAttachment();
    }

    public void NextAttachment()
    {
        // cycle through attachments
        List<Tool> tl = m_DisplayToolType.availableTools.Intersect(toolsList.Select((tool) => tool.Name)).ToList();
        int index = tl.IndexOf(currentTool);
        if (index == tl.Count - 1)
        {
            currentTool = tl[0];
            m_DisplayToolType.DisplayTool(currentTool);
        }
        else
        {
            currentTool = tl[++index];
            m_DisplayToolType.DisplayTool(currentTool);
        }
        SetCurrentTool();
    }

    void StartDraw()
    {
        //Debug.Log("State: start");
        print($"currentToolType: {currentToolType}");
        if (restrictedPlane.HasValue)
        {
            currentToolType.RestrictToPlane(restrictedPlane.Value, withBoundingBox: m_RestrictToPlaneWithBoundingBox ? restrictedRect : null);
        }
        currentToolType.OnTriggerDown();
        isTriggerDown = true;
    }

    void Drawing()
    {
        //Debug.Log("State: drawing");
        currentToolType.OnTriggerHold();
    }

    void StopDraw()
    {
        //Debug.Log("State: end");
        isTriggerDown = false;
        currentToolType.OnTriggerUp();
        currentToolType.UnrestrictFromPlane();
    }

    Plane? PlaneFromGameObject(GameObject obj)
    {
        if (obj == null) { return null; }

        Vector3 point = obj.transform.position;
        Vector3 norm = obj.transform.up;

        return new Plane(inNormal: norm, inPoint: point);
    }

    RestrictedRect RectFromGameObject(GameObject obj)
    {
        if (obj == null) { return null; }
        return new RestrictedRect(obj.transform.right * obj.transform.localScale.x * 5, obj.transform.up, obj.transform.forward * obj.transform.localScale.z * 5, obj.transform.position);
    }
}
