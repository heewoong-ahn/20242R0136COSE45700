using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateRoom : MonoBehaviour
{
    public Text codeDisplay; // 생성된 코드를 보여줄 UI Text

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
        int randomCode = Random.Range(100000, 1000000); // 100000부터 999999까지의 범위에서 랜덤 숫자 생성
        Debug.Log("생성된 숫자 코드: " + randomCode);

        if (codeDisplay != null)
        {
            codeDisplay.text = randomCode.ToString(); // 코드 UI Text에 표시
        }
    }
}
