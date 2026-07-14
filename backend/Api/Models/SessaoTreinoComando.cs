namespace Api.Models;

public class SessaoTreinoComando
{
    public Guid Id { get; set; }
    public Guid SessaoTreinoId { get; set; }
    public SessaoTreino? SessaoTreino { get; set; }
    public Guid ComandoTreinoId { get; set; }
    public ComandoTreino? ComandoTreino { get; set; }
    public NivelSucesso NivelSucesso { get; set; }
}
