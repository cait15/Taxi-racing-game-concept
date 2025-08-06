using System.Collections.Generic;
using UnityEngine;

public class IgnoreBranchesManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static IgnoreBranchesManager instance;

    public List<Transform> idealGraphPath; // this is for caluclating position, it ingores branches.

    private void Awake()
    {
        instance = this;
    }
}
