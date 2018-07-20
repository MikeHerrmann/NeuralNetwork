using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SensorField : MonoBehaviour
{
    public GameObject groundStonePrefab;
    public Material brightGrayMat;
    public Material darkGrayMat;

    public Camera cam;

    public List<Field> sensorFields;
    public List<Field> hiddenFields;
    public List<Field> outputFields;

    public Archive archive;
    public Archive testPattern;

    NetworkInit netWorkInit;
    LearnManager learnManager;
    StoreManager storeManager;

    float edgeLength;
    float edgeWidth;
    float gridWidth;
    float xOffset;

    public GameObject inputLayerText;
    public GameObject hiddenLayerText;
    public GameObject outputLayerText;
    public GameObject introText;
    //---------------------------------------------------------

    void Start()
    {
        netWorkInit = GetComponent<NetworkInit>();
        learnManager = GetComponent<LearnManager>();
        storeManager = GetComponent<StoreManager>();


        sensorFields = new List<Field>();
        hiddenFields = new List<Field>();
        outputFields = new List<Field>();

        int inputFieldNumb = netWorkInit.networkStructure[0];
        edgeLength = Mathf.Sqrt(inputFieldNumb);
        edgeWidth = edgeLength;
        gridWidth = 1f;
        xOffset = gridWidth / 2f;

        Vector3 camPosition = new Vector3(edgeWidth * gridWidth * 1.3f, edgeLength * gridWidth * 1.5f, -edgeLength * gridWidth / 2f + 1f);
        cam.transform.position = camPosition;
        Vector3 camRotation = new Vector3(90, 0, 0);
        cam.transform.rotation = Quaternion.Euler(camRotation);

        
    }

    //----------------------------------------------------------------

    public void CreateSensorField()
    {

        inputLayerText.SetActive(true);
        hiddenLayerText.SetActive(true);
        outputLayerText.SetActive(true);
        introText.SetActive(true);

        for (int i = 0; i < edgeLength; i++)

        {
            for (int j = 0; j < edgeWidth; j++)

            {
                Field newField = new Field();


                GameObject newGroundStone = Instantiate(groundStonePrefab);

                Vector3 newPosition = new Vector3(j * gridWidth + xOffset, -0.1f, -i * gridWidth - gridWidth / 2);
                newGroundStone.transform.position = newPosition;
                newGroundStone.transform.parent = transform;
                newField.coords = new Vector2(j, i);
                newField.groundStone = newGroundStone;

                sensorFields.Add(newField);

            }
        }


        inputLayerText.transform.position = new Vector3(xOffset, 0f, -edgeWidth);
        introText.transform.position = new Vector3(xOffset + edgeWidth / 2f, 0f, -edgeWidth / 2f);

        xOffset += edgeWidth + 4;


        int fieldNumb = netWorkInit.networkStructure[1];
        for (int j = 0; j < fieldNumb; j++)
        {
            Field newHiddenField = new Field();

            GameObject newGroundStone = Instantiate(groundStonePrefab);
            Vector3 newPosition = new Vector3(xOffset, -0.1f, -j * gridWidth - gridWidth / 2);
            newGroundStone.transform.position = newPosition;
            newGroundStone.GetComponent<Renderer>().material.color = Color.white;
            newGroundStone.transform.parent = transform;
            newHiddenField.groundStone = newGroundStone;
            hiddenFields.Add(newHiddenField);
           
        }

        hiddenLayerText.transform.position = new Vector3(xOffset + 1f, 0, -20f);
        xOffset += 5;

        fieldNumb = netWorkInit.networkStructure[netWorkInit.networkStructure.Count-1];
        for (int i = 0; i < fieldNumb; i++)
        {
            Field newOutputField = new Field();
            GameObject newGroundStone = Instantiate(groundStonePrefab);
            Vector3 newPosition = new Vector3(xOffset, -0.1f, -i * gridWidth - gridWidth / 2);
            newGroundStone.transform.position = newPosition;
            newGroundStone.transform.parent = transform;
            newOutputField.groundStone = newGroundStone;
            outputFields.Add(newOutputField);
        }

        outputLayerText.transform.position = new Vector3(xOffset + 1f, 0, -10f);

    }

    //-------------------------------------------------------------------

    void MouseTrack()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50))
        {
            GameObject hitGroundstone = hit.collider.gameObject;

            hitGroundstone.GetComponent<Renderer>().material.color = Color.black;
            for (int i = 0; i < sensorFields.Count; i++)
            {
                if (hitGroundstone == sensorFields[i].groundStone)
                {
                    sensorFields[i].drawValue = 1;
                }
            }
        }
    }

    //------------------------------------------------------------------

    void MouseErase()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 50))
        {
            GameObject hitGroundstone = hit.collider.gameObject;

            hitGroundstone.GetComponent<Renderer>().material.color = Color.white;
            for (int i = 0; i < sensorFields.Count; i++)
            {
                if (hitGroundstone == sensorFields[i].groundStone)
                {
                    sensorFields[i].drawValue = 0;
                }
            }
        }
    }

    //------------------------------------------------------------------

    void ClearSensorFields()
    {
        for (int i = 0; i < sensorFields.Count; i++)
        {
            sensorFields[i].groundStone.GetComponent<Renderer>().material.color = Color.white;
            sensorFields[i].drawValue = 0;
        }
    }

   
    //------------------------------------------------------------------

    public void ShowPattern(int number)
    {
        Folder folder = storeManager.testPattern.folders[number];
        int index = (Random.Range(0, folder.patterns.Count - 1));
        Pattern pattern = folder.patterns[index];
        for (int i = 0; i < sensorFields.Count; i++)
        {
            float colVal = pattern.values[i];
            Color color = new Color(colVal, colVal, colVal, 1);
            sensorFields[i].groundStone.GetComponent<Renderer>().material.color = color;
            sensorFields[i].drawValue = colVal;
        }
    }

    //------------------------------------------------------------------

    private void Update()
    {

        if (Input.GetMouseButton(0))
        {
            introText.SetActive(false);
            learnManager.StopAllCoroutines();
            netWorkInit.currentTestNumber = 10;
            netWorkInit.gemeinteZahl.text = "?";
            netWorkInit.erkannteZahl.text = "";
            MouseTrack();
        }
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            netWorkInit.GetDrawInput();
        }

        if (Input.GetMouseButton(1))
        {
            learnManager.StopAllCoroutines();
            MouseErase();
        }

        if (Input.GetKeyDown("space"))
        {
            learnManager.StopAllCoroutines();
            ClearSensorFields();
        }




    }

    //-------------------------------------------------------------------
}


