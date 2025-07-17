using System.Collections.Generic;
using UnityEngine;
public class GameManager : SingletonMono<GameManager>
{
    public List<Fruit> fruits;
    
    public Fruit emptyFruit;
    public Fruit GetRandomFruit()
    {
        return fruits[Random.Range(0, fruits.Count)];
    }
    public Fruit GetEmptyTile()
    {
        return emptyFruit;
    }
}
