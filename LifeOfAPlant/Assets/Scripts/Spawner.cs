using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject pot;
    public GameObject stemPrefab;
    private GameObject stem;
    public GameObject branchPrefab;

    struct Branch
    {
        public Branch(float angle_in, float scale_in, GameObject gameObject_in/*, Branch* parent_in*/)
        {
            angle = angle_in;
            scale = scale_in;
            branchObject = gameObject_in;
            // parent = parent_in;

            isOuterBranch = true;
        }
        public float angle;
        public float scale;
        private GameObject branchObject;
        // public Branch* parent;

        public bool isOuterBranch;
    }

    private Branch[] branches;

    private void Start()
    {
        /* {{ Create the stem of the tree }} */
        // Get the position of the pots pivot point
        Vector3 potCenter = pot.transform.position;

        // Get the position of the pots top point
        Renderer renderer = pot.GetComponent<Renderer>();
        // Move up from half the pots height so it won't spawn in the pots center
        float potHeight = renderer.bounds.size.y;
        potCenter.y += potHeight / 2;

        // Move up from half the spawnee height so it won't spawn in the spawnee center
        renderer = stemPrefab.GetComponent<Renderer>();
        float spawneeHeight = renderer.bounds.size.y;
        potCenter.y += spawneeHeight / 2;

        // Instantiate the stem of the tree
        stem = Instantiate(stemPrefab, potCenter, pot.transform.rotation);


        /* {{ Create the branches of the tree }} */

    }

}
