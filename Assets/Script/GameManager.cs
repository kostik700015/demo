using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //number of uncrashed cars in test. 
    public int CarStayAlive;

    //проверка чекпоинтов
    public Transform[] CheckPoints;

    //если окончен тест текущего поколения или все машины разбиты
    bool EndTheGenerationTest = true;

    //лучшее достигнутое значение фитнес функции
    public double BestFitness = 0;

    //точка в которой появляются тачки
    public Transform StartPosition;

    //переменные объектов сцены
    private GeneticManager Genetic_Manager;
    private CameraFollow MainCamera;
    private InformationDisplay display;

    //тут хрнится длина пройденного пути
    private double TrackLenght;

	public static float StartTimeScale = 1.0f;

    private void Start()
    {

        Genetic_Manager = GetComponent<GeneticManager>();
        MainCamera = Camera.main.GetComponent<CameraFollow>();
        display = GameObject.FindGameObjectWithTag("Display").GetComponent<InformationDisplay>();

        //рассчет пройденного пути
        TrackLenght = CalculateTrackLength();

		StartTimeScale = Time.timeScale;
    }


    double CalculateTrackLength()
    {

        
		// рассчет расстояний между стартовой позицией и каждым чекпоинтом вплоть до финишной черты

        double tracklenght = Vector3.Distance(CheckPoints[0].position, StartPosition.position);

        for (int i = 0; i < CheckPoints.Length - 1; i++)
        {
            tracklenght += Vector3.Distance(CheckPoints[i].position, CheckPoints[i + 1].position);
        }

        return tracklenght;

    }



    //начало теста нового поколения машинок
    public void StartNewTest(int NumOfPopulition)
    {
        
        CarStayAlive = NumOfPopulition;

        //делает переменную false перед началом теста
        EndTheGenerationTest = false;

        //сброс позиции камеры
        MainCamera.ResetCameraPosition();


    }

	void Update () 
    {

        if(EndTheGenerationTest)
        {
            return;
        }

        //проверяет, если все тачки разбиты -- останавливаем тест
        if (CarStayAlive<=0  && EndTheGenerationTest == false)
        {
            EndTheGenerationTest = true;
            Genetic_Manager.RePopulition();
        }
		
	}

    public double CalculateFitness(Vector3 CarPosition,int IndexforLastChekPoint)
    {
       
        if(IndexforLastChekPoint==CheckPoints.Length-1)
        {
            return 1;
        }

        //считаем значение фитнес функции по расстоянию до финиша


        double Fitness = Vector3.Distance(CheckPoints[IndexforLastChekPoint+1].position,CarPosition);

        for (int i=IndexforLastChekPoint+1;i<CheckPoints.Length-1;i++)
        {
            Fitness += Vector3.Distance(CheckPoints[i].position, CheckPoints[i + 1].position);
        }


        Fitness = (1 - (Fitness / TrackLenght));


        if (BestFitness<Fitness)
        {
            BestFitness = Fitness;
        }    

        return Fitness;

    }


    public void TestFinish()
    {
        display.ShowSuccessPanel();

        Time.timeScale = 0;
    }

}
