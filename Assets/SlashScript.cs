using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashScript : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 5f);
    }
    public void Update()
    {
        gameObject.transform.position += gameObject.transform.forward / 5f;
    }
}
