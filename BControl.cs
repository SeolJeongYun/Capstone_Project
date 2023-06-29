using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BControl : MonoBehaviour
{
    public GameObject plane1;
    public GameObject plane2;
    public GameObject plane3;
//버튼을 누르면 정해진 plane을 제외한 다른 오브젝트들이 꺼짐
    public void Button1()
    {
        plane1.gameObject.SetActive(true);
        plane2.gameObject.SetActive(false);
        plane3.gameObject.SetActive(false);
    }
    public void Button2()
    {
        plane1.gameObject.SetActive(false);
        plane2.gameObject.SetActive(true);
        plane3.gameObject.SetActive(false);
    }
    public void Button3()
    {
        plane1.gameObject.SetActive(false);
        plane2.gameObject.SetActive(false);
        plane3.gameObject.SetActive(true);
    }
}
