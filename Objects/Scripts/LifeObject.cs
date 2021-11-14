using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeObject : MonoBehaviour
{
    public LayerMask layerMask;
    private BasicSpriteMovement objectMovement;
    private ObjectM objectM;

    public enum TypeOfLifeObject {
        OneUpMushroom,
        TwoUpMushroom,
        ThreeUpMoon
    };
    public TypeOfLifeObject typeOfLifeObject;
    public bool startGoingLeft = true;

    private void Start()
    {
        if (typeOfLifeObject != TypeOfLifeObject.ThreeUpMoon) { 
            objectMovement = gameObject.AddComponent<BasicSpriteMovement>();

            objectMovement.layerMask = layerMask;
            objectMovement.leftDirection = startGoingLeft;
            objectMovement.speed *= 2;
        }

        objectM = ObjectM.GetObject(gameObject);
    }

    private void Update()
    {
        if (objectM.playerCollided) {
            PlayerCollided();
        }
    }

    public void PlayerCollided()
    {
        switch (typeOfLifeObject) {
            case TypeOfLifeObject.OneUpMushroom:
                MainSettings.ChangeLifeCounter(1);
                break;

            case TypeOfLifeObject.TwoUpMushroom:
                MainSettings.ChangeLifeCounter(2);
                break;

            case TypeOfLifeObject.ThreeUpMoon:
                MainSettings.ChangeLifeCounter(3);
                break;
        }

        gameObject.SetActive(false);
    }
}
