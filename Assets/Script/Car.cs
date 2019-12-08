using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

	//Link for current agent in array of population 
    public int Id;

    public NeuralNetwork NN;
	//link for sensors position

    public Transform[] SensorsPosition;


	public double LastEngineValue = 0.0f;
	public double LastTurnValue = 0.0f;
 
    private bool CarCrashed= false;

    //when car at finish
    private int LastIndexCheckPoint = -1;

  
    private GameManager Game_Manager;
    private GeneticManager Genetic_Manager;
    private CameraFollow MainCamera;

    private void Start()
    {
        //назначить отсылки
        Game_Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Genetic_Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GeneticManager>();
        MainCamera = Camera.main.GetComponent<CameraFollow>();
    }

    public void InstNeuralNetwork(int[] Layers, double[] Weights,double[] Biases)
    {

        NN = GetComponent<NeuralNetwork>();

        NN.inst(Layers, Weights, Biases);

    }



    void Update ()
    {
 
        //checking status of car, if "Crushed" stop car's moving
        if (CarCrashed)
        {
            return;
        }
  


        //check initializing of neural network  
        if (NN==null)
        {
            return;
        }



        //compute entering layer from sensors data 
        double[] InputLayer = CalculateInputLayer();

        //start NN and take output layer. 
        double[] Output = NN.Run(InputLayer);


		//first value of output layer is speed == engine value 
          double EngineValue = NN.Sigmoid(Output[0]);

		//2nd value from output layer is angle of turn 
           double TurnValue = Output[1];

		LastEngineValue = EngineValue;
		LastTurnValue = TurnValue;

        //moving
        transform.Translate(new Vector3((float)EngineValue * 2f * Time.deltaTime, 0, 0));

        //turn
        transform.Rotate(new Vector3(0, (float)TurnValue * 90f * Time.deltaTime, 0));

  
    }

   




    // computing input layer

    double[] CalculateInputLayer()
    {
        //creation array of distances 
        double[] Distances = new double[SensorsPosition.Length];

        //computing distances from sensors 
        for (int i = 0; i < SensorsPosition.Length; i++)
        {
            //"Ray Cast" from sensors  
            Ray ray = new Ray(SensorsPosition[i].position, SensorsPosition[i].forward);

            RaycastHit hit;

            Physics.Raycast(SensorsPosition[i].position, SensorsPosition[i].forward, out hit,10f);

            if(hit.collider!=null)
            {
                //distance between sensor and collision (wall, border etc) 
                Distances[i] = Vector3.Distance(SensorsPosition[i].position, hit.point);

            }
            else
            {
                Distances[i] = 0;
            }         

        }

        return Distances;

    }

    //checking "Crashed" status 
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Wall"  && CarCrashed==false)
        {
            Crashed();
        }
    }
    //car at checkpoint or not 
    private void OnTriggerExit(Collider other)
    {
        if(other.tag=="CheckPoint")
        {   
            LastIndexCheckPoint++;

            //updating last checkpoint 
            MainCamera.UpdateTarget(LastIndexCheckPoint + 1);

        }

        if(other.tag=="FinishLine")
        {

            Game_Manager.TestFinish();

        }
    }


    void Crashed()
    {

        CarCrashed = true;
        //reducing quantity of "uncrushed" cars  
        Game_Manager.CarStayAlive--;
        //computing of fitness function 
        double fitness = Game_Manager.CalculateFitness(transform.position, LastIndexCheckPoint);
        //assign fitness function for crashed "car" 
        Genetic_Manager.Populition[Id].Fitness = fitness;

        Destroy(gameObject, 1f);

    }
}
