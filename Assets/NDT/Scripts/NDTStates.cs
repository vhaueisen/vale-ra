using System;
using UnityEngine.Events;

public class NDTState
{
    public enum ProbeState
    {
        Default,
        Clean,
        PenetratingLiquid,
        Reveal
    }
    public bool Skipable;
    public ProbeState State;
    public string Hint;
    public NDTAction Action;
    private string alert;
    public string Alert(string from) => string.Format(alert, from);

    public NDTState(bool skipable, string hint, NDTAction action, ProbeState state, string alert)
    {
        Skipable = skipable;
        Hint = hint;
        Action = action;
        State = state;
        this.alert = alert;
    }
}

public enum NDTAction
{
    Skip,
    Examine,
    BrushShort,
    BrushLong,
    SandShort,
    SandLong,
    Solvent,
    MeshClean,
    DryClean,
    PenetratingLiquid,
    Reveal,
    Finish

}
public class NDTStates : Singleton<NDTStates>
{
    private static NDTState[] States;
    public static ITraining CurrentTraining
    {
        set
        {
            States = (NDTState[])value.Payload;
            currentTraining = value;
        }
        get => currentTraining;
    }
    private static ITraining currentTraining;
    public static UnityEvent<NDTAction> ChangeState = new UnityEvent<NDTAction>();
    public static NDTState[] VisualTest = {
        new NDTState(
            true,
            "Olá aluno!\nBem vindo ao curso guiado de ensaios mecânicos por teste visual.\nEu sou seu professor, e estarei te orientando pelo restante do treinamento.",
            NDTAction.Examine,
            NDTState.ProbeState.Default,
            ""
        ),
        new NDTState(
            false,
            "Vamos iniciar o procedimento com a análise prévia da peça.\nPor favor, aponte seu dispositivo para a peça e pressione inspecionar.",
            NDTAction.Examine,
            NDTState.ProbeState.Default,
            "Ah não! Me parece que você pegou '{0}'. Experimente inspecionar a peça de ensaio."
        ),
        new NDTState(
            true,
            "Muito bem! Não se esqueça de anotar cuidadosamente todas as dimensões e descontinuidades observadas.",
            NDTAction.BrushLong,
            NDTState.ProbeState.Default,
            ""
        ),
        new NDTState(
            false,
            "Vamos prosseguir com a limpeza mecânica. Por favor, pegue a escova de cerdas longas e escove a peça.",
            NDTAction.BrushLong,
            NDTState.ProbeState.Default,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, pegue a escova de cerdas longas."
        ),
        new NDTState(
            false,
            "Muito bem! Vamos remover alguns destes riscos com a escova de cerdas curtas!",
            NDTAction.BrushShort,
            NDTState.ProbeState.Default,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, pegue a escova de cerdas curtas."
        ),
            new NDTState(
            false,
            "Maravilha!\nAgora, você deve lixar a peça, começando pela lixa mais grossa.",
            NDTAction.SandLong,
            NDTState.ProbeState.Default,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, pegue a lixa mais grossa."
        ),
        new NDTState(
            false,
            "Estamos quase finalizando a limpeza mecânica...\nVamos remover estes riscos com a lixa mais fina.",
            NDTAction.SandShort,
            NDTState.ProbeState.Clean,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, pegue a lixa mais fina."
        ),
        new NDTState(
            false,
            "Agora, devemos iniciar a limpeza química do material.\nPor favor, despeje um pouco de solvente no trapo.",
            NDTAction.Solvent,
            NDTState.ProbeState.Clean,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, despeje um pouco de solvente no trapo."
        ),
        new NDTState(
            false,
            "Perfeito, lembre-se de nunca despejar o solvente diretamente na peça! \nAgora, limpe a peça utilizando o trapo.",
            NDTAction.MeshClean,
            NDTState.ProbeState.Clean,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, limpe a peça utilizando o trapo."
        ),
        new NDTState(
            false,
            "Maravilha! Veja como já está brilhante! \nVamos finalizar o ensaio visual inspecionando a peça devidamente limpa.",
            NDTAction.Examine,
            NDTState.ProbeState.Clean,
            "Que está fazendo? Já terminamos o procedimento. Por favor, faça a análise final da peça."
        ),
        new NDTState(
            true,
            "Muito bem! Não se esqueça de anotar cuidadosamente todas as dimensões e descontinuidades observadas.\nVocê finalizou o ensaio mecânico por teste visual.",
            NDTAction.Finish,
            NDTState.ProbeState.Clean,
            ""
        )
    };
    public static NDTState[] PenetratingLiquidTest = {
        new NDTState(
            true,
            "Olá aluno!\nBem vindo ao curso guiado de ensaios mecânicos não destrutivos por líquido penetrante.\nEu sou seu professor, e estarei te orientando pelo restante do treinamento.",
            NDTAction.Examine,
            NDTState.ProbeState.Clean,
            ""
        ),
        new NDTState(
            false,
            "Vamos iniciar o procedimento com a análise prévia da peça.\nPor favor, aponte seu dispositivo para a peça e pressione inspecionar.",
            NDTAction.Examine,
            NDTState.ProbeState.Clean,
            "Ah não! Me parece que você pegou '{0}'. Experimente inspecionar a peça de ensaio."
        ),
        new NDTState(
            false,
            "Maravilha! Agora, por favor, borrife líquido penetrante na peça.",
            NDTAction.PenetratingLiquid,
            NDTState.ProbeState.Clean,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, borrife liquido penetrante na peça."
        ),
        new NDTState(
            false,
            "Bacana! Vamos remover um pouco o excesso. Raspe um pouco do excesso de líquido com o papel toalha seco.",
            NDTAction.DryClean,
            NDTState.ProbeState.PenetratingLiquid,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, borrife liquido penetrante na peça."
        ),
        new NDTState(
            false,
            "Perfeito, retire agora, cuidadosamente, mais um pouco do líquido com o trapo...\nNão use muita força para não remover líquido demais!",
            NDTAction.MeshClean,
            NDTState.ProbeState.PenetratingLiquid,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, retire cuidadosamente mais um pouco do líquido com o trapo."
        ),
        new NDTState(
            false,
            "Pronto! Para visualizar melhor as descontinuidades, borrife um pouco de revelador na peça",
            NDTAction.Reveal,
            NDTState.ProbeState.PenetratingLiquid,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, borrife um pouco de revelador na peça."
        ),
        new NDTState(
            false,
            "Maravilha! Veja como as descontinuidades já estão aparentes...\nVamos finalizar o ensaio por líquido penetrante inspecionando a peça.",
            NDTAction.Examine,
            NDTState.ProbeState.Reveal,
            "Que está fazendo? Já terminamos o procedimento. Por favor, faça a análise final da peça."
        ),
        new NDTState(
            true,
            "Muito bem! Não se esqueça de anotar cuidadosamente todas as dimensões e descontinuidades observadas.\nVamos partir para a limpeza final da peça... não devemos guardá-la suja de penetrante e revelador!",
            NDTAction.DryClean,
            NDTState.ProbeState.Reveal,
            ""
        ),
        new NDTState(
            false,
            "Por favor, remova um pouco do excesso de material raspando a peça com o papel toalha seco.",
            NDTAction.DryClean,
            NDTState.ProbeState.Reveal,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, limpe a peça com o papel toalha seco."
        ),
        new NDTState(
            false,
            "Vamos prosseguir com a limpeza mecânica. Por favor, pegue a escova de cerdas longas e escove a peça.",
            NDTAction.BrushLong,
            NDTState.ProbeState.Default,
            "Hum... Isso não me parece a ferramenta correta, você pegou '{0}'! Por favor, pegue a escova de cerdas longas."
        ),
        new NDTState(
            false,
            "Muito bem! Você finalizou seu treinamento guiado de ensaios mecânicos por líquido penetrante!",
            NDTAction.Finish,
            NDTState.ProbeState.Clean,
            ""
        ),
    };

    public NDTState Current
    {
        get => States[currentIdx];
    }

    public NDTState Previous = null;
    private int currentIdx = 0;

    void Start()
    {
        ChangeState.AddListener(OnStateChange);
    }


    private void OnStateChange(NDTAction to)
    {
        Previous = Current;
        if (to == NDTAction.Skip && Current.Skipable)
        {
            currentIdx++;
            return;
        }
        if (to == Current.Action)
        {
            currentIdx++;
            return;
        }
        if (Current.Skipable)
        {
            while (Current.Skipable)
            {
                currentIdx = currentIdx + 1;
            }
            NDTStates.ChangeState.Invoke(to);
        }
    }
}
