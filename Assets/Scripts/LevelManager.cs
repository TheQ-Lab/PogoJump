using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Modules")]
    [Tooltip("The Instance of the already present at Startup StartModule must be here")]
    public GameObject StartModule;
    [Tooltip("All Levels that are instantiated and spawned")]
    public List<GameObject> ModulePrefabs;
    
    [Header("Testing/Debugging")]
    [Tooltip("For one time use of in Editor instantiated and at (0,0,0) positioned Module, or its WalllLevel Object - [CAN BE LEFT EMPTY during normal Runtime]")]
    public GameObject TestHighestLowest;
    [Tooltip("Should [TestModules] be spawned instead of regular [ModulePrefabs]?")]
    public bool EnableTestModuleList;
    [Tooltip("Modules to spawn instead of [ModulePrefabs]")]
    public List<GameObject> TestModules;

    [Header("Referenzen")]
    [Tooltip("The [GameObject] which triggers the spawn of new levels")]
    public GameObject SpawnTrigger;
    public Acid acid;

    private List<GameObject> modulePool = new List<GameObject>();
    private int indexCurrentModule = -1;
    
    void Start()
    {
        if (TestHighestLowest != null) GetHighestY(TestHighestLowest);
        if (EnableTestModuleList)
        {
            if (TestModules.Count == 0)
                Debug.LogWarning("No Module in TestModules, please fill TestModules with prefab(s) or set EnableTestModuleList false!");
            else
            {
                ModulePrefabs.Clear();
                foreach(GameObject m in TestModules)
                    ModulePrefabs.Add(m);
            }
        }

        FillPool();
        //SpawnNewModule();
    }


    void FixedUpdate()
    {
        if (indexCurrentModule == -1)
        {
            if (SpawnTrigger.transform.position.y >= StartModule.transform.Find("LowestPoint").position.y)
                SpawnNewModule();
            return;
        }
        else if (SpawnTrigger.transform.position.y >= modulePool[indexCurrentModule].transform.Find("LowestPoint").position.y)
        {
            SpawnNewModule();
            if (acid.GetRising() == false)
                acid.StartRising();
            else 
                acid.RaiseVelocityByIncrement(1);
        }
    }

    private void FillPool()
    {
        foreach(GameObject prefab in ModulePrefabs)
        {
            GameObject newModule = Instantiate(prefab, new Vector3(0f, -100f, 0f), Quaternion.identity);
            modulePool.Add(newModule);
            newModule.SetActive(false);
        }
    }


    private void SpawnNewModule()
    {
        GameObject currentModule;
        if (indexCurrentModule == -1)
        {
            currentModule = StartModule;
        }
        else
        {
            currentModule = modulePool[indexCurrentModule];
        }
        Vector2 highestPointPrev = (Vector2)currentModule.transform.Find("HighestPoint").position; //absolute/worldPos
        //calculate Index of new module
        int indexNewModule = indexCurrentModule;
        while (indexNewModule == indexCurrentModule)
        {
            indexNewModule = Random.Range(0, modulePool.Count);
        }
        AttatchModule(indexNewModule, highestPointPrev);
        ResetInteractiveModuleContent(indexNewModule);
        indexCurrentModule = indexNewModule;
        Debug.Log("Spawned Module #" + indexCurrentModule);
    }

    private void AttatchModule(int ModuleIndex, Vector2 highestPointPrev)
    {
        GameObject nextModule = modulePool[ModuleIndex];

        Vector2 lowestPointNext = (Vector2)nextModule.transform.Find("LowestPoint").localPosition * nextModule.transform.localScale; //relative to Parent Position * Parent Scale.y
        nextModule.transform.position = highestPointPrev - lowestPointNext;

        nextModule.SetActive(true);
    }


    public float GetHighestY(GameObject objectThatHasARenderableComponent)
    {
        //Gives you highest y value of the rendered sprite
        Renderer rend = objectThatHasARenderableComponent.GetComponent<Renderer>();
        
        float highestY = rend.bounds.max.y / 1f /*(1f is scaling.y of Parent (Level))*/;
        Debug.Log("Testable Levels highestY: " + highestY);
        return highestY;
    }

    private void ResetInteractiveModuleContent(int moduleIndex)
    {
        GameObject m = modulePool[moduleIndex];
        foreach (Transform t in m.transform)
        {
            //Debug.LogWarning(t.name);
            if (t.name.Contains("ExtraLife"))
            {
                //Debug.LogError("Reset " + t.name + " in Module " + m.transform.name + " , Index: " + moduleIndex);
                int rand = Random.Range(0, 100);
                if (rand < 15) // "< 35" exakt 35%, da min inklusiv, max exklusiv
                    t.gameObject.SetActive(true);
                else
                    t.gameObject.SetActive(false);
                //Debug.LogWarning(rand + " 1Up");
            }
            else if (t.name.Contains("PowerUp"))
            {
                int rand = Random.Range(0, 100);
                if (rand < 25)
                    t.gameObject.SetActive(true);
                else
                    t.gameObject.SetActive(false);
                //Debug.LogWarning(rand + " PUp");
            }
            else if (t.name.StartsWith("Mask"))
            {
                t.GetComponent<Mask>().enabled = true;
                t.GetComponent<Mask>().Reset();
            }
        }
    }
}
