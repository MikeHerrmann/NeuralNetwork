using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Field
{
    public Vector2 coords = Vector2.zero;
    public GameObject groundStone;
    public float drawValue = 0;
}

public class Network
{
    public List<int> structure;
    public List<Layer> layers;
    public int trainingRounds;
    public List<Result> expectedResults;
}

//------------------------------------

public class Layer
{
    public int index;
    public int numNeurons;
    public List<Neuron> neurons;
   
}

//-----------------------------------

public class Neuron
{
    public List<float> inputWeights;
    //public float weight = 0;
    public float bias = 0;
    public float activationValue;

}

public class Result
{
    public int iD;
    public string name;
    public List<float> outputConfiguration;

}

//Übungsvorlagen

public class Pattern
{
    public List<float> values = new List<float>();
}

public class Folder
{
    public int iD;
    public List<Pattern> patterns = new List<Pattern>();
}

public class Archive
{
    public List<Folder> folders = new List<Folder>();
}