using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class AnimationPanelView : ApplicationElement
{
    public Dropdown animationList;
    private List<string> animationsName;
    private List<string> animationsId;
    private Animator m_assetAnimator;
    public static bool pause = true;
    private string currentPlaying;

    public void initializePlayer()
    {
        m_assetAnimator = FindObjectOfType<ProjectionModel>().CurrentInstance.GetComponent<Animator>();
        if (m_assetAnimator == null)
            return;

        animationList.AddOptions(m_assetAnimator.runtimeAnimatorController.animationClips.Select(o => o.name).ToList());
    }

    private void animationListChanged()
    {
        currentPlaying = animationsId[animationList.value];
        pause = false;
        timeline.value = 0.0f;
        Pause();
        updateFowardButtons();
    }
    public Image pauseButtonImage;
    public Button forwardButton;
    public Button backwardButton;

    public Sprite pausedSprite;
    public Sprite playSprite;
    private void updateFowardButtons()
    {
        if (animationList.value + 1 == animationsId.Count)
        {
            forwardButton.interactable = false;
        }
        else
        {
            forwardButton.interactable = true;
        }
        if (animationList.value == 0)
        {
            backwardButton.interactable = false;
        }
        else
        {
            backwardButton.interactable = true;
        }
    }
    public void togglePause()
    {
        if (m_assetAnimator == null)
            return;
        pause = !pause;
        if (pause)
        {
            m_assetAnimator.speed = 0.0f;
            pauseButtonImage.sprite = pausedSprite;
        }
        else
        {
            PlayAnimation();
        }
    }
    public void ToggleCamera(bool i)
    {
        if (i == false)
        {
            ExitPlayMode(false);
        }
    }

    private void Pause()
    {
        pause = true;
        ExitPlayMode(false);
        m_assetAnimator.Play(currentPlaying);
        m_assetAnimator.speed = 0.0f;
        pauseButtonImage.sprite = pausedSprite;
    }

    public Transform animationCameraTransform;
    public Transform targetCameraTransform;
    private bool hasExited = true;

    private void PlayAnimation()
    {
        m_assetAnimator.Play(currentPlaying);
        m_assetAnimator.speed = 1.0f;
        pauseButtonImage.sprite = playSprite;
    }
    public Slider timeline;


    private void Update()
    {
        // if (!pause)
        // {
        //     timeline.value = assetAnimation[currentPlaying].normalizedTime;
        // }
        // if (!assetAnimation.isPlaying)
        // {
        //     if (!hasExited)
        //     {
        //         ExitPlayMode();
        //     }
        // }
    }

    public void UpdateTime(float t)
    {
        // m_assetAnimator.time = t * m_assetAnimator.c
    }

    private void ExitPlayMode(bool smooth = true)
    {
        hasExited = true;
        StopAllCoroutines();
        if (smooth)
        {
            StartCoroutine(ExitAnimation());
        }
    }

    private IEnumerator ExitAnimation()
    {
        pause = true;
        yield return new WaitForSeconds(2.5f);
        // m_assetAnimator.Rewind(currentPlaying);
        // m_assetAnimator.Stop();
        yield break;
    }

    public void toggleStop()
    {
        timeline.value = 0.0f;
        if (m_assetAnimator == null)
            return;
        pause = true;
        ExitPlayMode(false);
        m_assetAnimator.speed = 0;
        pauseButtonImage.sprite = pausedSprite;
    }

    public void skip(int i)
    {
        if (m_assetAnimator == null)
            return;
        if (i > 0)
        {
            if (animationList.value + 1 > animationsId.Count)
            {
                animationList.value = 0;
            }
            else
            {
                animationList.value = animationList.value + 1;
            }
        }
        else
        {
            if (animationList.value - 1 < 0)
            {
                animationList.value = 0;
            }
            else
            {
                animationList.value = animationList.value - 1;
            }
        }
        animationListChanged();
    }
}