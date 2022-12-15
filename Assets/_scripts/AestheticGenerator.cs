using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AlCaTrAzzGames.Utilities;

public class AestheticGenerator : Singleton<AestheticGenerator>
{
    public GameObject aestheticPrefab;
    public Vector2 yRange;
    public float xDistanceInFront;

    public Vector2 sizeRange;
    public Vector2 rotationRange;

    public Vector2 creationRangeBounds;
    float currentCreationRange;
    float travelledDistance;
    float lastLocation;

    // Instead of fixing this implementation we can alter it into a pooled system pretty easily.
    List<GameObject> aestheticSquares { get; } = new List<GameObject>();
    float aestheticBehindToRegenDistance = 15f;

    void Start(){
        currentCreationRange = Random.Range(creationRangeBounds.x, creationRangeBounds.y);
        travelledDistance = 0;
        lastLocation = 0;
    }


    void Update(){
        if(activePlayer == null){
            return;
        }

        float thisDistance = activePlayer.transform.position.x - lastLocation;
        travelledDistance += thisDistance;

        if(travelledDistance > currentCreationRange){
            GenerateAesthetic();
            travelledDistance -= currentCreationRange;
        }

        lastLocation = activePlayer.transform.position.x;

        CleanAestheticSquares();
    }


    Player activePlayer => GameController.Instance.activePlayer;

    void GenerateAesthetic(){
        currentCreationRange = Random.Range(creationRangeBounds.x, creationRangeBounds.y);

        GameObject newAesthetic = GetPooledAesthetic();
        newAesthetic.transform.SetParent(transform);
        newAesthetic.transform.position = new Vector3(lastLocation + xDistanceInFront, Random.Range(yRange.x, yRange.y), 0f);

        float size = Random.Range(sizeRange.x, sizeRange.y);
        newAesthetic.transform.localScale = new Vector3(size, size, size);

        float rot = Random.Range(rotationRange.x, rotationRange.y);
        newAesthetic.transform.localRotation = Quaternion.Euler(0f, 0f, rot);
    }

    GameObject GetPooledAesthetic(){

        // Some prefer foreach, i use for unless otherwise neccecary not to.
        for(int i = 0; i < aestheticSquares.Count; i++)
        {
            if(!aestheticSquares[i].activeSelf)
            {
                aestheticSquares[i].SetActive(true);
                return aestheticSquares[i];
            }
        }

        var newAesthetic = GameObject.Instantiate(aestheticPrefab);
        aestheticSquares.Add(newAesthetic);

        return newAesthetic;
    }

    void CleanAestheticSquares()
    {
        for(int i = 0; i < aestheticSquares.Count; i++)
        {
            if(aestheticSquares[i].transform.position.x < (activePlayer.transform.position.x - aestheticBehindToRegenDistance))
            {
                aestheticSquares[i].SetActive(false);
            }
        }
    }

    public IEnumerator CleanAllAestheticSquaresAfterDelay(float delay = 0.35f)
    {
        yield return new WaitForSeconds(delay);

        CleanAllAestheticSquares();
    }

    void CleanAllAestheticSquares()
    {
        for (int i = 0; i < aestheticSquares.Count; i++)
        {
            aestheticSquares[i].SetActive(false);
        }
    }

    public void SetActivePlayer(Player p){
        lastLocation = activePlayer.transform.position.x;
    }


    // In reality this should all be changed this could be a lot better. But lets keep it simple for this.
    public void ResetStats() {
        currentCreationRange = Random.Range(creationRangeBounds.x, creationRangeBounds.y);
        travelledDistance = 0;
        lastLocation = 0;
        CleanAestheticSquares();
    }
}
