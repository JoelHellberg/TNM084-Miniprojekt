using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Importera UI-namespace

public class Plant : MonoBehaviour
{
    public int maxDepth = 5;        // Max rekursionsdjup (antal nivåer av grenar)
    public float branchLength = 1f; // Grundlängd för varje gren
    public float growthSpeed = 0.5f; // Hur snabbt trädet växer
    public float angle = 30f;       // Vinkel mellan grenar

    public Slider branchLengthSlider; // Referens till UI-slidern

    void Start()
    {
        if (branchLengthSlider != null)
        {
            branchLengthSlider.onValueChanged.AddListener(UpdateBranchLength);
        }
        GrowPlant(Vector3.zero, Vector3.up, maxDepth);
    }

    void UpdateBranchLength(float value)
    {
        branchLength = value;
        RedrawTree();
    }

    void RedrawTree()
    {
        // Ta bort existerande delar
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Rita om växten
        GrowPlant(Vector3.zero, Vector3.up, maxDepth);
    }

    void GrowPlant(Vector3 startPosition, Vector3 direction, int depth)
    {
        if (depth <= 0) return;

        // Skapa stammen först
        GameObject trunk = new GameObject("Trunk");
        trunk.transform.parent = transform; // Sätt stammen som barn till växten
        LineRenderer line = trunk.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = Mathf.Lerp(0.1f, 0.01f, (maxDepth - depth) / (float)maxDepth); // Jämn övergång i tjocklek
        line.endWidth = line.startWidth;
        line.material = new Material(Shader.Find("Sprites/Default")); // Enkel standardmaterial
        line.startColor = new Color(0.55f, 0.27f, 0.07f); // Brun färg
        line.endColor = new Color(0.55f, 0.27f, 0.07f); // Brun färg

        Vector3 endPosition = startPosition + direction * branchLength;

        // Sätt stamens positioner direkt
        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);

        // Lägg till grenar på sidorna
        if (depth > 1)
        {
            GrowBranch(endPosition, Quaternion.Euler(0, 0, angle) * direction, depth - 1);
            GrowBranch(endPosition, Quaternion.Euler(0, 0, -angle) * direction, depth - 1);
        }

        // Fortsätt att växa stammen
        GrowPlant(endPosition, direction, depth - 1);
    }

    void GrowBranch(Vector3 startPosition, Vector3 direction, int depth)
    {
        if (depth <= 0) return;

        // Skapa en ny gren
        GameObject branch = new GameObject("Branch");
        branch.transform.parent = transform; // Sätt gren som barn till växten
        LineRenderer line = branch.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.05f * depth;
        line.endWidth = 0.03f * depth;
        line.material = new Material(Shader.Find("Sprites/Default")); // Enkel standardmaterial
        line.startColor = new Color(0.55f, 0.27f, 0.07f); // Brun färg
        line.endColor = Color.green; // Grön färg för grenens slut

        Vector3 endPosition = startPosition + direction * branchLength;

        // Sätt grenens positioner direkt
        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);

        // Nästa nivå av grenar
        GrowBranch(endPosition, Quaternion.Euler(0, 0, angle) * direction, depth - 1);
        GrowBranch(endPosition, Quaternion.Euler(0, 0, -angle) * direction, depth - 1);
    }
}