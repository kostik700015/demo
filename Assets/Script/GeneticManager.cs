using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticManager : MonoBehaviour
{
    public GameObject CarPrefab;

    public Agent[] Populition;

    public int PopulitionSize = 40;

    //число лучших тачек которые войдут в следующее поколение
    public int NumberOfBestAgentFitness = 6; 

    //количество особей для кроссовера
    public int NumberOfAgentToCrossOver = 4; 

    //Размер и число слоев в нейросети 
    public int[] NeuralNetworkShape;


    //процентное значение для веса мутации
    public double MutationRate = 0.05;
    //процентное значение для веса кросовера
    public double CrossOverWeightRate = 0.3;
         

    //номер текущей эпохи
    public  int GenerationNumber = 1;

	//Число весов во всех слоях нейронной сети
    private int WeightNum;
    //число смещений во всех слоях нейросети
    private int BiasesNum;


    private GameManager Game_Manager;


    private void Start()
    {
        Game_Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        //рассчитать количество весов в нейронной сети
        for (int i = 1; i < NeuralNetworkShape.Length; i++)
        {
            WeightNum += NeuralNetworkShape[i - 1] * NeuralNetworkShape[i];
        }

        //рассчитать количество смещений в нейронной сети
        BiasesNum = NeuralNetworkShape.Length - 1;
   
        //создаем популяцию 
        Populition = new Agent[PopulitionSize];

        for (int i = 0; i < PopulitionSize; i++)
        {
            //устанавливаем агент хромосомы 
            Populition[i] = new Agent();

            Populition[i].Fitness = 0;
            //устанавливаем веса новой хромосомы
            Populition[i].Weights = new double[WeightNum];
            //устанавливаем новые смещения агента
            Populition[i].Biases = new double[BiasesNum];

            //задаем случайные значения весов
            for (int j = 0; j < WeightNum; j++)
            {
                Populition[i].Weights[j] = Random.Range(-1.0f, 1.0f);
            }

            //рандомные значения смещений
            for (int j = 0; j < BiasesNum; j++)
            {
                Populition[i].Biases[j] = Random.Range(-1.0f, 1.0f);
            }

        }

        StartTheGenetationTest();

    }

    public void StartTheGenetationTest()
    {
        for (int i = 0; i < PopulitionSize; i++)
        {
            //устанавливаем новый Car Prefab
			Populition[i].Prefab = Instantiate(CarPrefab, Game_Manager.StartPosition.transform.position, new Quaternion(0, 0, 0, 0));
            //устанавливаем новую неросеть машинки
            Populition[i].Prefab.GetComponent<Car>().InstNeuralNetwork(NeuralNetworkShape, Populition[i].Weights,Populition[i].Biases);
            //назначаем индексы хромасомам по индексам массива Population
            Populition[i].Prefab.GetComponent<Car>().Id = i;

        }


        //запускаем тест
        Game_Manager.StartNewTest(PopulitionSize);

    }


    public void RePopulition()
    {


        Debug.Log("Generation : " + GenerationNumber + "|" + "Test End Best Fitness :" + Game_Manager.BestFitness);



        //сортировка популяции по значению фитнес функции
        for (int i = 0; i < PopulitionSize; i++)
        {
            for (int j = i; j < PopulitionSize; j++)
            {
                if (Populition[i].Fitness < Populition[j].Fitness)
                {
                    Agent Temp = Populition[i];
                    Populition[i] = Populition[j];
                    Populition[j] = Temp;
                }
            }
        }


        int IndexforNewPopulition = 0;

        Agent[] NewPopulition = new Agent[PopulitionSize];

        //берем лучшие 25% популяции и оставляем их 
        for (int i = 0; i < NumberOfBestAgentFitness; i++)
        {

            NewPopulition[IndexforNewPopulition] = Populition[i];

            NewPopulition[IndexforNewPopulition].Fitness = 0;

            IndexforNewPopulition++;

        }

      
        //Скрещевание: 
        for (int i = 0; i < NumberOfAgentToCrossOver; i+=2)
        {
            Agent Child1 = new Agent();
            Agent Child2 = new Agent();

            Child1.Weights = new double[WeightNum];
            Child2.Weights = new double[WeightNum];

            Child1.Biases = new double[BiasesNum];
            Child2.Biases = new double[BiasesNum];

            Child1.Fitness = 0;
            Child2.Fitness = 0;

            for (int j=0;j<WeightNum;j++)
            {
                if(Random.Range(0.0f,1.0f)<CrossOverWeightRate)
                {
                    Child1.Weights[j] = Populition[i].Weights[j];
                    Child2.Weights[j] = Populition[i + 1].Weights[j];
                }
                else
                {
                    Child1.Weights[j] = Populition[i + 1].Weights[j];
                    Child2.Weights[j] = Populition[i ].Weights[j];
                }
            }

            for (int j = 0; j < BiasesNum; j++)
            {
                if (Random.Range(0.0f, 1.0f) < CrossOverWeightRate)
                {
                    Child1.Biases[j] = Populition[i].Biases[j];
                    Child2.Biases[j] = Populition[i + 1].Biases[j];
                }
                else
                {
                    Child1.Biases[j] = Populition[i + 1].Biases[j];
                    Child2.Biases[j] = Populition[i].Biases[j];
                }
            }


            NewPopulition[IndexforNewPopulition] = Child1;

            IndexforNewPopulition++;

            NewPopulition[IndexforNewPopulition] = Child2;

            IndexforNewPopulition++;

        }


      


        //делаем мутацию     
        for (int i=0;i<IndexforNewPopulition;i++)
        {
            if (Random.Range(0.0f, 1.0f) < MutationRate)
            {
            

                for(int j=0;j<WeightNum;j++)
                {

                    if(Random.Range(0.0f, 1.0f) < MutationRate)
                    {
                        NewPopulition[i].Weights[j] = Random.Range(-1.0f, 1.0f);
                    }
                    
                }


                for (int j = 0; j < BiasesNum; j++)
                {
					// меняет значения нейронов смещения
                    if (Random.Range(0.0f, 1.0f) < MutationRate )
                    {
                        NewPopulition[i].Biases[j] = Random.Range(-1.0f, 1.0f);
                    }

                }


            

          


            }

        }


        //создаем новый агент
        while (IndexforNewPopulition<PopulitionSize)
        {
            NewPopulition[IndexforNewPopulition] = new Agent();

            NewPopulition[IndexforNewPopulition].Fitness = 0;
            //устанавливаем новую хромосому весов
            NewPopulition[IndexforNewPopulition].Weights = new double[WeightNum];
            //устанавливаем новые смещения
            NewPopulition[IndexforNewPopulition].Biases = new double[BiasesNum];
            //случайные значения весов
            for (int j = 0; j < WeightNum; j++)
            {
                NewPopulition[IndexforNewPopulition].Weights[j] = Random.Range(-1.0f, 1.0f);
            }
            
            //случайные значения для нейронов смещения
            for (int j = 0; j < BiasesNum; j++)
            {
                NewPopulition[IndexforNewPopulition].Biases[j] = Random.Range(-1.0f, 1.0f);
            }

            IndexforNewPopulition++;
        }
    


        //назначить новую популяцию
        Populition = NewPopulition;

        //начть тест с новым поколением
        StartTheGenetationTest();

        //повысить номер поколения
        GenerationNumber++;



    }
}