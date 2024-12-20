using System.Collections.Generic;
using UnityEngine;

class Branch : MonoBehaviour
{
    // Constructor class of a branch
    public Branch(GameObject gameObject_in, int parentPos_in, string branchType)
    {
        branchObject = gameObject_in;
        originalScale = gameObject_in.transform.localScale;

        // Calculate how tall the object is
        Renderer renderer = branchObject.GetComponent<Renderer>();
        originalHeight = renderer.bounds.size.y;

        parentIndex = parentPos_in;

        // Randomize how high upon the parent the branch should stem
        posOnparent = UnityEngine.Random.Range(0.2f, 0.9f);

        // Generate a random value of 0 to 360 for the yAxis
        float randomYValue = UnityEngine.Random.Range(0f, 360f);

        // Define in what direction the branch should rotate,
        // do this only once so the branch won't change position
        direction = new Vector3(1, randomYValue, 0);
    }

    public void Remove()
    {
        Destroy(branchObject);
    }

    // Return how tall the object is
    public float getHeight() { return originalHeight * scale; }
    public float getScale() { return scale; }
    public float getAngle() { return angle; }
    public Vector3 getTopPos()
    {
        Vector3 centerPos = branchObject.transform.position;

        // Fetch the height of the branchObject
        float objHeight = getHeight();
        // Get the distance from the bottom of the branch to its center point
        Vector3 distanceToPivot = new Vector3(0.0f, objHeight / 2.0f, 0.0f);
        // Adapt the distance to the rotation
        Quaternion rotationVector = Quaternion.Euler(rotation);
        Vector3 rotatedVector = rotationVector * distanceToPivot;

        return centerPos + rotatedVector;
    }
    public Vector3 getBottomPos()
    {
        Vector3 centerPos = branchObject.transform.position;

        // Fetch the height of the branchObject
        float objHeight = getHeight();
        // Get the distance from the bottom of the branch to its center point
        Vector3 distanceToPivot = new Vector3(0.0f, objHeight / 2.0f, 0.0f);
        // Adapt the distance to the rotation
        Quaternion rotationVector = Quaternion.Euler(rotation);
        Vector3 rotatedVector = rotationVector * distanceToPivot;

        return centerPos - rotatedVector;
    }

    public void moveBranch(Vector3 pos)
    {
        branchObject.transform.position = pos;

    }
    public void rotateBranch(float angle_in)
    {
        angle = angle_in;
        rotation = new Vector3(direction.x * angle_in, direction.y, direction.z * angle_in);
        branchObject.transform.Rotate(rotation);

    }
    public void scaleBranch(float scale_in)
    {
        scale = scale_in;
        branchObject.transform.localScale = branchObject.transform.localScale * scale;
    }
    public void resetBranch()
    {
        branchObject.transform.rotation = Quaternion.identity;
        branchObject.transform.localScale = originalScale;
    }

    public void addChild()
    {
        childrenAmount++;
    }
    public void removeChild()
    {
        childrenAmount--;
        if (childrenAmount < 0) { throw new System.Exception("ISSUE with removing branches: 'childrenAmount' < 0 !!"); }
    }
    public int getChildrenAmount()
    {
        return childrenAmount;
    }

    private float angle = 0;
    private Vector3 direction;
    public Vector3 rotation = new Vector3(0.0f, 0.0f, 0.0f);

    private float originalHeight;
    public float posOnparent;
    private float scale = 1;
    private Vector3 originalScale;
    private GameObject branchObject;

    public int parentIndex;
    private int childrenAmount = 0;
}

public class CustomPlant : MonoBehaviour
{
    private GameObject stemPrefab;
    private GameObject branchPrefab;
    private GameObject leafPrefab;

    private GameObject pot;
    private Vector3 potTop;

    private float rotationMax = 60.0f;
    private float rotationMin = 20.0f;

    private float scaleMax = 0.8f;
    private float scaleMin = 0.6f;

    public CustomPlant(GameObject stemPrefab_in, GameObject branchPrefab_in, GameObject leafPrefab_in, GameObject pot_in)
    {
        stemPrefab = stemPrefab_in;
        branchPrefab = branchPrefab_in;
        leafPrefab = leafPrefab_in;
        pot = pot_in;
        createPlant();
    }

    public void updateRotation(float daysWithoutWater)
    {
        float rotationImpact = 80.0f * (daysWithoutWater / 62.0f);
        rotationMax = Mathf.Round((60.0f + rotationImpact) * 10f) / 10f;
        rotationMin = Mathf.Round((20.0f + rotationImpact) * 10f) / 10f;
    }

    public void updateScale(float daysLived)
    {
        float scaleImpact = 0.6f * (daysLived / 365.0f);
        scaleMax = Mathf.Round((0.4f + scaleImpact) * 10f) / 10f;
        scaleMin = Mathf.Round((0.2f + scaleImpact) * 10f) / 10f;
    }

    private void createPlant()
    {
        /* {{ Create the stem of the tree }} */
        // Get the position of the pots pivot point
        Vector3 potCenter = pot.transform.position;

        // Get the position of the pots top point
        Renderer renderer = pot.GetComponent<Renderer>();
        // Move up from half the pots height so it won't spawn in the pots center
        float potHeight = renderer.bounds.size.y;
        potCenter.y += potHeight / 2;
        potTop = potCenter;

        // Move up from half the spawnee height so it won't spawn in the spawnee center
        renderer = stemPrefab.GetComponent<Renderer>();
        float spawneeHeight = renderer.bounds.size.y;
        potCenter.y += spawneeHeight / 2;

        // Instantiate the stem of the tree
        GameObject stem = Instantiate(stemPrefab, potCenter, pot.transform.rotation);
        Branch stemBranch = new Branch(stem, -1, "default");

        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        stemBranch.scaleBranch(scale);
        branches.Add(stemBranch);

        /* {{ Create the branches on the stem }} */
        for (int i = 0; i < 5; i++) { createBranch(0); }

        /* {{ Add branches on branches }} */
        int parentIndex = 1;
        for (int i = 0; i < 2; i++)
        {
            createBranch(parentIndex);
            if (branches[parentIndex].getChildrenAmount() > 1) { parentIndex++; }
        }

        applyLeafs();
    }

    private void createBranch(int parentIndex)
    {
        if (parentIndex > 0) { branches[parentIndex].addChild(); }
        GameObject branchObj = Instantiate(branchPrefab);
        Branch branchDummy = new Branch(branchObj, parentIndex, "default");

        float angle = UnityEngine.Random.Range(rotationMin, rotationMax);
        branchDummy.rotateBranch(angle);

        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        if (parentIndex > 0) { scale *= 0.5f * branches[parentIndex].getScale(); }
        branchDummy.scaleBranch(scale);

        positionBranchOnRoot(branchDummy);
        branches.Add(branchDummy);
    }

    private void positionBranchOnRoot(Branch branch)
    {

        Vector3 parentRootPos;
        if (branch.parentIndex < 0)
        {
            parentRootPos = potTop;
        }
        else
        {
            Vector3 parentTopPos = branches[branch.parentIndex].getTopPos();
            Vector3 parentBottomPos = branches[branch.parentIndex].getBottomPos();
            Vector3 parentRange = parentTopPos - parentBottomPos;
            parentRootPos = parentBottomPos + parentRange * branch.posOnparent;
        }

        Vector3 branchRotation = branch.rotation;

        // Fetch the height of the branchObject
        float branchHeight = branch.getHeight();
        // Get the distance from the bottom of the branch to its center point
        Vector3 distanceToPivot = new Vector3(0.0f, branchHeight / 2.0f, 0.0f);

        // Apply the rotation applied to the branchObject to get the new distance
        Quaternion rotationVector = Quaternion.Euler(branchRotation);
        Vector3 rotatedVector = rotationVector * distanceToPivot;

        Vector3 finalPosition = parentRootPos + rotatedVector;
        branch.moveBranch(finalPosition);
    }

    public void updatePlant()
    {
        branches[0].resetBranch();
        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        branches[0].scaleBranch(scale);
        positionBranchOnRoot(branches[0]);

        for (int i = 1; i < branches.Count; i++)
        {
            branches[i].resetBranch();
            int parentIndex = branches[i].parentIndex;

            scale = UnityEngine.Random.Range(scaleMin, scaleMax);
            if (parentIndex > 0) { scale *= 0.6f *branches[parentIndex].getScale(); }
            branches[i].scaleBranch(scale);

            float angle = UnityEngine.Random.Range(rotationMin, rotationMax);
            if (parentIndex > 0) { angle += 0; }
            branches[i].rotateBranch(angle);

            positionBranchOnRoot(branches[i]);
        }

        applyLeafs();
    }

    private void applyLeafs()
    {
        foreach (GameObject leaf in leafs)
        {
            Destroy(leaf);
        }
        for (int i = 0; i < branches.Count; i++)
        {
            int leavesPerBranch = UnityEngine.Random.Range(5, 8);
            for (int j = 0; j < leavesPerBranch; j++) {
                    
                    float degreesToIncrease = 360 / leavesPerBranch;
                    GameObject leafObj = Instantiate(leafPrefab);
                    Vector3 currentScale = leafObj.transform.localScale;
                    float branchScale = branches[i].getScale();
                    leafObj.transform.localScale = new Vector3(
                        currentScale.x * branchScale * UnityEngine.Random.Range(1.0f, 1.2f),
                        currentScale.y * branchScale,
                        currentScale.z * branchScale * UnityEngine.Random.Range(0.8f, 1.0f)
                    );

                // Define the two rotation vectors
                Vector3 angleOfLeaf = new Vector3(0.0f, degreesToIncrease * j, 0.0f); // In degrees
                    Vector3 angleOfBranch = branches[i].rotation;

                    // Apply the angle of the branch on the angle of the Leaf
                    Quaternion combinedRotation = Quaternion.Euler(angleOfLeaf) * Quaternion.Euler(angleOfBranch);

                    // Get the resulting rotation as a vector (if needed)
                    Vector3 resultEulerAngles = combinedRotation.eulerAngles;

                    leafObj.transform.Rotate(angleOfBranch);
                    leafObj.transform.Rotate(resultEulerAngles);

                    leafObj.transform.position = branches[i].getTopPos();
                    leafs.Add(leafObj);
                }
        }
        
    }

    private List<Branch> branches = new List<Branch>();

    private List<GameObject> leafs = new List<GameObject>();

}