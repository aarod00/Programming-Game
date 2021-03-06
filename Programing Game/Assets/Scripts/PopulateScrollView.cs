using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PopulateScrollView : MonoBehaviour
{
    Dictionary<string, float> d_Rankings = new Dictionary<string, float>();
    public GameObject listing;
    public GameObject content;
    GameObject newObj;
    public int numberOfListings;
    private Database localDB;
    public bool open;
    // Start is called before the first frame update
    void Start()
    {
        localDB = GameObject.FindGameObjectWithTag("Player").GetComponent<Database>();
        
    }

    public void GetRankings()
    {
        //Iterate over each user, collecting and calculating scores based on the following
        //(Questions correct/(Questions incorrect+Questions correct)) Note: will need to sum correct and incorrect from the three seperate difficulties
        //Beginner category has weight of 25%
        //Intermediate has weight of 30%
        //Advanced has weight of 45%
        //Multiply weighted avg by completion status
        open = !open;
        if (!open)
        {           
            foreach (Transform child in content.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return;
        }

        float weightedAvg, begAvg, intAvg, advAvg, ranking;
        foreach (KeyValuePair<string, UserData> entry in localDB.users)
        {
            if (entry.Key.Equals(""))
            {
                continue;
            }

            //Reset all values
            weightedAvg = 0.0F;
            begAvg = 0.0F;
            intAvg = 0.0F;
            advAvg = 0.0F;
            ranking = 0.0F;
            //Calculate averages
            if (entry.Value.qstCrctBeginner == 0 && entry.Value.qstWrgBeginner == 0)
            {
                begAvg = 0;
            }
            else
            {
                begAvg = (entry.Value.qstCrctBeginner) / (entry.Value.qstWrgBeginner + entry.Value.qstCrctBeginner);
            }

            if (entry.Value.qstCrctIntermediate == 0 && entry.Value.qstWrgIntermediate == 0)
            {
                intAvg = 0;
            }
            else
            {
                intAvg = (entry.Value.qstCrctIntermediate) / (entry.Value.qstWrgIntermediate + entry.Value.qstCrctIntermediate);
            }

            if (entry.Value.qstCrctAdvanced == 0 && entry.Value.qstWrgAdvanced == 0)
            {
                advAvg = 0;
            }
            else
            {
                advAvg = (entry.Value.qstCrctAdvanced) / (entry.Value.qstWrgAdvanced + entry.Value.qstCrctAdvanced);

            }

            //Calculate weighted averages
            begAvg *= 0.25F;
            intAvg *= 0.30F;
            advAvg *= 0.45F;

            //Gives current 'grade'
            weightedAvg = begAvg + intAvg + advAvg;

            //Calculate final averages based on completion %
            begAvg *= entry.Value.progressBeginner;
            intAvg *= entry.Value.progressIntermediate;
            advAvg *= entry.Value.progressAdvanced;

            //Sum final avgs for ranking
            ranking = advAvg + intAvg + begAvg;

            if (d_Rankings.ContainsKey(entry.Key))
            {
                continue;
            }
            else
            {
                d_Rankings.Add(entry.Key, ranking);
            }   
        }
        //Should sort d_Rankings to display users in descending order based on score
        var sortedRankings = (from entry in d_Rankings orderby entry.Value descending select entry);

        foreach (KeyValuePair<string, float> entry in sortedRankings)
        {            
            newObj = (GameObject)Instantiate(listing, transform);
            newObj.transform.SetParent(GameObject.FindGameObjectWithTag("ScrollView_Content").transform);
            newObj.transform.Find("Name").GetComponent<Text>().text = entry.Key;
            newObj.transform.Find("Score").GetComponent<Text>().text = entry.Value + "%";
            newObj.SetActive(true);
        }
    }
}
