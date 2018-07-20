using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LearnManager : MonoBehaviour
{
    public Text textField;

    NetworkInit networkInit;
    SensorField sensorField;
    StoreManager storeManager;
    int roundsOfExercise = 0;
    int thisPatternRounds = 0;
    int roundsOfTest = 0;
    int currentExerciseSet = 1;
    int currentNumber = 0;
    WaitForSeconds delay;

    void Start()
    {

        networkInit = GetComponent<NetworkInit>();
        sensorField = GetComponent<SensorField>();
        storeManager = GetComponent<StoreManager>();

    }

    //------------------------------------------

    IEnumerator Exercise()
    {
        while (roundsOfExercise < 100)
        {

            networkInit.currentTestNumber = currentNumber;
            networkInit.GetDirectInput();
            networkInit.CalculateNetworkOutput();
            networkInit.Correction();
            networkInit.Visualize();
            networkInit.gemeinteZahl.text = networkInit.currentTestNumber.ToString();

            currentNumber++;

            if (currentNumber > 9)
            {
                currentNumber = 0;
                roundsOfExercise++;
                thisPatternRounds++;
                networkInit.netWork.trainingRounds++;

                textField.text = "Trainingsrunden insgesamt: " + networkInit.netWork.trainingRounds + "\nAktuelle Trainingsrunde: " + roundsOfExercise.ToString();

                if (thisPatternRounds >= 50)
                {
                    ToggleExerciseAndTestPatterns();
                    thisPatternRounds = 0;
                }

                
            }

            yield return null;
        }
        //storeManager.SaveNetwork();
        textField.text = "Trainingsrunden insgesamt: " + networkInit.netWork.trainingRounds + "\n" + roundsOfExercise + "Trainings-Runden komplett";
    }

    //---------------------------------------

    public void StartExercise()
    {
        StopAllCoroutines();
        roundsOfExercise = 0;
        thisPatternRounds = 0;
        currentNumber = 0;
        StartCoroutine(Exercise());

    }

    //---------------------------------------------

    IEnumerator Test()
    {
        while (roundsOfTest < 30)
        {
            networkInit.currentTestNumber = currentNumber;
            networkInit.GetDirectInput();
            networkInit.CalculateNetworkOutput();
            networkInit.Visualize();
            networkInit.gemeinteZahl.text = networkInit.currentTestNumber.ToString();
            textField.text = textField.text = "Test-Runde: " + roundsOfTest + "\nFalsch: " + networkInit.wrongAnswers.ToString() + "\nRichtig: " + networkInit.rightAnswers.ToString();

            currentNumber++;

            if (currentNumber > 9)
            {
                currentNumber = 0;
                roundsOfTest++;
                thisPatternRounds++;
                if (thisPatternRounds >= 15)
                {
                    ToggleExerciseAndTestPatterns();
                    thisPatternRounds = 0;
                }

            }
            yield return null;
        }
        int gesamt = networkInit.rightAnswers + networkInit.wrongAnswers;
        float fehlerQuote = Percent(gesamt, networkInit.wrongAnswers);
       
        textField.text = textField.text =  roundsOfTest + "Test-Runden komplett\nFalsch: " + networkInit.wrongAnswers.ToString() + "\nRichtig: " + networkInit.rightAnswers.ToString() + "\nFehlerquote: " + fehlerQuote.ToString("0.0") + " %";
    }

    //---------------------------------------

   
    public void StartTest()
    {
        StopAllCoroutines();
        networkInit.rightAnswers = 0;
        networkInit.wrongAnswers = 0;
        roundsOfTest = 0;
        currentNumber = 0;
        StartCoroutine(Test());

    }

    //---------------------------------------------

    public void ToggleExerciseAndTestPatterns()
    {
        currentExerciseSet = -(currentExerciseSet - 1) + 2;

        Archive tempArchive = storeManager.testPattern;
        storeManager.testPattern = storeManager.exercisePattern;
        storeManager.exercisePattern = tempArchive;
        //storeManager.SavePatternArchive(storeManager.exercisePattern, "ExercisePattern.xml");
        //storeManager.SavePatternArchive(storeManager.testPattern, "TestPattern.xml");
        ShowExerciseArchive();

    }

    //---------------------------------------------------------------------

    public void ShowExerciseArchive()
    {
        textField.text = "Aktuelles Trainingsset: " + currentExerciseSet.ToString() + "\n\n";
        for (int i = 0; i < storeManager.exercisePattern.folders.Count; i++)
        {
            Folder folder = storeManager.exercisePattern.folders[i];
            textField.text += folder.iD + ": " + folder.patterns.Count.ToString() + "\n";
        }
    }

    //---------------------------------------

    float Percent(float gesamt, float teil)
    {
        float centVal = 100 / gesamt;
        float percentVal = teil * centVal;
        return percentVal;
    }
}
