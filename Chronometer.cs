// RATNI 03/12/2015 -> class definition
using UnityEngine;

/// <summary>
/// This class is responsible of chronometer management
/// </summary>
public class Chronometer : MonoBehaviour {

	private float timeElapsed;
	private bool on;

	/// <summary>
    /// Initialization
    /// </summary>
	void Start ()
    {
        on = false;
        timeElapsed = 0.0f;
	}

	/// <summary>
    /// Starts the chronometer
    /// </summary>
	public void Enable(){
        if (!on) Debug.Log("in colision [START chronometer]");
        on = true;
	}

	/// <summary>
    /// Stops the chronometer
    /// </summary>
	public void Disable(){
		on = false;
		timeElapsed = 0.0f;
	}

	/// <summary>
    /// Gets the time elapsed since chronometer was started
    /// </summary>
    /// <returns>the seconds since chronometer was started</returns>
	public float getTimeElapsed(){
		return timeElapsed;
	}
    
	/// <summary>
    /// Updates te elapsed time each frame
    /// </summary>
	void Update () {
		if (on) timeElapsed += Time.deltaTime;
	}
}
