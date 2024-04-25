using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsLine : MonoBehaviour
{
    // three line renderers for three colors, only supports gradient coloring, thus needs seperate
    private Dictionary<string, LineRenderer> lineRenderers = new Dictionary<string, LineRenderer>();
    [SerializeField]
    Material defaultLineMaterial;

    // setting the preset board RGB colors for lines
    Color blueColor = new Color(78.0f / 255, 0.0f / 255, 255.0f / 255, 255.0f / 255);
    Color greenColor = new Color(12.0f / 255, 172.0f / 255, 80.0f / 255, 255.0f / 255);
    Color redColor = new Color(255.0f / 255, 0.0f / 255, 0.0f / 255, 255.0f / 255);

    // Start is called before the first frame update
    void Start()
    {
        defaultLineMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
        CreateLines();
    }

    /// <summary>
    /// set up line points and draw the line, for top and bottom, object name is used to differentiate top and bottom,
    /// which fixes the x, y coordinates of the line for each points
    /// </summary>
    void CreateLines()
    {
        // 4 line renderers for 4 independent colored lines, same line for all would not support point to point coloring, only gradient
        // only one line renderer per object, so create children
        var bridge = new GameObject();
        bridge.gameObject.name = "Bridge";
        bridge.transform.SetParent(transform);
        lineRenderers["Bridge"] = bridge.AddComponent<LineRenderer>();
        var red = new GameObject();
        red.gameObject.name = "Red";
        red.transform.SetParent(transform);
        lineRenderers["Red"] = red.AddComponent<LineRenderer>();
        var blue = new GameObject();
        blue.gameObject.name = "Blue";
        blue.transform.SetParent(transform);
        lineRenderers["Blue"] = blue.AddComponent<LineRenderer>();
        var green = new GameObject();
        green.gameObject.name = "Green";
        green.transform.SetParent(transform);
        lineRenderers["Green"] = green.AddComponent<LineRenderer>();

        // set sorting layers of the lines to be below board
        bridge.GetComponent<Renderer>().sortingLayerName = "Lines";
        bridge.GetComponent<Renderer>().sortingOrder = 2;
        red.GetComponent<Renderer>().sortingLayerName = "Lines";
        blue.GetComponent<Renderer>().sortingLayerName = "Lines";
        green.GetComponent<Renderer>().sortingLayerName = "Lines";

        // setup of renderers
        foreach (var lineRenderer in lineRenderers)
        {
            lineRenderer.Value.widthMultiplier = 0.07f;
            lineRenderer.Value.useWorldSpace = true; //as the position points are gotten using transform.position, not sure if incompatible when moved
            lineRenderer.Value.material = defaultLineMaterial;
        }

        int offset = 1;
        int index = 1;
        Vector3[] positions = new Vector3[7]; // 7 points, 6 for 3 cells, 1 for origin. 
        positions[0] = transform.position; //add self position point
        if (gameObject.name != "PointsBoxTop")
        {
            offset = -1;
        }

        // the positions are found from the boardcells
        foreach (var cell in BoardBounds.cellPositions)
        {
            Vector3 temp = cell.Value[0];
            temp.y *= offset;
            temp.x = transform.position.x;
            positions[index] = temp; // first outside cell point
            index++;
            positions[index] = cell.Value[0] * offset; // cell connect point
            index++;
            if (index == 7)
            {
                // 7 points, so last is index 6
                break;
            }
        }

        // 4 lines from 7 points, as follows (0,1), (0,1,2), (1,3,4), (3,5,6)
        lineRenderers["Bridge"].positionCount = 2;
        lineRenderers["Red"].positionCount = 3;
        lineRenderers["Blue"].positionCount = 3;
        lineRenderers["Green"].positionCount = 3;
        // setting index and points manually, red green swap??
        lineRenderers["Bridge"].SetPosition(0, positions[0]);
        lineRenderers["Bridge"].SetPosition(1, positions[1] + new Vector3(0, 0.035f * offset, 0)); // bridge offset for symmetry 
        lineRenderers["Red"].SetPosition(0, positions[0]);
        lineRenderers["Red"].SetPosition(1, positions[5]);
        lineRenderers["Red"].SetPosition(2, positions[6]);
        lineRenderers["Blue"].SetPosition(0, positions[0]);
        lineRenderers["Blue"].SetPosition(1, positions[3]);
        lineRenderers["Blue"].SetPosition(2, positions[4]);
        lineRenderers["Green"].SetPosition(0, positions[0]);
        lineRenderers["Green"].SetPosition(1, positions[1]);
        lineRenderers["Green"].SetPosition(2, positions[2]);

        //setting colors
        lineRenderers["Red"].startColor = redColor;
        lineRenderers["Red"].endColor = redColor;
        lineRenderers["Blue"].startColor = blueColor;
        lineRenderers["Blue"].endColor = blueColor;
        lineRenderers["Green"].startColor = greenColor;
        lineRenderers["Green"].endColor = greenColor;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.time; // Get the current time

        // Calculate the gradient color based on time
        Color gradientColor = new Color(
            Mathf.Sin(time), // Red component
            Mathf.Sin(time + Mathf.PI * 2 / 3), // Green component
            Mathf.Sin(time + Mathf.PI * 4 / 3), // Blue component
            1f // Alpha component
        );

        // Set the start and end color of the bridge line renderer
        lineRenderers["Bridge"].startColor = gradientColor;
        lineRenderers["Bridge"].endColor = gradientColor;
    }
}
