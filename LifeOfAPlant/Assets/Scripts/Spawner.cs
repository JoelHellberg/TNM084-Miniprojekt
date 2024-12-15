using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;


public class Spawner : MonoBehaviour
{

    public GameObject pot;
    private Vector3 potTop;

    public GameObject stemPrefab;
    private GameObject stem;
    private Vector3 stemTop;
    private Vector3 stemBottom;

    public GameObject leafPrefab;

    private float rotationMax = 60.0f;
    private float rotationMin = 20.0f;

    private float scaleMax = 0.8f;
    private float scaleMin = 0.6f;

    public UnityEngine.UI.Slider daysLivedSlider;
    public UnityEngine.UI.Slider waterSlider;


    class Branch
    {
        // Create a branch
        public Branch(GameObject gameObject_in, int parentPos_in)
        {
            scale = 1;
            originalScale = gameObject_in.transform.localScale;
            branchObject = gameObject_in;
            Renderer renderer = branchObject.GetComponent<Renderer>();
            originalHeight = renderer.bounds.size.y;

            parentIndex = parentPos_in;
            childrenAmount = 0;


            // Generate a random value of either -1 or 1 for z,y & z to determine the direction of the branch
            float randomXValue = UnityEngine.Random.Range(0, 2) * 2 - 1;
            float randomZValue = UnityEngine.Random.Range(0, 2) * 2 - 1;

            posOnparent = UnityEngine.Random.Range(0.2f, 0.9f);

            // Generate a random value of 0 to 360 for the yAxis
            float randomYValue = UnityEngine.Random.Range(0f, 360f);

            direction = new Vector3(randomXValue, randomYValue, randomZValue);

            rotation = new Vector3(0.0f, 0.0f, 0.0f);
        }
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
        public float getHeight() { return originalHeight * scale; }

        public float getScale() { return scale; }

        public void moveBranch(Vector3 pos)
        {
            branchObject.transform.position = pos;

        }
        public void rotateBranch(float angle_in)
        {
            rotation = new Vector3 (direction.x * angle_in, direction.y, direction.z * angle_in);
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
            if(childrenAmount < 0) { throw new System.Exception("ISSUE with removing branches: 'childrenAmount' < 0 !!"); }
        }

        private Vector3 direction;
        public Vector3 rotation;

        private float originalHeight;
        public float posOnparent;
        private float scale;
        private Vector3 originalScale;
        private GameObject branchObject;

        public int parentIndex;
        private int childrenAmount;
    }

    public GameObject branchPrefab;
    private List<Branch> branches = new List<Branch>();
    private List<GameObject> leafs = new List<GameObject>();

    private void Start()
    {
        createPlant();

        UpdateLifeTime(daysLivedSlider.value);
        daysLivedSlider.onValueChanged.AddListener(UpdateLifeTime);

        UpdateWaterStatus(waterSlider.value);
        waterSlider.onValueChanged.AddListener(UpdateWaterStatus);
    }

    void UpdateLifeTime(float val)
    {
        float floatValue = daysLivedSlider.value;
        scaleMax = Mathf.Round((0.4f + 0.6f * (floatValue / 365.0f)) * 10f) / 10f;
        scaleMin = Mathf.Round((0.2f + 0.6f * (floatValue / 365.0f)) * 10f) / 10f;
        updatePlant();
    }

    void UpdateWaterStatus(float val)
    {
        float floatValue = waterSlider.value;
        rotationMax = Mathf.Round((60.0f + 80.0f * (floatValue / 62.0f)) * 10f) / 10f;
        rotationMin = Mathf.Round((20.0f + 80.0f * (floatValue / 62.0f)) * 10f) / 10f;
        updatePlant();
    }

    void createPlant()
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
        stemTop = potCenter + new Vector3(0, spawneeHeight, 0);
        stemBottom = potCenter;
        potCenter.y += spawneeHeight / 2;

        // Instantiate the stem of the tree
        stem = Instantiate(stemPrefab, potCenter, pot.transform.rotation);
        Branch stemBranch = new Branch(stem, -1);

        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        stemBranch.scaleBranch(scale);
        branches.Add(stemBranch);

        /* {{ Create the branches of the tree }} */
        for (int i = 0; i < 10; i++)
        {
            createBranch(0);
        }
        applyLeafs();
    }

    void createBranch(int parentIndex)
    {
        GameObject branchObj = Instantiate(branchPrefab);
        Branch branchDummy = new Branch(branchObj, parentIndex);

        float angle = UnityEngine.Random.Range(rotationMin, rotationMax);
        branchDummy.rotateBranch(angle);

        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        branchDummy.scaleBranch(scale);
        Debug.Log("First scale value: " + scale);


        Vector3 parentRootPos;
        if(parentIndex < 0) {
            Vector3 parentRange = stemTop - stemBottom;
            parentRootPos = stemBottom + parentRange * branchDummy.posOnparent;
        }
        else {
            Vector3 parentTopPos = branches[parentIndex].getTopPos();
            Vector3 parentBottomPos = branches[parentIndex].getTopPos();
            Vector3 parentRange = parentTopPos - parentBottomPos;
            parentRootPos = parentBottomPos + parentRange * branchDummy.posOnparent;
        }
        positionPlantOnRoot(branchDummy);
        branches.Add(branchDummy);
    }

    void positionPlantOnRoot(Branch branch)
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

    void updatePlant()
    {
        branches[0].resetBranch();
        float scale = UnityEngine.Random.Range(scaleMin, scaleMax);
        branches[0].scaleBranch(scale);
        positionPlantOnRoot(branches[0]);

        for (int i = 1; i < branches.Count; i++) {
            branches[i].resetBranch();

            scale = UnityEngine.Random.Range(scaleMin, scaleMax);
            branches[i].scaleBranch(scale);

            float angle = UnityEngine.Random.Range(rotationMin, rotationMax);
            Debug.Log("Actual angle value: " + angle);
            branches[i].rotateBranch(angle);

            positionPlantOnRoot(branches[i]);
        }

        applyLeafs();
    }

    void applyLeafs()
    {
        foreach (GameObject leaf in leafs) {
            Destroy(leaf);
        }
        for (int i = 1; i < branches.Count; i++)
        {
            GameObject leafObj = Instantiate(leafPrefab);
            leafObj.transform.localScale = leafObj.transform.localScale * branches[i].getScale();
            leafObj.transform.position = branches[i].getTopPos();
            leafObj.transform.Rotate(branches[i].rotation);
            leafs.Add(leafObj);
        }
    }

}
