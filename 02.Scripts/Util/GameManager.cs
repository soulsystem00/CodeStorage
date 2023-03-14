using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public static GameManager Instance
    {
        get
        {
            if (inst == null)
            {
                inst = GameObject.FindObjectOfType<GameManager>();
            }

            if (inst == null)
            {
                GameObject unityObject = new GameObject();
                var newSingleton = unityObject.AddComponent<GameManager>();
                inst = newSingleton;

                unityObject.name = inst.GetType().Name;

                DontDestroyOnLoad(inst.gameObject);
            }

            return inst;
        }
    }

    //  - 점수 저장
    //  - 게임오버 상태 표현
    //  - 게임오버 되었을때 게임오버 UI 활성화
    //  - 플레이어의 사망을 감지해 게임오버 처리 실행
    //  - 점수에 따라 점수 UI 텍스트 갱신
}
