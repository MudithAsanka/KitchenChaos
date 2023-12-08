 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
    // This class is responsible for Reset Static Data listeners,
    // Otherwise when loading again there are more listeners,
    // MissingReferenceExceptions because of destroyed gameobjects still trying to access

    private void Awake()
    {
        BaseCounter.ResetStaticData();
        CuttingCounter.ResetStaticData();
        TrashCounter.ResetStaticData();
    }
}
