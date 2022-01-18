using System;
using UnityEngine.Events;

public class NDTState
{
    public bool Skipable;
    public string Hint;
    public NDTAction Action;
    private string alert;
    public string Alert(string from) => string.Format(alert, from);

    public NDTState(bool skipable, string hint, NDTAction action, string _alert)
    {
        Skipable = skipable;
        Hint = hint;
        Action = action;
        alert = _alert;
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
    public static UnityEvent<NDTAction> ChangeState = new UnityEvent<NDTAction>();
    private readonly NDTState[] States = {
        new NDTState(
            true,
            "Olá aluno!\nBem vindo ao curso guiado de ensaios mecânicos não destrutivos.\n\nEu sou seu professor, e estarei te orientando pelo restante do treinamento.",
            NDTAction.Examine,
            ""
        ),
        new NDTState(
            false,
            "Vamos iniciar o procedimento com a análise prévia da peça.\nPor favor, aponte seu dispositivo para a peça e pressione inspecionar.",
            NDTAction.Examine,
            "Ah não! Me parece que você pegou a {0}. Experimente inspecionar a peça de ensaio."
        ),
        new NDTState(
            true,
            "Muito bem! Não se esqueça de anotar cuidadosamente todas as dimensões e descontinuidades observadas.",
            NDTAction.BrushLong,
            ""
        ),
        new NDTState(
            false,
            "Vamos prosseguir com a limpeza mecânica. Por favor, pegue a escova de cerdas longas e escove a peça.",
            NDTAction.BrushLong,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, pegue a escova de cerdas longas."
        ),
        new NDTState(
            false,
            "Muito bem! Vamos remover alguns destes riscos com a escova de cerdas curtas!",
            NDTAction.BrushShort,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, pegue a escova de cerdas curtas."
        ),
            new NDTState(
            false,
            "Maravilha!\nAgora, você deve lixar a peça, começando pela lixa mais grossa.",
            NDTAction.SandLong,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, pegue a lixa mais grossa."
        ),
        new NDTState(
            false,
            "Estamos quase finalizando a limpeza mecânica...\nVamos remover estes riscos com a lixa mais fina.",
            NDTAction.SandShort,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, pegue a lixa mais fina."
        ),
        new NDTState(
            false,
            "Agora, devemos iniciar a limpeza química do material.\nPor favor, despeje um pouco de solvente no trapo.",
            NDTAction.Solvent,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, despeje um pouco de solvente no trapo."
        ),
        new NDTState(
            false,
            "Perfeito, lembre-se de nunca despejar o solvente diretamente na peça! \nAgora, limpe a peça utilizando o trapo.",
            NDTAction.MeshClean,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, limpe a peça utilizando o trapo."
        ),
        new NDTState(
            false,
            "Maravilha! Veja como já está brilhante! \nVamos finalizar o ensaio visual inspecionando a peça devidamente limpa.",
            NDTAction.Examine,
            "Que está fazendo? Já terminamos o procedimento. Por favor, faça a análise final da peça."
        ),
        new NDTState(
            true,
            "Muito bem! Não se esqueça de anotar cuidadosamente todas as dimensões e descontinuidades observadas.\nVamos partir para o procedimento do ensaio por líquido penetrante.",
            NDTAction.PenetratingLiquid,
            ""
        ),
        new NDTState(
            false,
            "Devemos iniciar o procedimento borrifando líquido penetrante na peça.",
            NDTAction.PenetratingLiquid,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, borrife liquido penetrante na peça."
        ),
        new NDTState(
            false,
            "Bacana! Vamos remover um pouco o excesso. Raspe um pouco do excesso de líquido com o papel toalha seco.",
            NDTAction.DryClean,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, borrife liquido penetrante na peça."
        ),
        new NDTState(
            false,
            "Perfeito, retire agora, cuidadosamente, mais um pouco do líquido com o trapo...\nNão use muita força para não remover líquido demais!",
            NDTAction.MeshClean,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, retire cuidadosamente mais um pouco do líquido com o trapo."
        ),
        new NDTState(
            false,
            "Pronto! Para visualizar melhor as descontinuidades, borrife um pouco de revelador na peça",
            NDTAction.Reveal,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, borrife um pouco de revelador na peça."
        ),
        new NDTState(
            false,
            "Maravilha! Veja como as descontinuidades já estão aparentes...\nVamos finalizar o ensaio por líquido penetrante inspecionando a peça.",
            NDTAction.Examine,
            "Que está fazendo? Já terminamos o procedimento. Por favor, faça a análise final da peça."
        ),
        new NDTState(
            true,
            "Muito bem! Não se esqueça de anotar cuidadosamente todas as dimensões e descontinuidades observadas.\nVamos partir para a limpeza final da peça... não devemos guardá-la suja de penetrante e revelador!",
            NDTAction.DryClean,
            ""
        ),
        new NDTState(
            false,
            "Por favor, remova um pouco do excesso de material raspando a peça com o papel toalha seco.",
            NDTAction.DryClean,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, limpe a peça com o papel toalha seco."
        ),
        new NDTState(
            false,
            "Vamos prosseguir com a limpeza mecânica. Por favor, pegue a escova de cerdas longas e escove a peça.",
            NDTAction.BrushLong,
            "Hum... Isso não me parece a ferramenta correta, você pegou a {0}! Por favor, pegue a escova de cerdas longas."
        ),
        new NDTState(
            false,
            "Muito bem! Você finalizou seu treinamento guiado!",
            NDTAction.Finish,
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
