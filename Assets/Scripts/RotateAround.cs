using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateAround : MonoBehaviour
{
    [Header("R�glages de la rotation")]
    [SerializeField] Transform mTarget;

    [Space]
    //[Range(0, 100)] // Ajoute un slider entre 0 et 100
    [SerializeField] float mSpeed = 0f;

    [Header("Param�tres suppl�mentaires")]
    [Tooltip("Cela d�finit si la rotation est active ou non.")]
    [SerializeField] bool isRotating = false; // �tat de la rotation

    // Update is called once per frame
    void Update()
    {
        if (isRotating && mSpeed > 0f)
        {
            transform.RotateAround(mTarget.position, Vector3.up, mSpeed * Time.deltaTime);
        }
    }

    // M�thode pour activer la rotation avec une vitesse donn�e
    public void StartRotation(float speed)
    {
        mSpeed = speed;
        isRotating = true;
        ActiveCamera(true);
    }

    // M�thode pour arr�ter la rotation
    public void StopRotation()
    {
        isRotating = false;
        mSpeed = 0f;
        ActiveCamera(false);
    }

    public void ActiveCamera(bool value)
    {
        this.gameObject.SetActive(value);
    }
}