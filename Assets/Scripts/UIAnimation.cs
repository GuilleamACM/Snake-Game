using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIAnimation : MonoBehaviour
{
    public Text startText;
    public Text restartText;
    Sequence startSequence;
    Sequence restartSequence;
    void Awake()
    {
        startSequence = DOTween.Sequence();
        startSequence.Append(startText.DOFade(0f, 1f));
        startSequence.Append(startText.DOFade(1f, 1f));
        startSequence.SetLoops(-1);
        restartSequence = DOTween.Sequence();
        restartSequence.Append(restartText.DOFade(0f, 1f));
        restartSequence.Append(restartText.DOFade(1f, 1f));
        restartSequence.SetLoops(-1);
    }

    public void PlayStartSequence()
    {
        startSequence.Play();
    }

    public void PlayRestartSequence()
    {
        restartSequence.Play();
    }
}
