using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Models
{
    public static class StatusConstants
    {
        public const string Parado = "Parado";
        public const string Aquecendo = "Aquecendo";
        public const string Pausado = "Pausado";
        public const string Concluido = "Concluído";
        public const string Cancelado = "Cancelado";
        public const string Retomado = "Retomado";
        public const string Mais30s = "+30s";
        
        public static string GetDescricao(StatusAquecimento status) => status switch
        {
            StatusAquecimento.Parado => Parado,
            StatusAquecimento.Aquecendo => Aquecendo,
            StatusAquecimento.Pausado => Pausado,
            StatusAquecimento.Concluido => Concluido,
            StatusAquecimento.Cancelado => Cancelado,
            _ => Parado
        };
    }
}
