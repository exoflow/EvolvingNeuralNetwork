//@author ExoFlow
//https://github.com/exoflow
//https://www.youtube.com/channel/UC019wlpKUC9Cae_IocyVRIA

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    //Prefabs
    public GameObject obstacleTop;
    public GameObject obstacleBottom;
    public GameObject birdPrefab;

    //Evolvement variables 
    public int populationSize;
    public int deadCount;
    public float mutationChance;
    public float maxFitness;
    public int generation;
    public List<Bird> lastPopulation;
    public List<Bird> birdList;

    ////Neural Network layers
    public int[] layers; 


    void Start()
    {
        layers = new int[] { 5, 10, 10, 2 };  //5 inputs, 2 outputs
        LoadPlayerPrefs(); 
        InstantiatePopulation();
        StartCoroutine(GenerateObstacle());
    }

    public void FixedUpdate()
    {
        birdList.Sort(SortByPosition);
        birdList.Sort(SortByFitness);
        maxFitness = Math.Max(birdList[0].fitness, maxFitness);
        DisplayStats();
    }



	#region generation management

	public void BirdCrashed()
	{
		deadCount++;

		if (deadCount == populationSize)
		{
			StartCoroutine(EndGeneration());
		}
	}

	IEnumerator EndGeneration()
	{
		yield return new WaitForSeconds(1f);
		StopAllCoroutines();
		CancelInvoke();
		DestroyObstacles();
		DestroyBirdBodies();
		StartGeneration();
	}

	void StartGeneration()
	{
		generation++;
		populationSize = PlayerPrefs.GetInt("populationSize");
		deadCount = 0;
		lastPopulation = birdList;
		birdList = new List<Bird>();
		InstantiatePopulation();
		StartCoroutine(GenerateObstacle());
	}

	void DestroyObstacles()
    {
        GameObject[] gameObjects;
        gameObjects = GameObject.FindGameObjectsWithTag("Obstacle");
        for (int i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }

    void DestroyBirdBodies()
    {
        if (birdList != null)
        {
            for (int i = 0; i < birdList.Count; i++)
            {
                Destroy(birdList[i].gameObject);
            }
        }
    }

    public void InstantiateBird()
    {
        Bird b = Instantiate(birdPrefab, new Vector2(0, 0), Quaternion.identity).GetComponent<Bird>();
        b.transform.SetParent(GameObject.Find("InstantiatedBirds").transform, false);
        birdList.Add(b);
    }

    public int SortByPosition(Bird a, Bird b)
    {
        return -(a.transform.position.x.CompareTo(b.transform.position.x));
    }

    public int SortByFitness(Bird a, Bird b)
    {
        return -(a.fitness.CompareTo(b.fitness));
    }

    public void InstantiatePopulation()
    {
        birdList = new List<Bird>();

        for (int i = 0; i < populationSize; i++)
        {
            InstantiateBird();

            if (generation == 0)
            {
                birdList[i].SetBrain(new NeuralNetwork(layers));
            }
            else
            {
                MutatelastPopulation(i);
            }
        }
    }

    public void MutatelastPopulation(int i)
    {
        if (i == 0)
            birdList[i].SetBrain(new NeuralNetwork(lastPopulation[0].brain, 0));
        if (i < populationSize * 0.25f)
            birdList[i].SetBrain(new NeuralNetwork(lastPopulation[i].brain, mutationChance));
        if (i < populationSize * 0.50f)
            birdList[i].SetBrain(new NeuralNetwork(lastPopulation[0].brain, mutationChance));
        if (i < populationSize * 0.75f)
            birdList[i].SetBrain(new NeuralNetwork(lastPopulation[0].brain, mutationChance * 0.5f));
        if (i < populationSize * 1.00f)
            birdList[i].SetBrain(new NeuralNetwork(lastPopulation[0].brain, mutationChance * 2));
    }

    #endregion

    #region PlayerPrefs

    void LoadPlayerPrefs()
	{
		populationSize = PlayerPrefs.GetInt("populationSize", 50);
		mutationChance = PlayerPrefs.GetFloat("mutationChance", 0.025f);

		GameObject.Find("PopulationSlider").GetComponent<Slider>().value = populationSize;
		GameObject.Find("MutationSlider").GetComponent<Slider>().value = mutationChance;
	}

	#endregion

	#region UI

	void DisplayStats()
    {
        GameObject.Find("Generation").GetComponent<Text>().text = "Generation: " + generation.ToString();
        GameObject.Find("Population").GetComponent<Text>().text = "Birds alive: " + (populationSize - deadCount).ToString();
        GameObject.Find("MaxFitness").GetComponent<Text>().text = "Max. fitness: " + Math.Round(maxFitness, 1).ToString();
        GameObject.Find("CurrentBestFitness").GetComponent<Text>().text = "Current best fitness: " + Math.Round(birdList[0].fitness, 1).ToString();
        birdList[0].GetComponentInChildren<Text>().enabled = (birdList[0].fitness >= maxFitness && generation != 0);
    }

    public void ToggleTimeScale()
    {
        Time.timeScale = Math.Abs(Time.timeScale - 1) < 0.01f ? 10 : 1;
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnPopulationSliderChanged(float value)
    {
        PlayerPrefs.SetInt("populationSize", (int)value);
        GameObject.Find("PopulationSlider").GetComponentInChildren<Text>().text = "Population size: " + (int)value;

    }

    void OnMutationSliderChanged(float value)
    {
        PlayerPrefs.SetFloat("mutationChance", value);
        GameObject.Find("MutationSlider").GetComponentInChildren<Text>().text = "Mutation chance: " + Math.Round(value * 100, 1) + "%";

    }

	#endregion

	#region obstacles

	public IEnumerator GenerateObstacle()
	{
		while (true)
		{
			InstantiateObstacle();
			yield return new WaitForSeconds(1.2f);
		}
	}

	public void InstantiateObstacle()
	{
		float obstacleFillHeight = Screen.height * 0.60f;
		float randomFloat = UnityEngine.Random.Range(0.15f, 0.85f);
		float topHeight = randomFloat * obstacleFillHeight;
		float bottomHeight = (1 - randomFloat) * obstacleFillHeight;

		GameObject newObstacleTop = Instantiate(obstacleTop, new Vector2(0, (-topHeight / 2)), Quaternion.identity);
		newObstacleTop.transform.SetParent(GameObject.Find("InstantiatedObstacles").transform, false);
		newObstacleTop.GetComponent<RectTransform>().sizeDelta = new Vector2(50, topHeight);
		newObstacleTop.GetComponent<BoxCollider2D>().size = newObstacleTop.GetComponent<RectTransform>().sizeDelta;
		newObstacleTop.GetComponent<Rigidbody2D>().AddForce(Vector3.left * 150000000, ForceMode2D.Force);

		GameObject newObstacleBottom = Instantiate(obstacleBottom, new Vector2(0, (bottomHeight / 2) + 25), Quaternion.identity);
		newObstacleBottom.transform.SetParent(GameObject.Find("InstantiatedObstacles").transform, false);
		newObstacleBottom.GetComponent<RectTransform>().sizeDelta = new Vector2(50, bottomHeight);
		newObstacleBottom.GetComponent<BoxCollider2D>().size = newObstacleBottom.GetComponent<RectTransform>().sizeDelta;
		newObstacleBottom.GetComponent<Rigidbody2D>().AddForce(Vector3.left * 150000000, ForceMode2D.Force);
	}

	#endregion
}
