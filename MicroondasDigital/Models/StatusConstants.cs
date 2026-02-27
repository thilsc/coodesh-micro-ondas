using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Models
{
    public static class StatusConstants
    {
        public const string Parado = "Parado";
        public const string Aquecendo = "Aquecendo";
        public const string Pausado = "Pausado";
        
        public static string GetDescricao(StatusAquecimento status) => status switch
        {
            StatusAquecimento.Parado => Parado,
            StatusAquecimento.Aquecendo => Aquecendo,
            StatusAquecimento.Pausado => Pausado,
            _ => Parado
        };
    }
}
