using System.Collections;
using System.Collections.Generic;
using GameCreator.Runtime.Dialogue;
using Unity.VisualScripting;
using UnityEngine;

public class CreateActor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Actor actor = ScriptableObject.CreateInstance<Actor>();
        actor.name = "Actor";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
