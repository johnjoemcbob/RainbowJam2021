using UnityEngine;
using System.Collections.Generic;

namespace NeuralNet
{
    public struct Neuron
    {
        public float Input;
        public float Threshold;
        public float Emit;
        public float Evaluate()
        {
            float sigmoidInput = 1.0f / (1.0f + Mathf.Exp(-Input));
            if(sigmoidInput >= Threshold)
            {
                return Emit;
            }

            return 0;
        }

        public static Neuron Blank(float threshold, float emit)
        {
            return new Neuron{
                Input = 0.0f,
                Threshold = threshold,
                Emit = emit
            };
        }
    }

    public class Layer
    {
        public List<Neuron> Neurons = new List<Neuron>();
    }

    public struct Connection
    {
        public int SrcLayer;
        public int SrcNeuron;
        public int DstLayer;
        public int DstNeuron;
    }

    public class NeuralNetwork
    {
        public List<Layer> Layers = new List<Layer>();
        public List<Connection> Connections = new List<Connection>();

        public static NeuralNetwork SimpleNetworkRandomConnectivity()
        {
            NeuralNetwork newNet = new NeuralNetwork();

            Layer Input = new Layer();
            Input.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Input.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Input.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Input.Neurons.Add(Neuron.Blank(0.5f, 1.0f));

            Layer Middle1 = new Layer();
            Middle1.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Middle1.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Middle1.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Middle1.Neurons.Add(Neuron.Blank(0.5f, 1.0f));

            Layer Middle2 = new Layer();
            Middle2.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Middle2.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Middle2.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Middle2.Neurons.Add(Neuron.Blank(0.5f, 1.0f));

            Layer Output = new Layer();
            Output.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Output.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Output.Neurons.Add(Neuron.Blank(0.5f, 1.0f));
            Output.Neurons.Add(Neuron.Blank(0.5f, 1.0f));

            newNet.Layers.Add(Input);
            newNet.Layers.Add(Middle1);
            newNet.Layers.Add(Middle2);
            newNet.Layers.Add(Output);

            for(int layerIdx = 0; layerIdx < 3; layerIdx++)
            {
                for(int neuronIdx = 0; neuronIdx < 4; neuronIdx++)
                {
                    newNet.Connections.Add(new Connection{
                        SrcLayer = layerIdx,
                        SrcNeuron = neuronIdx,
                        DstLayer = layerIdx + 1,
                        DstNeuron = Random.Range(0,4)
                    });
                }
            }

            return newNet;
        }
    }
}