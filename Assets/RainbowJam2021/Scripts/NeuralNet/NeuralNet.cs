using UnityEngine;
using System.Collections.Generic;

namespace NeuralNet
{
    public static class NNFloatExt
    {
        public static float Sigmoid(this float input)
        {
            return 1.0f / (1.0f + Mathf.Exp(-input));
        }
    }

    public class Layer
    {
        public int NumNeurons;
        public Layer PrevLayer;
        public float[,] CoefficientsWithPrevLayer;
        public float[] Inputs;
        
        public static Layer BlankRandomConnect(int neurons, Layer prevLayer, float[] inputs = null)
        {
            Layer newLayer = new Layer();
            newLayer.PrevLayer = prevLayer;
            newLayer.NumNeurons = neurons;

            // If prevLayer is null, then this is an input layer, and the values must be fed in manually.
            if(newLayer.PrevLayer != null)
            {
                newLayer.CoefficientsWithPrevLayer = new float[newLayer.NumNeurons,newLayer.PrevLayer.NumNeurons];

                for(int neuron = 0; neuron < newLayer.NumNeurons; neuron++)
                {
                    for(int prevNeuron = 0; prevNeuron < newLayer.PrevLayer.NumNeurons; prevNeuron++)
                    {
                        newLayer.CoefficientsWithPrevLayer[neuron,prevNeuron] = Random.Range(-1.0f, 1.0f);
                    }
                }
            }
            else
            {
                newLayer.Inputs = inputs;
            }

            return newLayer;
        }

        public float[] GetOutputs()
        {
            float[] outputs = new float[NumNeurons];

            for(int i = 0; i < outputs.Length; i++)
            {
                outputs[i] = GetNeuronOutput(i);
            }

            return outputs;
        }

        public float GetNeuronOutput(int neuronIndex)
        {
            float output = 0;

            if(PrevLayer != null)
            {
                for(int prevNeuronIdx = 0; prevNeuronIdx < PrevLayer.NumNeurons; prevNeuronIdx++)
                {
                    output += CoefficientsWithPrevLayer[neuronIndex,prevNeuronIdx] * PrevLayer.GetNeuronOutput(prevNeuronIdx);
                }

                output = output.Sigmoid();
            }
            else
            {
                output = Inputs[neuronIndex];
            }
            
            return output;
        }

        public (byte[], int, int) CoefficientsToGenes()
        {
            int sizeX = CoefficientsWithPrevLayer.GetLength(0);
            int sizeY = CoefficientsWithPrevLayer.GetLength(1);
            List<byte> genes = new List<byte>(sizeX * sizeY * sizeof(float));

            for(int x = 0; x < sizeX; x++)
            {
                for(int y = 0; y < sizeY; y++)
                {
                    genes.AddRange(System.BitConverter.GetBytes(CoefficientsWithPrevLayer[x,y]));
                }
            }

            return (genes.ToArray(), sizeX, sizeY);
        }

        public void CoefficientsFromGenes(byte[] genes, int sizeX, int sizeY)
        {
            CoefficientsWithPrevLayer = new float[sizeX, sizeY];

            for(int x = 0; x < sizeX; x++)
            {
                for(int y = 0; y < sizeY; y++)
                {
                    int geneOffset = x * y * sizeof(float);
                    CoefficientsWithPrevLayer[x,y] = System.BitConverter.ToSingle(genes, geneOffset);
                }
            }
        }
    }

    


    public class NeuralNetwork
    {
        public List<Layer> Layers = new List<Layer>();

        public static NeuralNetwork SimpleNetworkRandomWeight(int sizeInput, int sizeMid1, int sizeMid2, int sizeOutput)
        {
            NeuralNetwork newNet = new NeuralNetwork();

            Layer Input = Layer.BlankRandomConnect(sizeInput, null);
            Layer Middle1 = Layer.BlankRandomConnect(sizeMid1, Input);
            Layer Middle2 = Layer.BlankRandomConnect(sizeMid2, Middle1);
            Layer Output = Layer.BlankRandomConnect(sizeOutput, Middle2);

            newNet.Layers.Add(Input);
            newNet.Layers.Add(Middle1);
            newNet.Layers.Add(Middle2);
            newNet.Layers.Add(Output);

            return newNet;
        }

        public float[] SolveForInputs(float[] inputs)
        {
            Layers[0].Inputs = inputs;
            var outputLayer = Layers[Layers.Count - 1];
            
            return outputLayer.GetOutputs();
        }
    }
}