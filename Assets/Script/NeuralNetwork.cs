using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    public int[] Layers;

    public double[] Weights;

    public double[] Biases;



    public void inst(int[] layers, double[] weights,double[] biases)
    {
  
        if(layers.Length<2)
        {       
            return;
        }

        //назначаем размер слоя
        this.Layers = layers;
        //назначаем веса
        this.Weights = weights;
        //назначаем смещения
        this.Biases = biases;

    }

  
    public double Sigmoid(double x)
    {
        return 1 / (1 + Math.Exp(-x));
    }


    public double[] Run(double[] Input)
    {
        //проверяем соответствие входного массива размерам входного слоя
        if(Input.Length!=Layers[0])
        {
            return null;
        }
  
        int WeightsIndex = 0;

        double[] PrevLayers = Input ;


        //считаем выходной слой
        for (int i=1;i<Layers.Length;i++)
        {
            double[] CurrentLayer=new double[Layers[i]];

            for(int j=0;j<Layers[i];j++)
            {
                double Sum = 0;

                for(int h=0;h<Layers[i-1];h++)
                {
                    Sum += PrevLayers[h] * Weights[WeightsIndex];

                    WeightsIndex++;
                }

                CurrentLayer[j] = Math.Tanh(Sum + Biases[i-1]);  
     
            }

            PrevLayers = CurrentLayer;
        }


        double[] Output = PrevLayers;

        return Output;
    }
	
}
