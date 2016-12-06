//  RATNI-RICCA
//  26/11/2015 -> class definition
//  03/12/2015 -> first version for selection
//  28/12/2015 -> first version for components placement (tags definition)
//  07/12/2015 -> second version for selection + manipulation (all cases considered)
//  08/12/2015 -> solved problems for selection
//  10/12/2015 -> added menu texts and exit

// TODO -> add switch between components in hand and placed
//      -> main menu
//      -> effects for testing (audio + visual)
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 
/// </summary>
public class HandMouvement : MonoBehaviour
{
    private const KeyCode exitkey = KeyCode.Q;
    private const KeyCode resetkey = KeyCode.R;
    private const KeyCode validationkey = KeyCode.V;
    private const KeyCode cancelkey = KeyCode.Escape;
    private const float timeThreshold = 0.5f;
    private const int SELECTION = 0;
    private const int MANIPULATION = 1;
    private const int VALIDATION = 2;
    private bool menu;

    private string[] manipulationTAGS = { "Target", /*"TargetPlaced", "ComponentPlaced",*/ "TrashBin", "Menu" };
    private string[] selectionTAGS = { "Component", "ComponentPlaced", "Menu" };

    private float speed = 0.3f;
    private float rayDistance = 20.0f;
    private Vector3 rayDirection;

    private Transform objToSelect;
    private Material materialInCollision;
    public Material materialSelection;
    private GameObject componentObject;

    public Light LED;
    public Light AmbientLight;

    private int mode;
    public Text componentText;
    public Text exitText;
    public Text longText;

    public AudioClip selectAudio;
    private AudioSource source;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        mode = SELECTION;
        rayDirection = new Vector3(0, -Mathf.Sin(Mathf.PI / 8), Mathf.Cos(Mathf.PI / 4));
        componentObject = null;
        objToSelect = null;
        menu = false;
        componentText.text = "";
        exitText.text = "";
        longText.text = "";
        AmbientLight.enabled = true;

        source = GetComponent<AudioSource>();
    }

    /// <summary>
    /// 
    /// </summary>
    void Update()
    {
        var posMouse = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        this.transform.Translate(speed * posMouse, Space.World);

        if (menu)
        {
            //Debug.Log("waiting for Q or ESC");
            if (Input.GetKeyDown(exitkey))
            {
                // TODO: save data for the player
                Debug.Log("EXIT game");
                Application.Quit();
            }
            else if (Input.GetKeyDown(cancelkey))
            {
                menu = false;
                exitText.text = "";
                longText.text = "";
                RemoveShowSelectObject();
                Debug.Log("BACK to game");
            }
            else if (Input.GetKeyDown(validationkey))
            {
                menu = false;
                exitText.text = "";
                longText.text = "";
                RemoveShowSelectObject();
                Debug.Log("VALIDATE game");

                bool valid = true;
                GameObject[] targetsWithComponents = GameObject.FindGameObjectsWithTag("TargetPlaced");

                if (targetsWithComponents.Length != 4)
                {
                    longText.text = "There are components that need to be placed";
                    return;
                }
                else
                {
                    foreach (GameObject obj in targetsWithComponents)
                    {
                        valid &= obj.GetComponent<Validation>().isValidComponentTarget();
                    }
                }

                if (!valid)
                {
                    longText.text = "Some components are not correctly placed";
                    return;
                }
                mode = VALIDATION;
                exitText.text = "";
                longText.text = "Test the circuit by keeping the button pushed\n\n Press <R> to reset the game";
            }
            return;
        }

        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, rayDirection, rayDistance).OrderBy(h => h.distance).ToArray();

        if (hits.Length > 0)
        {
            switch (mode)
            {
                case SELECTION:
                    if (!selectionTAGS.Contains(hits[0].transform.tag))
                    {
                        this.GetComponent<Chronometer>().Disable();
                        RemoveShowSelectObject();
                        return;
                    }

                    ShowSelectObject(hits[0].transform);
                    this.GetComponent<Chronometer>().Enable();

                    // timeout for chronometer
                    if (this.GetComponent<Chronometer>().getTimeElapsed() > timeThreshold)
                    {
                        source.PlayOneShot(selectAudio, 1.0f);

                        if (hits[0].transform.tag == "Menu")
                        {
                            menu = true;
                            // TODO : show options and help ... (this is the tools' menu)
                            exitText.text = "Press <Q> to exit <ESC> to cancel <V> to validate";
                        }
                        else
                        {
                            // first object == closest one
                            componentObject = hits[0].transform.gameObject;
                            componentText.text = "You picked up a: " + componentObject.GetComponent<ObjectMovement>().getComponentName();
                            componentObject.tag = "inHand";

                            // if it was placed => untag targetplaced ---- (it is my father)
                            if (componentObject.transform.parent && componentObject.transform.parent.tag == "TargetPlaced")
                                componentObject.transform.parent.tag = "Target";
                            
                            // move object to hand
                            componentObject.transform.parent = this.transform;
                            componentObject.GetComponent<ObjectMovement>().setDestPosition();

                            Debug.Log("Switch to MANIPULATION");
                            mode = 1;

                            this.GetComponent<Chronometer>().Disable();
                            RemoveShowSelectObject();
                        }
                    }
                    break;
                case MANIPULATION:
                    if (!componentObject || !manipulationTAGS.Contains(hits[0].transform.tag))
                    {
                        this.GetComponent<Chronometer>().Disable();
                        RemoveShowSelectObject();
                        return;
                    }

                    ShowSelectObject(hits[0].transform);
                    this.GetComponent<Chronometer>().Enable();

                    // timeout for chronometer
                    if (this.GetComponent<Chronometer>().getTimeElapsed() > timeThreshold)
                    {
                        source.PlayOneShot(selectAudio, 1.0f);

                        if (hits[0].transform.tag == "Menu")
                        {
                            menu = true;
                            // TODO : show options and help ... (this is the tools' menu)
                            exitText.text = "Press <Q> to exit <ESC> to cancel <V> to validate";
                        }
                        else
                        {
                            componentText.text = "";

                            // CASE : deleting an object => move to components box
                            if (hits[0].transform.tag == "TrashBin")
                            {
                                componentObject.transform.parent = null;
                                componentObject.GetComponent<ObjectMovement>().setDestPosition(2);
                                componentObject.tag = "Component";
                            }

                            // CASE : component/target placed => switch components!
                            else if (hits[0].transform.tag == "ComponentPlaced")// || hits[0].transform.tag == "TargetPlaced")
                            {
                                
                            }

                            // CASE : component to place
                            else
                            {
                                // first object == closest one
                                componentObject.transform.tag = "ComponentPlaced";
                                hits[0].transform.transform.tag = "TargetPlaced";
                                componentObject.transform.parent = hits[0].transform;
                                componentObject.GetComponent<ObjectMovement>().setDestPosition(1);
                            }

                            Debug.Log("Switch to SELECTION");
                            mode = 0;

                            this.GetComponent<Chronometer>().Disable();
                            RemoveShowSelectObject();
                        }
                    }
                    break;
                case VALIDATION:
                    if (Input.GetKeyDown(resetkey))
                    {
                        //Application.LoadLevel(0); // deprecated
                        SceneManager.LoadScene("GameScene");
                        return;
                    }

                    if (hits[0].transform.tag == "ComponentPlaced")
                    {
                        if (hits[0].transform.gameObject.GetComponent<ObjectMovement>().getComponentName() == "Button")
                        {
                            // turn on LED
                            LED.enabled = true;
                        }
                        else
                        {
                            LED.enabled = false;
                        }
                    }
                    else
                    {
                        LED.enabled = false;
                    }
                    break;
            }
        }
        else
        {
            this.GetComponent<Chronometer>().Disable();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    void ShowSelectObject(Transform obj)
    {
        if (objToSelect != null) return;

        //Debug.Log("ON material");
        objToSelect = obj;
        materialInCollision = objToSelect.GetComponent<Renderer>().material;
        objToSelect.GetComponent<Renderer>().material = materialSelection;
    }

    /// <summary>
    /// 
    /// </summary>
    void RemoveShowSelectObject()
    {
        if (objToSelect == null) return;

        //Debug.Log("OUT material");
        objToSelect.GetComponent<Renderer>().material = materialInCollision;
        objToSelect = null;
    }

    /// <summary>
    /// This function is just for showing the raycast for debug (only seen in the scene)
    /// </summary>
    void OnDrawGizmos()
    {
        // for debug without LineRenderer
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, rayDirection * rayDistance);
    }
}
