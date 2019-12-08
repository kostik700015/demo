using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class InformationDisplay : MonoBehaviour
{

    public GameObject SuccessPanel;

    public Text GenerationNumberText;

    public Text BestFitnessText;

    public Text CarStayAliveText;

	public Text CarSpeed;

	public Text CarTurn;

    private GeneticManager Genetic_Manager;
    private GameManager Game_Manager;

	void Start ()
    {
        Genetic_Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GeneticManager>();
        Game_Manager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
	
	
	void Update ()
    {
        GenerationNumberText.text = Genetic_Manager.GenerationNumber.ToString();

        BestFitnessText.text = Game_Manager.BestFitness.ToString();

        CarStayAliveText.text = Game_Manager.CarStayAlive.ToString();

		Transform lastCheckpoint = Game_Manager.CheckPoints [Game_Manager.CheckPoints.Length - 1];

		Car nearestCar = null;

		for (int i = 0; i < Genetic_Manager.Populition.Length; i++) {
			GameObject car = Genetic_Manager.Populition [i].Prefab;
			if (car == null)
				continue;
			float sqrDistance = (car.transform.position - lastCheckpoint.position).sqrMagnitude;
			if ((nearestCar == null) || ((nearestCar.transform.position - lastCheckpoint.position).sqrMagnitude > sqrDistance))
				nearestCar = car.GetComponent<Car>();
		}

		if (nearestCar != null) {
			if (CarSpeed != null)
				CarSpeed.text = nearestCar.LastEngineValue.ToString("0.00000");
			if (CarTurn != null)
				CarTurn.text = nearestCar.LastTurnValue.ToString("0.00000");
		}

    }

    public void ShowSuccessPanel()
    {
        SuccessPanel.SetActive(true);

        SuccessPanel.GetComponent<SuccessPanel>().SetInformation(Genetic_Manager.GenerationNumber, Genetic_Manager.MutationRate, Genetic_Manager.CrossOverWeightRate);
    }



   




}
