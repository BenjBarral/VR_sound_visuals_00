using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class strobe : MonoBehaviour
{
    public float t1 = 46f;
    public float t2 = 53f;
    public float prob_effect = 0.2f;

    public float time_offset = 0f;

    private bool has_reset = false;

    private bool is_active=true;


    public AudioSource audio_source;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool getChangeEffect()
    {
        float sampled_float = Random.Range(0f, 1f);
        return (sampled_float < prob_effect);
    }

    void changeChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(is_active);
            /// All your stuff with child here...
        }
    }

    void changeChildrenRandomly()
    {
        //foreach (Transform child in transform)
        //{
        //    child.gameObject.SetActive(is_active);
        //    /// All your stuff with child here...
        //}
        foreach (Transform child in transform)
        {
            if (this.getChangeEffect())
            {
                bool is_child_active = child.gameObject.activeSelf;
                child.gameObject.SetActive(!is_child_active);
            }
            /// All your stuff with child here...
        }

    }
    void resetChildren()
    {
        //foreach (Transform child in transform)
        //{
        //    child.gameObject.SetActive(is_active);
        //    /// All your stuff with child here...
        //}
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
            /// All your stuff with child here...
        }

    }
    bool getChangeEffectTouch()
    {
        return (Input.GetKey("up"));
    }

    // Update is called once per frame
    void Update()
    {
        float current_time = Time.time - time_offset;
        if (current_time >t1 && current_time < t2)
        {
            //bool change_effect = this.getChangeEffect();
            //if (change_effect)
            //{
            //    is_active = !is_active;
            //    this.changeChildren();
            //}
            changeChildrenRandomly();
        }
        //if (current_time >=t2 && !is_active)
        //{
        //    is_active = !is_active;
        //    this.changeChildren();
        //}
        if (current_time >= t2 && !has_reset)
        {
            //Debug.Log("reset");
            resetChildren();
            has_reset = true;
        }


    }
}
