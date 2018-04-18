using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagRed {

    bool VeliavaYra = true;

    
    // Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


    void OnCollisionEnter(Collision collision)
    {
        if (VeliavaYra)
        {
            var hit = collision.gameObject;
            var team = hit.GetComponent<Health>().teamText.ToString();
            if (team != null)
            {
                if(team == "Team Blue")
                {

                }
            }
        }
    }
}

public class FlagCapture : Flag
{


}
