using MicroondasDigital.Models.Enums;

namespace MicroondasDigital.Models
{
    public static class TipoAquecimentoConstants
    {
        #region Declarações privadas
        #region Modos de Aquecimento
        private const string desc_Padrao = "Padrão";
        private const string desc_Pipoca = "Pipoca";
        private const string desc_Leite  = "Leite";
        private const string desc_Carne  = "Carne";
        private const string desc_Frango = "Frango";
        private const string desc_Feijao = "Feijão";
        #endregion

        #region Instruções por receita
        private const string instr_Pipoca = 
            "Observar o barulho de estouros do milho, caso houver um intervalo " +
            "de mais de 10 segundos entre um estouro e outro, interrompa o aquecimento.";

        private const string instr_Leite  = 
            "Cuidado com aquecimento de líquidos, o choque térmico " +
            "aliado ao movimento do recipiente pode causar fervura imediata causando risco de queimaduras.";

        private const string instr_CarneFrango  = 
            "Interrompa o processo na metade e vire o conteúdo com a parte de "+
            "baixo para cima para o descongelamento uniforme.";

        private const string instr_Feijao = 
            "Deixe o recipiente destampado e em casos de plástico, "+
            "cuidado ao retirar o recipiente pois o mesmo pode perder resistência em altas temperaturas.";
        
        #endregion

        #region Tempo de Aquecimento por receita
        private const int tempo_Padrao = 0;
        private const int tempo_Pipoca = 180; //3 minutos
        private const int tempo_Leite  = 300; //5 minutos
        private const int tempo_Carne  = 840; //14 minutos
        private const int tempo_Frango = 480; //8 minutos
        private const int tempo_Feijao = 480; //8 minutos
        #endregion

        #region Potência por receita
        private const int pot_Pipoca = 7;
        private const int pot_Leite  = 5;
        private const int pot_Carne  = 4;
        private const int pot_Frango = 7;
        private const int pot_Feijao = 9;
        #endregion
        #endregion

        #region Caractere de Progresso por Receita
        private const char caractereProgresso_Padrao = '.';
        private const char caractereProgresso_Pipoca = '%';
        private const char caractereProgresso_Leite  = 'H';
        private const char caractereProgresso_Carne  = '$';
        private const char caractereProgresso_Frango = '@';
        private const char caractereProgresso_Feijao = 'º';
        #endregion

        public static string GetDescricao(TipoAquecimento status) => status switch
            {
                TipoAquecimento.Padrao => desc_Padrao,
                TipoAquecimento.Pipoca => desc_Pipoca,
                TipoAquecimento.Leite  => desc_Leite,
                TipoAquecimento.Carne  => desc_Carne,        
                TipoAquecimento.Frango => desc_Frango,
                TipoAquecimento.Feijao => desc_Feijao,
                _ => desc_Padrao
            };

        public static string GetInstrucoes(TipoAquecimento status) => status switch
            {
                TipoAquecimento.Padrao => "",
                TipoAquecimento.Pipoca => instr_Pipoca,
                TipoAquecimento.Leite  => instr_Leite,
                TipoAquecimento.Carne  => instr_CarneFrango,        
                TipoAquecimento.Frango => instr_CarneFrango,
                TipoAquecimento.Feijao => instr_Feijao,
                _ => ""
            };
 
        public static int GetTempoAquecimento(TipoAquecimento status) => status switch
            {
                TipoAquecimento.Padrao => 0,
                TipoAquecimento.Pipoca => tempo_Pipoca,
                TipoAquecimento.Leite  => tempo_Leite,
                TipoAquecimento.Carne  => tempo_Carne,        
                TipoAquecimento.Frango => tempo_Frango,
                TipoAquecimento.Feijao => tempo_Feijao,
                _ => 0
            }; 

        public static int GetPotencia(TipoAquecimento status) => status switch
            {
                TipoAquecimento.Padrao => Constants.PotenciaPadrao,
                TipoAquecimento.Pipoca => pot_Pipoca,
                TipoAquecimento.Leite  => pot_Leite,
                TipoAquecimento.Carne  => pot_Carne,        
                TipoAquecimento.Frango => pot_Frango,
                TipoAquecimento.Feijao => pot_Feijao,
                _ => Constants.PotenciaPadrao
            };  

        public static char GetProgressChar(TipoAquecimento status) => status switch
            {
                TipoAquecimento.Padrao => caractereProgresso_Padrao,
                TipoAquecimento.Pipoca => caractereProgresso_Pipoca,
                TipoAquecimento.Leite  => caractereProgresso_Leite,
                TipoAquecimento.Carne  => caractereProgresso_Carne,
                TipoAquecimento.Frango => caractereProgresso_Frango,
                TipoAquecimento.Feijao => caractereProgresso_Feijao,
                _ => caractereProgresso_Padrao
            };                      
    }
}
