using System;
using System.ServiceModel;

namespace BloodSpider.Services.Web
{
    [ServiceContract]
    public interface IGTService
    {
        [OperationContract]
        [FaultContract(typeof(Exception))]
        Common ValidateLogin(string AssemblyName, string Appid, string Username, string Password);

        [OperationContract]
        [FaultContract(typeof(Exception))]
        bool PostGlucoseRecords(BloodSpider.Communication.Records records, Common user, int metertype);

        [OperationContract]
        [FaultContract(typeof(Exception))]
        void UpdateLastWebLogin(Common user);

        [OperationContract]
        [FaultContract(typeof(Exception))]
        string IsUpdatePresent(string appid, string version);

        [OperationContract]
        [FaultContract(typeof(Exception))]
        void ReportBug(string appid, string ErrorCode, string StackTrace, string Message, string Version);
    }
}
