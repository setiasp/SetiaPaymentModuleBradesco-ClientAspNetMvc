//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SetiaPaymentSampleWebApp
{
    using System;
    using System.Collections.Generic;
    
    public partial class Boleto
    {
        public Boleto()
        {
            this.BoletoItens = new HashSet<BoletoItens>();
        }
    
        public int id { get; set; }
        public string order_id { get; set; }
        public string merchant_id { get; set; }
        public string cedente { get; set; }
        public string banco { get; set; }
        public string numero_agencia { get; set; }
        public string numero_conta { get; set; }
        public string url_logo_lojista { get; set; }
        public string mensagem_header_lojista { get; set; }
        public string assinatura { get; set; }
        public string data_emissao { get; set; }
        public string data_processamento { get; set; }
        public string data_vencimento { get; set; }
        public string nome_sacado { get; set; }
        public string cpf_sacado { get; set; }
        public string cip { get; set; }
        public string ano_nosso_numero { get; set; }
        public string cep_sacado { get; set; }
        public string endereco_sacado { get; set; }
        public string cidade_sacado { get; set; }
        public string uf_sacado { get; set; }
        public string numero_pedido { get; set; }
        public string valor_documento { get; set; }
        public string shopping_id { get; set; }
        public string numero_documento { get; set; }
        public string carteira { get; set; }
        public string instrucao_01 { get; set; }
        public string instrucao_02 { get; set; }
        public string instrucao_03 { get; set; }
        public string instrucao_04 { get; set; }
        public string instrucao_05 { get; set; }
    
        public virtual ICollection<BoletoItens> BoletoItens { get; set; }
    }
}
