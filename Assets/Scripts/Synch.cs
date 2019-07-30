using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Synch : MonoBehaviour
{

    public bool Position, Rotation, Scale;
    public Space Space;

    public Transform From;
	
	// Update is called once per frame
	void LateUpdate ()
	{
	    if (From == null)
	        return;
        switch (Space)
        {
            case Space.World:
                if (Position)
                    transform.position = From.position;
                if (Rotation)
                    transform.rotation = From.rotation;
                if (Scale)
                    Scale = false;
                break;
            case Space.Self:
                if (Position)
                    transform.localPosition = From.localPosition;
                if (Rotation)
                    transform.localRotation = From.localRotation;
                if (Scale)
                    transform.localScale = From.localScale;
                break;
        }
    }
}
