using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour, IDamage
{

    [SerializeField] Renderer model;

    [SerializeField] int HP;

    Color colorOrig;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        StartCoroutine(flashColor());

        if (HP <= 0) 
        {
            Destroy(gameObject);
        }
    }
    
    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
