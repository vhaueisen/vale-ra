public class QuickStartView : ApplicationElement
{
    public QuickStartModel model;

    private float m_animationTime = 0.3f;

    public void EnterHeader()
    {
        model.header.LeanAlpha(1f, 2f * m_animationTime + 0.5f);
        model.header.LeanMoveY(0f, 2f * m_animationTime + 0.5f);
    }

    public void EnterBody()
    {
        model.panelList[0].LeanAlpha(1f, 2f * m_animationTime);
        model.panelList[0].LeanMoveY(0f, 2f * m_animationTime).setOnComplete(
            () => model.slideBtns[1].interactable = true
        );
        PlayBookAnim(true);
    }

    private void PlayBookAnim(bool state)
    {
        if (state)
            LeanTween.play(model.books, model.bookSpriteSheet).setFrameRate(30);
        else
            model.books.LeanCancel();
    }
    private void MoveIndicator()
    {
        model.indicatorIdx.LeanMoveX(model.currentPosition.x, m_animationTime);
        model.indicatorIdx.LeanScaleX(2.5f, m_animationTime / 2f).setOnComplete(
            () => model.indicatorIdx.LeanScaleX(1f, m_animationTime / 2f)
        );
    }

    private void CurrentGuideLabel()
    {
        if (model.currentIdx == 0 || model.currentIdx == 5)
            model.currentGuideText.text = "";
        else if (model.currentIdx < 6)
            model.currentGuideText.text = "Telas";
        else
            model.currentGuideText.text = "Ferramentas";
    }

    public void SwitchPanel(int i)
    {
        if (model.currentIdx == 2)
            if (!model.controller.RequestCamera())
                return;

        float width = model.container.rect.width;
        int previousIdx = model.currentIdx;
        model.currentIdx = i;
        model.container.LeanCancel();

        if (model.currentIdx == previousIdx)
        {
            if (model.currentIdx == model.maxIdx)
                model.controller.Done();
            return;
        }


        PlayBookAnim(model.currentIdx == 0);
        CurrentGuideLabel();
        model.panelList[previousIdx].LeanAlpha(0f, m_animationTime).setEase(LeanTweenType.easeOutExpo);
        model.panelList[previousIdx].LeanMoveX(
            -i * width, m_animationTime
        ).setOnComplete(
            () =>
            {
                model.panelList[previousIdx].gameObject.SetActive(false);
            }
        );

        model.panelList[model.currentIdx].LeanMoveX(i * width, 0f);
        model.panelList[model.currentIdx].gameObject.SetActive(true);
        model.panelList[model.currentIdx].LeanMoveX(0f, m_animationTime);
        model.panelList[model.currentIdx].LeanAlpha(1f, m_animationTime);
        MoveIndicator();
    }
}