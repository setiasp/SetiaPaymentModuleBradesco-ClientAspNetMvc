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
    
    public partial class BoletoItens
    {
        public int id { get; set; }
        public int id_boleto { get; set; }
        public string descritivo { get; set; }
        public string quantidade { get; set; }
        public string valor { get; set; }
        public Nullable<bool> adicional { get; set; }
    
        public virtual Boleto Boleto { get; set; }
    }
}
