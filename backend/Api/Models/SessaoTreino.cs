namespace Api.Models;

public enum NivelSucesso
{
    NaoRespondeu,
    Tentou,
    ConseguiuComAjuda,
    ConseguiuSozinho
}

public class SessaoTreino
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public DateOnly Data { get; set; }
    public List<SessaoTreinoComando> SessaoTreinoComandos { get; set; } = [];
}
