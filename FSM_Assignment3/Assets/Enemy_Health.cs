using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    public float EnemyHealthPoints
    {
        get
        {
            return EnemyhealthPoints;
        }
        set
        {
            EnemyhealthPoints = value;
            
            if(EnemyhealthPoints <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    [SerializeField]
    private float EnemyhealthPoints = 100f;// Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
