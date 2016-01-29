using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using SetiaPaymentModuleBradesco;
using SetiaPaymentModuleBradesco.Models;
using System.Globalization;
using SetiaPaymentSampleWebApp;

namespace SetiaPaymentSampleWebApp.Controllers
{
    public class PaymentBradescoController : Controller
    {
        private SepsEntities db = new SepsEntities();

        /// <summary>
        /// Form
        /// </summary>
        /// <returns></returns>
        public ActionResult FormBoleto()
        {
            //Obtem dados da base local. Trecho somente de exemplo
            var boleto = new SepsEntities().Boleto.SqlQuery("QUERY PARA RETORNAR OS DADOS DO BOLETO. SOMENTE EXEMPLO").Single();

            String numeroPedido = boleto.order_id;
            ViewBag.numeroPedido = numeroPedido;
            return View("FormBoleto", boleto);
        }

        [HttpPost]
        public ActionResult PostFormUpdateBasket(Boleto boleto)
        {
            try
            {
                var boletoDB = db.Boleto.FirstOrDefault();

                boletoDB.merchant_id = boleto.merchant_id;
                boletoDB.cedente = boleto.cedente;

                boletoDB.order_id = boleto.order_id;
                boletoDB.banco = boleto.banco;
                boletoDB.numero_agencia = boleto.numero_agencia;
                boletoDB.numero_conta = boleto.numero_conta;
                boletoDB.url_logo_lojista = boleto.url_logo_lojista;
                boletoDB.mensagem_header_lojista = boleto.mensagem_header_lojista;
                boletoDB.numero_pedido = boleto.numero_pedido;
                boletoDB.ano_nosso_numero = boleto.ano_nosso_numero;
                boletoDB.cpf_sacado = boleto.cpf_sacado;

                db.SaveChanges();

                String numeroPedido = boleto.order_id;
                ViewBag.numeroPedido = numeroPedido;

            }
            catch (Exception e)
            {
                ViewBag.ResultadoOperacao = "Erro ao realizar operacao: " + e.Message.ToString();
            }

            return View("FormBoleto");
        }


        public ActionResult GenerateNewOrderId()
        {
            var boleto = db.Boleto.FirstOrDefault();

            String numeroPedido = new Random().Next(99999).ToString() + DateTime.Now.Millisecond.ToString();
            boleto.order_id = numeroPedido.ToString() + "ABC";
            boleto.numero_pedido = numeroPedido;

            db.SaveChanges();

            ViewBag.numeroPedido = numeroPedido;

            return View("FormBoleto", boleto);
        }


        /// <summary>
        /// Url disponibilizada para a Plataforma Bradesco consultar os dados do pedido
        /// </summary>
        /// <returns></returns>
        public ActionResult GetXmlCartData()
        {
            //Retorna o XML (Indicado fazer isso com WebApi)
            String numOrder = Request.QueryString["numOrder"];
            return Content(generateCartData(numOrder));
        }

        public ActionResult ViewXmlData()
        {
            String numOrder = Request.QueryString["numOrder"];
            ViewBag.xmlData = generateCartData(numOrder);

            return View("XmlData");
        }

        public JsonResult ViewStatusBoleto()
        {
            String numOrder = Request.QueryString["numOrder"];
            var provedor = new PagamentoBradesco();
            var data = PagamentoBradesco.ObterStatusBoleto(
                mercador: "", //MerchantId
                periodo: DateTime.Now, //Periodo
                administrador: "", //Administrador
                senha: "", //Senha
                pedido: numOrder //OrderId
                );

            return new JsonResult
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = data,
            };
        }

        /// <summary>
        /// Gera o XML com os dados do carrinho
        /// </summary>
        /// <returns></returns>
        private String generateCartData(String numOrder)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");

            //Obtem dados da base local
            Boleto boleto = db.Boleto.FirstOrDefault();

            //Ordem de cobranca
            var ordemCobranca = new OrdemCobranca();

            //Dados do pedido
            var pedido = new Pedido();

            if (!String.IsNullOrEmpty(numOrder))
            {
                pedido.NumeroPedido = numOrder;
            }
            else
            {
                pedido.NumeroPedido = boleto.order_id;
            }

            //Itens do pedido
            pedido.Itens = new ItemPedido[boleto.BoletoItens.Count];
            int i = 0;

            foreach (var item in boleto.BoletoItens)
            {
                pedido.Itens[i] = new ItemPedido { Descritivo = item.descritivo, Quantidade = Convert.ToInt32(item.quantidade), Unidade = "un", Valor = Convert.ToDecimal(item.valor) };
            }

            /*
            {
                new ItemPedido { Descritivo = "Produto 01", Quantidade = 1, Unidade = "cm", Valor = 120 },
                new ItemPedido { Descritivo = "Produto 02", Quantidade = 1, Unidade = "cm", Valor = 120 },
                new ItemPedido { Descritivo = "Produto 03", Quantidade = 1, Unidade = "cm", Valor = 120 }
            };
            */

            //Itens adicionais do pedido
            //Melhorar!
            pedido.ItemAdicional = new[] {
                new ItemAdicionalPedido { Descricao = "Item adicional", Valor = 100 }
            };

            //Atribui o pedido a ordem de cobranca
            ordemCobranca.Pedido = pedido;

            //Dados do boleto
            var boletoBancario = new BoletoBancario
            {
                Banco = boleto.banco,                                               //FIXO
                NumeroAgencia = boleto.numero_agencia,                              //CONFIG. ADMIN    
                NumeroConta = boleto.numero_conta,                                  //CONFIG. ADMIN
                CIP = Convert.ToInt16(boleto.cip),                                  //FIXO
                AnoNossoNumero = Convert.ToInt16(boleto.ano_nosso_numero),          //FIXO
                NumeroDocumento = Convert.ToInt32(boleto.numero_documento),         //PEDIDO
                Carteira = boleto.carteira,                            //FIXO
                ShoppingId = Convert.ToInt16(boleto.shopping_id),                             //CONFIG. ADMIN    
                Assinatura = boleto.assinatura, //CONFIG. ADMIN    
                //Valor = String.Format("{0:C}", 460),        //PEDIDO
                Valor = boleto.valor_documento,        //PEDIDO
                NossoNumero = boleto.numero_pedido,          //PEDIDO
                Cedente = boleto.cedente,                   //CONFIG. ADMIN - Razao Social da loja
                EnderecoSacado = boleto.endereco_sacado, //PEDIDO
                CepSacado = boleto.cep_sacado,                     //PEDIDO
                CidadeSacado = boleto.cidade_sacado,                 //PEDIDO
                UfSacado = boleto.uf_sacado,                            //PEDIDO
                //DataEmissao = String.Format("{0:dd/MM/yyyy}", DateTime.Now),               //PEDIDO 
                //DataProcessamento = String.Format("{0:dd/MM/yyyy}", DateTime.Now),         //PEDIDO
                //DataVencimento = String.Format("{0:dd/MM/yyyy}", DateTime.Now.AddDays(5)), //PEDIDO + CONFIG. ADMIN (DIAS VENCIMENTO)

                DataEmissao = boleto.data_emissao,               //PEDIDO 
                DataProcessamento = boleto.data_processamento,         //PEDIDO
                DataVencimento = boleto.data_vencimento, //PEDIDO + CONFIG. ADMIN (DIAS VENCIMENTO)

                CpfSacado = boleto.cpf_sacado,                  //PEDIDO
                NomeSacado = boleto.nome_sacado,    //PEDIDO
                MensagemCabecalho = boleto.mensagem_header_lojista,                       //CONFIG. ADMIN
                UrlLogotipo = boleto.url_logo_lojista,              //CONFIG. ADMIN
                Descricao = new[] {
                    boleto.instrucao_01,
                    boleto.instrucao_02,
                    boleto.instrucao_03,
                    boleto.instrucao_04,
                    boleto.instrucao_05
                }
            };

            //Atribui os dados do boleto a ordem de cobranca
            ordemCobranca.Boleto = boletoBancario;

            return new PagamentoBradesco().GerarXml(ordemCobranca);
        }

        /// <summary>
        /// Url disponibilizada para a Plataforma Bradesco notificar a loja sobre o resultado da operacao (Sucesso ou falha)
        /// Importante: Em caso de sucesso, será fornecido o parametro comB com a chave referente ao boleto gerado. 
        /// Para gerar a segunda via do boleto é necessário enviar esta chave 
        /// </summary>
        /// <returns></returns>
        public ActionResult StatusOperation()
        {
            var comb = Request.QueryString.Get("comB");
            //Outros parametros também são informados...

            //Apos receber estes dados, fazer a persistencia do COMB para gerar a segunda via do boleto bancario
            return new HttpStatusCodeResult(200);
        }

    }
}
