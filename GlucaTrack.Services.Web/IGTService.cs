using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using GlucaTrack.Communication;
using GlucaTrack.Services.Common;

namespace GlucaTrack.Services.Web
{
    [ServiceContract]
    public interface IGTService
    {
        [OperationContract]
        [FaultContract(typeof(Exception))]
        Common ValidateLogin(string Username, string Password);

        [OperationContract]
        [FaultContract(typeof(Exception))]
        bool PostGlucoseRecords(GlucaTrack.Communication.Records records, Common user, int metertype);

        [OperationContract]
        [FaultContract(typeof(Exception))]
        bool UpdateLastSync(Common user);
    }
}
