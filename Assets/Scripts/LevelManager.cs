using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [System.Serializable]
    public class Transition
    {
        public GameObject transitionModule;
        public BiomeType.Biome biomeLower;
        public BiomeType.Biome biomeUpper;

        public Transition(GameObject _transitionModule, BiomeType.Biome _biomeLower, BiomeType.Biome _biomeUpper)
        {
            transitionModule = _transitionModule;
            biomeLower = _biomeLower;
            biomeUpper = _biomeUpper;
        }

        public static Transition zero()
        {
            Transition ret = new Transition(null, BiomeType.Biome.Cave, BiomeType.Biome.Jungle);
            return ret;
        }
    }

    [Header("Modules")]
    [Tooltip("The Instance of the already present at Startup StartModule must be here")]
    public GameObject StartModule;
    [Tooltip("All Levels that are instantiated and spawned")]
    public List<GameObject> ModulePrefabs;
    [Tooltip("All Transitions between LevelModules")]
    public List<Transition> Transitions;
    
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
    public List<Transition> transitionPool = new List<Transition>();
    private int indexCurrentModule = -1;
    
    void Start()
    {
        if (TestHighestLowest != null) GetHighestY(TestHighestLowest);
        if (EnableTestModuleList)
        {
            if (TestModules.Count < 2)
                Debug.LogWarning("No Module or only one in TestModules, please fill TestModules with prefab(s) or set EnableTestModuleList false!");
            else
            {
                ModulePrefabs.Clear();
                foreach(GameObject m in TestModules)
                    ModulePrefabs.Add(m);
            }
        }

        FillPools();
        SpawnNewModule(); //if(indexCurrentModule == -1)
    }


    void FixedUpdate()
    {
        if (SpawnTrigger.transform.position.y >= modulePool[indexCurrentModule].transform.position.y)
        {
            SpawnNewModule();
            if (acid.GetRising() == false)
                acid.StartRising();
            else 
                acid.RaiseVelocityByIncrement(1);
        }
    }

    private void FillPools()
    {
        foreach(GameObject prefab in ModulePrefabs)
        {
            GameObject newModule = Instantiate(prefab, new Vector3(0f, -100f, 0f), Quaternion.identity);
            modulePool.Add(newModule);
            newModule.SetActive(false);
        }

        foreach(Transition transition in Transitions)
        {
            GameObject newTransitionObject = Instantiate(transition.transitionModule, new Vector3(0f, -100f, 0f), Quaternion.identity);
            transitionPool.Add(new Transition(newTransitionObject, transition.biomeLower, transition.biomeUpper));
            newTransitionObject.SetActive(false);
        }
    }


    private void SpawnNewModule()
    {
        GameObject currentModule;
        if (indexCurrentModule == -1)
            currentModule = StartModule;
        else
            currentModule = modulePool[indexCurrentModule];
        Vector2 highestPointPrev = (Vector2)currentModule.transform.Find("HighestPoint").position; //absolute/worldPos

        //calculate Index of new module
        int indexNewModule = indexCurrentModule;
        while (indexNewModule == indexCurrentModule)
            indexNewModule = Random.Range(0, modulePool.Count);

        if(modulePool[indexNewModule].GetComponent<BiomeType>().biome == currentModule.GetComponent<BiomeType>().biome)
            // No Biome change
            AttatchModule(modulePool[indexNewModule], highestPointPrev);
        else
        {
            //VORLÄUFIG
            BiomeType.Biome biomeLower = currentModule.GetComponent<BiomeType>().biome;
            BiomeType.Biome biomeUpper = modulePool[indexNewModule].GetComponent<BiomeType>().biome;

            Transition newTransition = Transition.zero();
            foreach(Transition t in transitionPool)
            {
                if(t.biomeLower == biomeLower && t.biomeUpper == biomeUpper)
                {
                    newTransition = t;
                    break;
                }
            }

            AttatchModule(newTransition.transitionModule, highestPointPrev);
            highestPointPrev = newTransition.transitionModule.transform.Find("HighestPoint").position;

            AttatchModule(modulePool[indexNewModule], highestPointPrev);
        }

        ResetInteractiveModuleContent(indexNewModule);
        indexCurrentModule = indexNewModule;
        Debug.Log("Spawned Module #" + indexCurrentModule);
    }

    private void AttatchModule(GameObject nextModule, Vector2 highestPointPrev)
    {
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
