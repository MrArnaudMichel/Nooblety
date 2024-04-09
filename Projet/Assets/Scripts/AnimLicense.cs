using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnimLicense : MonoBehaviour
{
    // Start is called before the first frame update
    // create TextMeshPro text
    [SerializeField]
    private TextMeshProUGUI text;

    private double m_Time;
    private double total_time;
    void Start()
    {
        m_Time = 0;
        total_time = 0;
        text.color = new Color(255, 255, 255, 0);
    }

    // Update is called once per frame
    void Update()
    {
        total_time += Time.deltaTime;
        // wait 2 second and increase the alpha value
        if (total_time > 1 && total_time < 5)
        {
            if ((m_Time += Time.deltaTime) < 1 && text.color.a < 255)
            {
                text.color = new Color(255, 255, 255, text.color.a + 0.01f);
                m_Time = 0;
            }
        }else if (total_time > 5)
        {
            if ((m_Time += Time.deltaTime) < 1 && text.color.a > 0)
            {
                text.color = new Color(255, 255, 255, text.color.a - 0.01f);
                m_Time = 0;
            }
        }
        
    }
}
