using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Importera UI-namespace

public class Plant : MonoBehaviour
{
    public int maxDepth = 5;        // Max rekursionsdjup (antal niv�er av grenar)
    public float branchLength = 1f; // Grundl�ngd f�r varje gren
    public float growthSpeed = 0.5f; // Hur snabbt tr�det v�xer
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

        // Rita om v�xten
        GrowPlant(Vector3.zero, Vector3.up, maxDepth);
    }

    void GrowPlant(Vector3 startPosition, Vector3 direction, int depth)
    {
        if (depth <= 0) return;

        // Skapa stammen f�rst
        GameObject trunk = new GameObject("Trunk");
        trunk.transform.parent = transform; // S�tt stammen som barn till v�xten
        LineRenderer line = trunk.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = Mathf.Lerp(0.1f, 0.01f, (maxDepth - depth) / (float)maxDepth); // J�mn �verg�ng i tjocklek
        line.endWidth = line.startWidth;
        line.material = new Material(Shader.Find("Sprites/Default")); // Enkel standardmaterial
        line.startColor = new Color(0.55f, 0.27f, 0.07f); // Brun f�rg
        line.endColor = new Color(0.55f, 0.27f, 0.07f); // Brun f�rg

        Vector3 endPosition = startPosition + direction * branchLength;

        // S�tt stamens positioner direkt
        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);

        // L�gg till grenar p� sidorna
        if (depth > 1)
        {
            GrowBranch(endPosition, Quaternion.Euler(0, 0, angle) * direction, depth - 1);
            GrowBranch(endPosition, Quaternion.Euler(0, 0, -angle) * direction, depth - 1);
        }

        // Forts�tt att v�xa stammen
        GrowPlant(endPosition, direction, depth - 1);
    }

    void GrowBranch(Vector3 startPosition, Vector3 direction, int depth)
    {
        if (depth <= 0) return;

        // Skapa en ny gren
        GameObject branch = new GameObject("Branch");
        branch.transform.parent = transform; // S�tt gren som barn till v�xten
        LineRenderer line = branch.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.05f * depth;
        line.endWidth = 0.03f * depth;
        line.material = new Material(Shader.Find("Sprites/Default")); // Enkel standardmaterial
        line.startColor = new Color(0.55f, 0.27f, 0.07f); // Brun f�rg
        line.endColor = Color.green; // Gr�n f�rg f�r grenens slut

        Vector3 endPosition = startPosition + direction * branchLength;

        // S�tt grenens positioner direkt
        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);

        // N�sta niv� av grenar
        GrowBranch(endPosition, Quaternion.Euler(0, 0, angle) * direction, depth - 1);
        GrowBranch(endPosition, Quaternion.Euler(0, 0, -angle) * direction, depth - 1);
    }
}