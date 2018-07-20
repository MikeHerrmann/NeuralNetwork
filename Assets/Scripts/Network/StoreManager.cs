
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml.Serialization;


public class StoreManager : MonoBehaviour
{
    public Text loadProgress;

    string streamingAssetsPath;
  
    public Archive exercisePattern;
    public Archive testPattern;
    List<Archive> patternArchiveList;
    public Network netWork;

    NetworkInit netWorkInit;
    SensorField sensorField;
    LearnManager learnManager;

    //---------------------------------------------

    void Start()
    {

        patternArchiveList = new List<Archive>();

       // streamingAssetsPath = Application.streamingAssetsPath + "/";
        streamingAssetsPath = "http://jojo-studio.de/Projekte/NeuralNetwork/StreamingAssets/";


        netWorkInit = GetComponent<NetworkInit>();
        sensorField = GetComponent<SensorField>();
        learnManager = GetComponent<LearnManager>();

       StartCoroutine(LoadNetworkFromWeb());
        ///StartCoroutine(LoadPatternArchive("ExercisePattern"));
        //StartCoroutine(LoadPatternArchive("TestPattern"));
        StartCoroutine(WaitForLoadingFiles());
    }

    //----------------------------------------------------

    IEnumerator LoadNetworkFromWeb()

    {
        string filePath = Path.Combine(streamingAssetsPath, "Network");
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        loadProgress.text = "Netzwerk laden...";
        yield return www.SendWebRequest();

        byte[] result = www.downloadHandler.data;

        using (MemoryStream reader = new MemoryStream(result))
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Network));
            netWork = (xmlSerializer.Deserialize(reader) as Network);
        }
        
        netWorkInit.netWork = netWork;
        netWorkInit.outputLayer = netWorkInit.netWork.layers[netWorkInit.netWork.layers.Count - 1];

        StartCoroutine(LoadPatternArchive("ExercisePattern"));

    }

    //-------------------------------------------------------------------


    IEnumerator LoadPatternArchive(string fileName)
    {

        string filePath = Path.Combine(streamingAssetsPath, fileName);
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        loadProgress.text = fileName + " laden...";
        yield return www.SendWebRequest();

        byte[] result = www.downloadHandler.data;

        using (MemoryStream reader = new MemoryStream(result))
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Archive));
            Archive patternArchive = (xmlSerializer.Deserialize(reader) as Archive);
            patternArchiveList.Add(patternArchive);
            if (patternArchiveList.Count < 2)
            {
                StartCoroutine(LoadPatternArchive("TestPattern"));
            }
        }

       
    }

    //----------------------------------------------------------------------

    IEnumerator WaitForLoadingFiles()
    {
        while (patternArchiveList.Count < 2)
        {
            
            yield return null;
        }
       
        exercisePattern = patternArchiveList[0];
        testPattern = patternArchiveList[1];
        sensorField.CreateSensorField();
        loadProgress.enabled = false;
       
    }

}
