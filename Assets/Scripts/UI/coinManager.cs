using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class coinManager : MonoBehaviour
{
    //[Header ("UI references")]
    [SerializeField] GameObject animatedCoin;
    [SerializeField] Transform target;
    [SerializeField] Transform startPosition;
    [SerializeField] Transform HolderCoin;

    [Space]
    [Header("Available coin : (coins to pool")]
    [SerializeField] int maxCoins;
    Queue<GameObject> coinsQueue = new Queue<GameObject>();

    [Space]
    [Header("Animation settings")]
    [SerializeField] [Range(0.5f, 0.9f)] float minAniDuration;
    [SerializeField] [Range(0.9f, 2f)] float maxAniDuration;
    [SerializeField] Ease easeType;
    [SerializeField] float spread;

    public int c;
    Vector3 targetPosition;

    public int Coins
    {
        get { return c; }
        set { c = value; }
    }

    private void Awake()
    {
        targetPosition = target.position;
        prepareCoin();
    }
    private void Start()
    {
        AddCoins(maxCoins);
    }
    //private void OnEnable()
    //{
    //    AddCoins(maxCoins);
    //}
    void prepareCoin()
    {
        GameObject coin;
        for (int i = 0; i < maxCoins; i++)
        {
            coin = Instantiate(animatedCoin);
            coin.transform.parent = HolderCoin;
            coin.SetActive(false);
            coinsQueue.Enqueue(coin);
        }
    }

    public void AddCoins(/*Vector3 collectedCoinPosition,*/ int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            if (coinsQueue.Count > 0)
            {
                GameObject coin = coinsQueue.Dequeue();
                coin.SetActive(true);
                coin.transform.position = startPosition.position + new Vector3(Random.Range(-spread, spread), 0f, 0f);
                float duration = Random.Range(minAniDuration, maxAniDuration);
                coin.transform.DOMove(target.position, duration)
                    .SetEase(easeType)
                    .OnComplete(() => {
                        coin.SetActive(false);
                        coinsQueue.Enqueue(coin);
                        Coins++;
                    });
            }
        }
    }
}
