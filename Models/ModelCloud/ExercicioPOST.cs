namespace MigraPatrim.Models.ModelCloud;

public class ExercicioPOST
{
    public ConfiguracaoOrganogramaExercicioPOST configuracaoOrganograma { get; set; }
    public int exercicio { get; set; }
}

public class ConfiguracaoOrganogramaExercicioPOST
{
    public int id { get; set; }
}