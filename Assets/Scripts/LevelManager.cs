using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> ModulePrefabs;
    public GameObject StartModule;
    public GameObject TestHighestLowest;
    public GameObject SpawnTrigger;
    public Acid acid;

    private List<GameObject> modulePool = new List<GameObject>();
    private int indexCurrentModule = -1;
    

    private int temp = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (TestHighestLowest != null) GetHighestY(TestHighestLowest);
        FillPool();
        //SpawnNewModule();
    }

    // Update is called once per frame
    void Update()
    {


    }

    void FixedUpdate()
    {
        temp++;
        //if (temp == 50) {  }
        if (indexCurrentModule == -1)
        {
            if (SpawnTrigger.transform.position.y >= StartModule.transform.position.y)
            {
                SpawnNewModule();
                acid.StartRising();//ONLY NEED TO ACTIVATE ONCE
            }
            return;
        }
        if (SpawnTrigger.transform.position.y >= modulePool[indexCurrentModule].transform.position.y)
        {
            SpawnNewModule();
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
        //calculate new module-no
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
                if (rand < 25) // "< 35" exakt 35%, da min inklusiv, max exklusiv
                    t.gameObject.SetActive(true);
                else
                    t.gameObject.SetActive(false);
                Debug.LogWarning(rand + " 1Up");
            }
            else if (t.name.Contains("PowerUp"))
            {
                int rand = Random.Range(0, 100);
                if (rand < 25)
                    t.gameObject.SetActive(true);
                else
                    t.gameObject.SetActive(false);
                Debug.LogWarning(rand + " PUp");
            }
        }
    }
}
