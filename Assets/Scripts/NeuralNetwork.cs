using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralNetwork
{
    public int[] layers;
    public float[][] neurons;
    public float[][][] connections;

    #region generic functions

    //Contructor which get the layers as an input and creates random connections between all neurons in the layers
    public NeuralNetwork(int[] mylayers)
    {
        layers = mylayers;
        InitNeurons();
        InitRandomConnections();
    }

    private void InitNeurons()
    {
        neurons = new float[layers.Length][];

        for (int i = 0; i < layers.Length; i++)
        {
            neurons[i] = new float[layers[i]];
        }
    }

    private void InitRandomConnections()
    {
        connections = new float[layers.Length][][];

        for (int i = 0; i < layers.Length; i++)
        {
            connections[i] = new float[layers[i]][];

            for (int j = 0; j < layers[i]; j++)
            {
                if (i > 0)
                {
                    connections[i][j] = new float[layers[i - 1]];
                    for (int k = 0; k < layers[i - 1]; k++)
                    {
                        connections[i][j][k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                    }
                }
            }
        }
    }

    //Feeds the input(s) through the neural network to return the output(s)
    public float[] FireNeurons(float[] input)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                if (i == 0)
                {
                    neurons[i][j] = input[j];
                }
                else
                {
                    neurons[i][j] = 0;
                    for (int k = 0; k < layers[i - 1]; k++)
                    {
                        // Linear activation function, Tanh or Sigmoid are also possible
                        neurons[i][j] += neurons[i - 1][k] * connections[i][j][k];
                    }
                }
            }
        }
        return neurons[layers.Length - 1];
    }

    public float[][][] getConnections()
    {
        return connections;
    }

    #endregion

    #region evolvement functions

    // Similar contructor as public NeuralNetwork(int[] mylayers) but uses an exisitng neural network and randomly mutatates some connections
    public NeuralNetwork(NeuralNetwork toCopy, float mutationChance)
    {
        layers = new int[toCopy.layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = toCopy.layers[i];
        }
        InitNeurons();
        MutateConnections(toCopy.getConnections(), mutationChance);
    }

    // Randomly mutatates some connections
    private void MutateConnections(float[][][] copyConnections, float mutationChance)
    {
        connections = new float[layers.Length][][];

        for (int i = 0; i < layers.Length; i++)
        {
            connections[i] = new float[layers[i]][];

            for (int j = 0; j < layers[i]; j++)
            {
                if (i > 0)
                {
                    connections[i][j] = new float[layers[i - 1]];
                    for (int k = 0; k < layers[i - 1]; k++)
                    {
                        //Uses a random multiplication factor the mutate the connection weight
                        float randomMutation = GetRandomMutation(mutationChance);
                        connections[i][j][k] = copyConnections[i][j][k] * randomMutation;
                    }
                }
            }
        }
    }

    //Mostly returns 1 but sometimes a random factor which is multiplied with the connection weight
    private float GetRandomMutation(float mutationChance)
    {
        float randomFloat = UnityEngine.Random.Range(0f, 1f);

        if (randomFloat < mutationChance)
        {
            //Using a multiplication of random range to make extreme mutations less likely and little mutations more likely
            return UnityEngine.Random.Range(-1.5f, 1.5f) * UnityEngine.Random.Range(-1.5f, 1.5f);
        }
        else
        {
            return 1;
        }
    }

    #endregion
}



