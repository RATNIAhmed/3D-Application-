//  26/11/2015 -> class definition
//  03/12/2015 -> LERP not working
//  28/12/2015 -> component rotation definition
//  07/01/2015 -> LERP problems solved
using UnityEngine;

/// <summary>
/// This class is responsible of the component movements (translation + rotation)
/// </summary>
public class ObjectMovement : MonoBehaviour
{
    // rotationType == 1 : rot=Pi | rotationType == 2 : rot=Pi/2
    private int rotationType;    // not used now => all rotate Pi/2
    public float mouvementSpeed;
    public float rotationSpeed;

    private Vector3 startMarker;
    private Vector3 endMarker;
    private Vector3 posInit;

    private float startTime;
    private float journeyLength;

    private bool translate;

    private KeyCode rotationLeft = KeyCode.LeftArrow;
    private KeyCode rotationRight = KeyCode.RightArrow;

    private int actualRotation;
    private int desiredRotation;    // for later use - now just matching to 0 (actual design orientation)
    public string componentName;

    /// <summary>
    /// Initialization
    /// </summary>
    void Start()
    {
        // in case of no initialization
        if (mouvementSpeed == 0) mouvementSpeed = 10.0f;
        if (rotationSpeed == 0) rotationSpeed = .25f;
        if (rotationType == 0) rotationType = 2;
        if (componentName == null) componentName = "";
        desiredRotation = 0;

        translate = false;
        posInit = this.transform.position;

        // random initial rotation
        int times = Random.Range(1, 2 * rotationType);
        actualRotation = (actualRotation + times) % 4;
        transform.eulerAngles = transform.eulerAngles + 180 / rotationType * Vector3.up * times;
    }

    /// <summary>
    /// Sets the actual position to be the start position and the destination position to be: its
    /// father's position (<c>home==false</c>) or the initial position when game started (<c>home==true</c>)
    /// </summary>
    /// <param name="type">specifies whether is its father's position (hand/target) or the initial position</param>
	public void setDestPosition(int type = 0)
    {
        if (type == 2) endMarker = posInit;
        else if (type == 1) endMarker = new Vector3(0, 1, 0);
        else endMarker = Vector3.zero;

        startMarker = this.transform.localPosition;     // !!!!!!!!!!!!!!!!!!!
        journeyLength = Vector3.Distance(startMarker, endMarker);
        translate = true;
        startTime = Time.time;
        Debug.Log("  set destination = OK");
    }

    /// <summary>
    /// Updates the actual position by interpolation between start and end markers
    /// Manages its rotation depending on keystrokes
    /// </summary>
    void Update()
    {
        // Translation
        if (translate)
        {
            float distCovered = (Time.time - startTime) * mouvementSpeed;
            float fracJourney = distCovered / journeyLength;
            transform.localPosition = Vector3.Lerp(startMarker, endMarker, fracJourney);
            translate = !(startMarker == endMarker);
        }

        // Rotation
        if (this.tag != "inHand") return;
        if (Input.GetKeyDown(rotationLeft))
        {
            transform.eulerAngles = transform.eulerAngles + 180 / rotationType * Vector3.up;
            actualRotation = (actualRotation + 1) % 4;
        }

        if (Input.GetKey(rotationRight))
        {
            transform.eulerAngles = transform.eulerAngles - 180 / rotationType * Vector3.up;
            actualRotation = (actualRotation + 3) % 4;
        }
    }

    /// <summary>
    /// This functions returns the name of the component it represents
    /// </summary>
    /// <returns>a string containing the name of the component that it represents</returns>
    public string getComponentName()
    {
        return componentName;
    }

    /// <summary>
    /// This functions verifies if the rotation of the component is the correct one
    /// </summary>
    /// <returns>true if the rotation is the desired one, false otherwise</returns>
    public bool isOnDesiredRotation()
    {
        Debug.Log("Im a " + componentName + " and my position is " + actualRotation + " should be " + desiredRotation);
        return (actualRotation == desiredRotation) ||  (componentName == "Resistance" || componentName == "Button") && ((actualRotation + 2) % 4 == desiredRotation);
    }
}

