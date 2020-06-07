using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    public float HealthPoints
    {
        get
        {
            return healthPoints;
        }
        set
        {
            healthPoints = value;
            
            // What if we reach 0?
            if(healthPoints <= 0)
            {
                life--;
                if (life >= 1)
                {
                    healthPoints = 100f;
                }
                else
                {
                    Destroy(gameObject);   
                }
            }
        }
    }

    [SerializeField]
    private float healthPoints = 100f;
    [SerializeField]
    private float life = 3.0f;
}
