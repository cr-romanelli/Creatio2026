namespace Terrasoft.Configuration
{
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using System.ServiceModel.Activation;
    using Terrasoft.Core.DB;
    using Terrasoft.Web.Common;
    using System;
    using System.Web.SessionState;
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class RealtyService : BaseService, IReadOnlySessionState //ereditando da Base Service eredita la user connection
    {// l'altro viene usato per le performance al fine di supportare il processamenento di piu metodi alla volta
        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]

        public decimal GetMaxPriceByTypeId(string realtyTypeId, string realtyOfferTypeId) //metodo
        {
            if (string.IsNullOrEmpty(realtyTypeId) || string.IsNullOrEmpty(realtyOfferTypeId))
            {
                return -1;
            }
            //recupero i dati direttamente dal database usando la classe Select
            Select select = new Select(UserConnection)
                .Column(Func.Max("TrnCPrice"))
                .From("TrnCRealty")
                //Le colonne provenienti da lookup avranno come suffisso ID in quanto FK
                .Where("TrnCTypeId").IsEqual(Column.Parameter(new Guid(realtyTypeId)))
                .And("TrnCOfferTypeId").IsEqual(Column.Parameter(new Guid(realtyOfferTypeId)))
                as Select;
            decimal result = select.ExecuteScalar<decimal>();
            return result;
        }
        [OperationContract]
        [WebInvoke(Method = "GET", BodyStyle = WebMessageBodyStyle.Wrapped,
            RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        public string GetExample() // METODO usato solo per debuggare il servizio e valutare se è stato compilato correttamente
        {
            return "OK!";
        }

    }
}