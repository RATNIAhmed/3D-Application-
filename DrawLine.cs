// RATNI 26/12/2015 -> class definition
using UnityEngine;

/// <summary>
/// This class is responsible for rendering the ray for RayCasting technique
/// </summary>
public class DrawLine : MonoBehaviour {

    private const float distance = 20f;
    private LineRenderer lineRenderer;
    private Vector3 direction;

    /// <summary>
    /// Initialization
    /// </summary>
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetWidth(.125f, .125f);

        // default initialization for line direction
        direction = new Vector3(0, -Mathf.Sin(Mathf.PI / 8), Mathf.Cos(Mathf.PI / 4));
    }

    /// <summary>
    /// Sets the line direction to <c>newDirection</c>
    /// </summary>
    /// <param name="newDirection">3-dimensional vector with the new direction</param>
    public void SetLineDirection(Vector3 newDirection)
    {
        direction = newDirection;
    }
	
    /// <summary>
    /// Updates line renderer with new origin-destination positions
    /// </summary>
	void Update ()
    {
        // origin position
        lineRenderer.SetPosition(0, this.transform.position);

        // destination position
        Vector3 destination = this.transform.position + direction * distance;
        lineRenderer.SetPosition(1, destination);
    }
}
