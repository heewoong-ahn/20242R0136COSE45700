using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateRoom : MonoBehaviour
{
    public Text codeDisplay; // ������ �ڵ带 ������ UI Text

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateCode()
    {
        int randomCode = Random.Range(100000, 1000000); // 100000���� 999999������ �������� ���� ���� ����
        Debug.Log("������ ���� �ڵ�: " + randomCode);

        if (codeDisplay != null)
        {
            codeDisplay.text = randomCode.ToString(); // �ڵ� UI Text�� ǥ��
        }
    }
}
