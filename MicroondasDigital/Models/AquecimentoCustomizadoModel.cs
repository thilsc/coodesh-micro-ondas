using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Models;

public class AquecimentoCustomizadoModel : BaseViewModel
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Alimento { get; set; } = string.Empty;
    public int Tempo { get; set; } = 1;
    public int Potencia { get; set; } = Constants.PotenciaPadrao;
    public char CaractereProgresso { get; set; } = TipoAquecimentoConstants.GetProgressChar(TipoAquecimento.Padrao);
    public string Instrucoes { get; set; } = string.Empty;

    private IEnumerable<ValidationResult> GetValidations()
    {
        return
            new List<ValidationResult>
            {
                new ValidationResult
                {
                    IsValid = !string.IsNullOrWhiteSpace(Nome),
                    Message = "O nome é obrigatório."
                },
                new ValidationResult
                {
                    IsValid = !string.IsNullOrWhiteSpace(Alimento),
                    Message = "O alimento é obrigatório."
                },
                new ValidationResult
                {
                    IsValid = Tempo >= 1,
                    Message = "O tempo informado não pode ser menor que 1."
                },
                new ValidationResult
                {
                    IsValid = (Potencia >= 1) && (Potencia <= Constants.PotenciaMax),
                    Message = "A potência deve ficar entre 1 e 10."
                },
                new ValidationResult
                {
                    IsValid = CaractereProgresso != '\0' && !char.IsWhiteSpace(CaractereProgresso),
                    Message = "O caractere de progresso é obrigatório."
                }
            };
    }    
}