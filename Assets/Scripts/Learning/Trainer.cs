using System.Collections;
using UnityEngine;

public interface ITraining
{
    string Name { get; }
    string Key { get; }
    string Description { get; }
    object Payload { get; }
    bool IsCompleted { get; }
    TrainingType Type { get; }
}
public class NDTTraining : ITraining
{
    private string name;
    private string key;
    private string description;
    private NDTState[] payload;
    private TrainingType type = TrainingType.NDT;
    public string Name { get => name; }
    public string Key { get => key; }
    public string Description { get => description; }
    public object Payload { get => (object)payload; }
    public TrainingType Type { get => type; }
    public bool IsCompleted
    {
        get => PlayerPrefs.GetInt(key, -1) > 0;
    }

    public NDTTraining(string name, string description, string key, NDTState[] states)
    {
        this.key = key;
        this.name = name;
        this.description = description;
        this.payload = states;
    }
}

public enum TrainingType
{
    NDT
}
public class Trainer : MonoBehaviour
{
    public static ITraining[] TrainingArray;
    void Awake()
    {
        TrainingArray = new NDTTraining[]{
            new NDTTraining("Ensaios Mecânicos - Teste Visual", "O teste visual é o mais utilizado na detecção de falhas macroscópicas. Utilizado para determinar aceitabilidade de componentes de processos produtivos que apresentem como requisito qualquer grau de qualidade, o teste visual pode ser utilizado para identificar: soldas ruins ou falhas em estruturas compostas; tubulações; articulações; fixadores ou componentes ausentes; acabamento de superfície inadequado; rachaduras; cavidades; amassados ou dimensões inadequadas. O Teste consiste em identificar descontinuidades na superfície da peça, com atenção à limpeza do material.", "NDT_VI", NDTStates.VisualTest),
            new NDTTraining("Ensaios Mecânicos - Teste por Líquido Penetrante", "O Teste por líquido penetrante é utilizado na detecção de descontinuidades, primordialmente superficiais (defeitos de superfície de fundição; forja; soldagem; rachaduras; porosidades superficiais etc.), abertas na superfície do material. É aplicado em metais (alumínio, cobre, aço, titânio etc.), vidros, materiais cerâmicos, borracha ou plásticos. Por meio da capilaridade, o fluido penetrante com baixa tensão superficial penetra descontinuidades no material. Após tempo de penetração adequado, o excesso de penetrante é removido por um revelador, que ajuda a tirar penetrante da falha e deixa indicações visíveis para o inspetor.", "NDT_LQP",NDTStates.PenetratingLiquidTest)
        };
    }

    public static void CompleteTraining(ITraining t)
    {
        PlayerPrefs.SetInt(t.Key, 1);
        NDTBehaviour.Instance.Back();
    }

    public static void InitializeTraining(ITraining t)
    {
        NDTStates.CurrentTraining = t;
        FooterView.OnSceneLoader(SceneLoaderModel.NDTScene, () => NDTStates.CurrentTraining = t);
    }
}
