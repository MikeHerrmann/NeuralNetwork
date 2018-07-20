using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkInit : MonoBehaviour
{
    float momentum = 0.5f;
    float globalError = 0;

    public Text gemeinteZahl;
    public Text erkannteZahl;
    // public GUIText testAuswertText;

   // public Button button;
   

    SensorField sensorField;
    LearnManager learnManager;
    StoreManager storeManager;

    public List<int> networkStructure;

    public Network netWork;
    public Layer outputLayer;

    public Archive patternArchive;


    public int currentTestNumber = 0;
    public int rightAnswers = 0;
    public int wrongAnswers = 0;

    List<Result> expectedResults;
    Pattern currentPattern;


    //-----------------------------------------------------

    private void Start()
    {
        //Button btn = button.GetComponent<Button>();
       // btn.onClick.AddListener(NewNetwork);

        networkStructure = new List<int> { 400, 20, 10 };

        sensorField = GetComponent<SensorField>();
        learnManager = GetComponent<LearnManager>();
        storeManager = GetComponent<StoreManager>();

        Result zero = new Result();
        zero.iD = 0;
        zero.name = "Null";
        zero.outputConfiguration = new List<float>() { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        // zero.outputConfiguration = new List<float>() { 0 , 0, 0, 0, 0};
        Result one = new Result();
        one.iD = 1;
        one.name = "Eins";
        one.outputConfiguration = new List<float>() { 0, 1, 0, 0, 0, 0, 0, 0, 0, 0 };
        //one.outputConfiguration = new List<float>() { 1 , 0, 0, 0, 0};
        Result two = new Result();
        two.iD = 2;
        two.name = "Zwei";
        two.outputConfiguration = new List<float>() { 0, 0, 1, 0, 0, 0, 0, 0, 0, 0 };
        // two.outputConfiguration = new List<float>() { 0, 1, 0, 0, 0};

        Result three = new Result();
        three.iD = 3;
        three.name = "Drei";
        three.outputConfiguration = new List<float>() { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0 };
        //three.outputConfiguration = new List<float>() { 0 , 0, 1, 0, 0};
        Result four = new Result();
        four.iD = 4;
        four.name = "Vier";
        four.outputConfiguration = new List<float>() { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 };
        // four.outputConfiguration = new List<float>() { 0 , 0, 0, 1, 0};
        Result five = new Result();
        five.iD = 5;
        five.name = "Fünf";
        five.outputConfiguration = new List<float>() { 0, 0, 0, 0, 0, 1, 0, 0, 0, 0 };
        //five.outputConfiguration = new List<float>() { 0 , 0, 0, 0, 1};
        Result six = new Result();
        six.iD = 6;
        six.name = "Sechs";
        six.outputConfiguration = new List<float>() { 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 };
        //six.outputConfiguration = new List<float>() { 1 , 1, 0, 0, 0};
        Result seven = new Result();
        seven.iD = 7;
        seven.name = "Sieben";
        seven.outputConfiguration = new List<float>() { 0, 0, 0, 0, 0, 0, 0, 1, 0, 0 };
        //seven.outputConfiguration = new List<float>() { 1 , 0, 1, 0, 0};
        Result eight = new Result();
        eight.iD = 8;
        eight.name = "Acht";
        eight.outputConfiguration = new List<float>() { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 };
        //eight.outputConfiguration = new List<float>() { 1 , 0, 0, 1, 0};
        Result nine = new Result();
        nine.iD = 9;
        nine.name = "Neun";
        nine.outputConfiguration = new List<float>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
        //nine.outputConfiguration = new List<float>() {1, 0, 0, 0, 1};

        expectedResults = new List<Result>() { zero, one, two, three, four, five, six, seven, eight, nine };

    }

    //------------------------------------------

    public Network CreateNetwork()
    {
        netWork = new Network();
        netWork.structure = networkStructure;
        netWork.layers = new List<Layer>();
        netWork.expectedResults = expectedResults;

        for (int i = 0; i < networkStructure.Count; i++)
        {
            Layer newLayer = new Layer();
            newLayer.index = i;
            newLayer.numNeurons = networkStructure[i];
            newLayer.neurons = new List<Neuron>();

            netWork.layers.Add(newLayer);

            for (int j = 0; j < newLayer.numNeurons; j++)
            {
                Neuron newNeuron = new Neuron();

                newNeuron.inputWeights = new List<float>();
                newLayer.neurons.Add(newNeuron);
                if (i > 0)
                {

                    for (int k = 0; k < netWork.layers[i - 1].neurons.Count; k++)
                    {
                        float inputWeight = Random.Range(0f, 1f);
                        newNeuron.inputWeights.Add(inputWeight);
                    }

                    float bias = Random.Range(0f, 1f);
                    newNeuron.bias = bias;

                }

            }
        }
        
        return netWork;
    }

    //----------------------------------------------


    void LearnIndividualPattern()
    {
        if (currentTestNumber < 10)
        {
            Pattern newPattern = new Pattern();
            newPattern.values = new List<float>();
            Correction();
            CalculateNetworkOutput();
            Visualize();
            for (int i = 0; i < netWork.layers[0].neurons.Count; i++)
            {
                Neuron neuron = netWork.layers[0].neurons[i];
                newPattern.values.Add(neuron.activationValue);
            }
            Folder folder = storeManager.exercisePattern.folders[currentTestNumber];
            folder.patterns.Add(newPattern);
            //learnManager.ShowExerciseArchive();
            //storeManager.SavePatternArchive(storeManager.exercisePattern, "ExercisePattern.xml");
            //storeManager.SavePatternArchive(storeManager.testPattern, "TestPattern.xml");
        }
        else
        {
            gemeinteZahl.text = "???";
        }
    }

    //--------------------------------------------------

    public void RecognizeData()
    {
        if (currentTestNumber < 10)
        {
            GetDirectInput();
            CalculateNetworkOutput();
            Visualize();
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        else
        {
            gemeinteZahl.text = "???";
        }
    }

    //------------------------------------------------


    public void LearnData()
    {
        GetDirectInput();
        CalculateNetworkOutput();
        Visualize();
        Correction();
        CalculateNetworkOutput();
        Visualize();
       // Folder folder = storeManager.patternArchive.folders[currentTestNumber];
       // folder.patterns.Add(pattern);
    }

    //-------------------------------------------------



    public void GetDirectInput()
    {
        Folder folder = storeManager.exercisePattern.folders[currentTestNumber];
        int index = (Random.Range(0, folder.patterns.Count - 1));
        Pattern pattern = folder.patterns[index];
        currentPattern = pattern;
        // Pattern pattern = folder.patterns[1];

        for (int i = 0; i < netWork.layers[0].neurons.Count; i++)
        {
            Neuron neuron = netWork.layers[0].neurons[i];
            neuron.activationValue = pattern.values[i];  
        }
        
    }

    //---------------------------------------------------

        void RemovePatternFromArchive()
    {
        Folder folder = storeManager.exercisePattern.folders[currentTestNumber];
        folder.patterns.Remove(currentPattern);

        //storeManager.SavePatternArchive(storeManager.exercisePattern, "ExercisePattern.xml");
        //storeManager.SavePatternArchive(storeManager.testPattern, "TestPattern.xml");
    }

    //--------------------------------------------------

    public void GetDrawInput()
    {

        for (int i = 0; i < netWork.layers[0].neurons.Count; i++)
        {
            Neuron neuron = netWork.layers[0].neurons[i];
            neuron.activationValue = sensorField.sensorFields[i].drawValue;
        }
        CalculateNetworkOutput();
        Visualize();
    }

    //---------------------------------------------------


    public void CalculateNetworkOutput()
    {

        for (int i = 1; i < netWork.layers.Count; i++)
        {
            Layer layer = netWork.layers[i];
            Layer previousLayer = netWork.layers[i - 1];

            for (int j = 0; j < layer.neurons.Count; j++)
            {
                Neuron neuron = layer.neurons[j];
                float totalInput = 0;

                for (int k = 0; k < previousLayer.neurons.Count; k++)
                {
                    Neuron previousLayerNeuron = previousLayer.neurons[k];
                    float correspondingWeight = neuron.inputWeights[k];

                    totalInput += previousLayerNeuron.activationValue * correspondingWeight;

                }
                totalInput += neuron.bias;
                // neuron.activationValue = Sigmoid(totalInput);
                // neuron.activationValue = ReLu(totalInput / previousLayer.neurons.Count);
                neuron.activationValue = totalInput / previousLayer.neurons.Count;
            }
        }
    }

    //-------------------------------------------------


    public void Correction()
    {
        List<float> expectedResult = netWork.expectedResults[currentTestNumber].outputConfiguration;


        for (int i = 0; i < outputLayer.neurons.Count; i++)
        {
            Neuron currentNeuron = outputLayer.neurons[i];
            globalError = expectedResult[i] - currentNeuron.activationValue;
            BackpropagationRecursive(currentNeuron, outputLayer, globalError);

        }

    }

    //----------------------------------------

    void Backpropagation(Neuron outputNeuron, float globalError)
    {


        Layer inputLayer = netWork.layers[0];
        Layer hiddenLayer = netWork.layers[1];

        for (int i = 0; i < outputNeuron.inputWeights.Count; i++)
        {
            outputNeuron.inputWeights[i] += globalError * hiddenLayer.neurons[i].activationValue * momentum;
        }
        outputNeuron.bias += globalError * momentum;

        for (int i = 0; i < hiddenLayer.neurons.Count; i++)
        {
            Neuron neuron = hiddenLayer.neurons[i];
            // float error = Derivative(neuron.activationValue) * globalError * outputNeuron.inputWeights[i];
            float error = globalError * outputNeuron.inputWeights[i];
            for (int j = 0; j < neuron.inputWeights.Count; j++)
            {
                neuron.inputWeights[j] += error * inputLayer.neurons[j].activationValue * momentum;
            }
            neuron.bias += error * momentum;
        }
    }

    //------------------------------------------

    void BackpropagationRecursive(Neuron neuron, Layer layer, float error)
    {

        Layer previousLayer = netWork.layers[layer.index - 1];
        neuron.bias += error * momentum;

        for (int i = 0; i < previousLayer.neurons.Count; i++)
        {
            Neuron previousLayerNeuron = previousLayer.neurons[i];
            float previousError = error * neuron.inputWeights[i];
            neuron.inputWeights[i] += error * previousLayerNeuron.activationValue * momentum;
            if (layer.index > 1)
            {
                BackpropagationRecursive(previousLayerNeuron, previousLayer, previousError);
            }
        }

    }

    //-------------------------------------------

    // Berechnet den den Input aufgrund der aktuellen Wichtungen bei gegebenem Output

    void Backview()
    {
        if (currentTestNumber < 10)
        {
            CleanActivations();
            for (int i = 0; i < outputLayer.neurons.Count; i++)
            {
                outputLayer.neurons[i].activationValue = expectedResults[currentTestNumber].outputConfiguration[i];
            }
            CalculateNetworkInput();
            Visualize();
        }
        else
        {
            gemeinteZahl.text = "???";
        }
    }

    //-------------------------------------------



    public void CalculateNetworkInput()
    {

        for (int i = netWork.layers.Count - 1; i > 0; i--)
        {
            Layer layer = netWork.layers[i];
            Layer previousLayer = netWork.layers[i - 1];

            for (int j = 0; j < layer.neurons.Count; j++)
            {
                Neuron neuron = layer.neurons[j];


                for (int k = 0; k < previousLayer.neurons.Count; k++)
                {
                    Neuron previousLayerNeuron = previousLayer.neurons[k];
                    float correspondingWeight = neuron.inputWeights[k];

                    previousLayerNeuron.activationValue += neuron.activationValue * correspondingWeight / layer.neurons.Count / Mathf.Sqrt(layer.neurons.Count) * 2;
                   
                }
            }
        }
    }

    //-------------------------------------------------

    public float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }



    public float Derivative(float x)
    {
        float s = Sigmoid(x);
        float temp = 1 - s;
        return (s * temp) / 100;
    }



    float ReLu(float x)
    {
        return (x >= 0) ? x * momentum : 0;
    }

    //-----------------------------------------------

    public void Visualize()
    {
        // Folder folder = patternArchive.folders[currentTestNumber];
        // Pattern pattern = folder.patterns[0];

        for (int i = 0; i < netWork.layers[0].neurons.Count; i++)
        {
            Neuron neuron = netWork.layers[0].neurons[i];
            float colVal = 1 - neuron.activationValue;
            Color color = new Color(colVal, colVal, colVal, 1);
            sensorField.sensorFields[i].groundStone.GetComponent<Renderer>().material.color = color;
        }

        for (int i = 1; i < netWork.layers.Count - 1; i++)
        {
            Layer currentLayer = netWork.layers[i];
            for (int j = 0; j < currentLayer.neurons.Count; j++)
            {
                float activationValue = currentLayer.neurons[j].activationValue;
                float colFac = 1 - Mathf.Clamp(activationValue, 0f, 1f);
                Color color = new Color(colFac, colFac, colFac, 1F);
                sensorField.hiddenFields[(i - 1) * 60 + j].groundStone.GetComponent<Renderer>().material.color = color;
            }
        }

        for (int i = 0; i < netWork.layers[netWork.layers.Count - 1].neurons.Count; i++)
        {
            float activationValue = netWork.layers[netWork.layers.Count - 1].neurons[i].activationValue;
            float colFac = 1 - Mathf.Clamp(activationValue, 0f, 1f);
            Color color = new Color(colFac, colFac, colFac, 1F);
            sensorField.outputFields[i].groundStone.GetComponent<Renderer>().material.color = color;
        }

        Result result = CheckResult();
        
        erkannteZahl.text = result.iD.ToString();

    }

    //-----------------------------------------------------------------------------------

    void CleanActivations()
    {
        for (int i = 0; i < netWork.layers.Count; i++)
        {
            Layer layer = netWork.layers[i];
            for (int j = 0; j < layer.neurons.Count; j++)
            {
                Neuron neuron = layer.neurons[j];
                neuron.activationValue = 0;
            }
        }
    }

    //------------------------------------------------------------------------------------

    Result CheckResult()
    {
        List<float> errors = new List<float>();
        int index = 1000000;
        float smallestError = 1000000;
        for (int i = 0; i < expectedResults.Count; i++)
        {
            List<float> testResult = expectedResults[i].outputConfiguration;
            float error = 0;
            for (int k = 0; k < outputLayer.neurons.Count; k++)
            {
                float tempError = Mathf.Abs(testResult[k] - outputLayer.neurons[k].activationValue);
                error += tempError;
            }
            errors.Add(error);
        }
        for (int i = 0; i < errors.Count; i++)
        {
            if (errors[i] < smallestError)
            {
                smallestError = errors[i];
                index = i;
            }
        }
        Result result = expectedResults[index];
        EvalResult(result);
        return result;
    }

    //---------------------------------------------------------

    public void EvalResult(Result result)
    {
        if (result.iD == currentTestNumber)
        {
            rightAnswers++;
        }
        else
        {
            wrongAnswers++;
        }

    }

    //-------------------------------------------------------------------------------

    private void Update()
    {

        if (Input.GetKeyDown("0"))
        {
            currentTestNumber = 0;
            gemeinteZahl.text = currentTestNumber.ToString();
        }

        if (Input.GetKeyDown("1"))
        {
            currentTestNumber = 1;
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        if (Input.GetKeyDown("2"))
        {
            currentTestNumber = 2;
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        if (Input.GetKeyDown("3"))
        {
            currentTestNumber = 3;
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        if (Input.GetKeyDown("4"))
        {
            currentTestNumber = 4;
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        if (Input.GetKeyDown("5"))
        {
            currentTestNumber = 5;
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        if (Input.GetKeyDown("6"))
        {
            currentTestNumber = 6;
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        if (Input.GetKeyDown("7"))
        {
            currentTestNumber = 7;
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        if (Input.GetKeyDown("8"))
        {
            currentTestNumber = 8;
            gemeinteZahl.text = currentTestNumber.ToString();
        }
        if (Input.GetKeyDown("9"))
        {
            currentTestNumber = 9;
            gemeinteZahl.text = currentTestNumber.ToString();
        }

        //-------------------------------

        if (Input.GetKeyDown("s"))
        {
            RecognizeData();
        }
/* 
        if (Input.GetKeyDown("d"))
        {
            RemovePatternFromArchive();
        }

        if (Input.GetKeyDown("c"))
        {
            Correction();
        }
*/
        if (Input.GetKeyDown("l"))
        {
            LearnIndividualPattern();

        }

        if (Input.GetKeyDown("b"))
        {
            Backview();

        }


        if (Input.GetKeyDown("e"))
        {
            learnManager.StartExercise();
        }

        if (Input.GetKeyDown("t"))
        {
            learnManager.StartTest();
        }
/* 
        if (Input.GetKeyDown("s"))
        {
            storeManager.SaveNetwork();
        }

        if (Input.GetKeyDown("w"))
        {
            learnManager.ToggleExerciseAndTestPatterns();
        }
*/
    }
}

