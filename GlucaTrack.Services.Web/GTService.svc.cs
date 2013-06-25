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
    public class GTService : IGTService
    {
        private static List<Guid> Authenticated = new List<Guid>();

        /// <summary>
        /// Validates the users login and password sent in, and returns the users information if valid.
        /// </summary>
        /// <param name="Username">Username to validate.</param>
        /// <param name="Password">Password to validate.</param>
        /// <returns>User information</returns>
        public Common ValidateLogin(string Username, string Password)
        {
            lock (Authenticated)
            {
                Authenticated.Clear();
            }

            using (CommonTableAdapters.sp_GetLoginTableAdapter ta = new CommonTableAdapters.sp_GetLoginTableAdapter())
            using (Common d = new Common())
            {
                try
                {
                    ta.Fill(d.sp_GetLogin, Username, Password);
                    
                    if (d.sp_GetLogin != null && d.sp_GetLogin.Count == 1)
                    {
                        //login accepted start session
                        Guid newSessionId = Guid.NewGuid();
                        
                        lock (Authenticated)
                        {
                            Authenticated.Add(newSessionId);
                        }
                        
                        //update user entry with the latest session id
                        using (CommonTableAdapters.QueriesTableAdapter queries = new CommonTableAdapters.QueriesTableAdapter())
                        {
                            queries.sp_UpdateUserSession(d.sp_GetLogin.First().user_id, newSessionId);
                        }

                        d.sp_GetLogin.First().sessionid = newSessionId;

                        return d;
                    }
                    else
                    {
                        //no user by that name exists
                        throw new FaultException("Login failed.");
                    }
                }
                catch (Exception ex)
                {
                    if (ex is FaultException)
                        throw ex;
                    else
                        throw new FaultException("Unexpected error: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Writes the users records to the glucose records table and attaches them to their userid.
        /// </summary>
        /// <param name="records">Glucose records from the users meter.</param>
        /// <param name="user">User information obtained from validating login with the webservice.</param>
        /// <param name="MeterType">Meter type id that corresponds with the primary key in the MeterTypes database table</param>
        /// <returns>Were the records successfully written to the database.</returns>
        public bool PostGlucoseRecords(Records records, Common user, int MeterType)
        {
            bool result = false;

            if (user.sp_GetLogin == null)
            {
                return result;
            }

            lock (Authenticated)
            {
                if (Authenticated.Count > 0 && Authenticated.Contains(user.sp_GetLogin.FirstOrDefault().sessionid))
                {
                    //authenticated session so allow post
                    Common.sp_GetLoginRow userinfo = null;
                    try
                    {
                        userinfo = user.sp_GetLogin.First();
                    }
                    catch(Exception ex)
                    {
                        if (userinfo == null)
                        {
                            throw new FaultException("PostGlucoseRecords-1: Invalid or blank user information detected.");
                        }
                        else
                        {
                            throw new FaultException("PostGlucoseRecords-1: " + ex.Message);
                        }
                    }
                    
                    try
                    {
                        using (CommonTableAdapters.QueriesTableAdapter queries = new CommonTableAdapters.QueriesTableAdapter())
                        {
                            foreach (Records.RecordRow row in records.Record)
                            {
                                //write record to database attaching to user
                                try
                                {
                                    queries.sp_PostGlucoseResult(row.Timestamp, row.Glucose, row.Units, userinfo.user_id, MeterType);
                                }//try
                                catch (Exception ex)
                                {
                                    if (ex.Message.ToLowerInvariant().Contains("violation of unique key constraint"))
                                        continue;
                                    else
                                        throw ex;
                                }
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        throw new FaultException("PostGlucoseRecords-2: " + ex.Message);
                    }

                    result = true;
                }
                else
                {
                    //not authenticated
                    throw new FaultException("PostGlucoseRecords-3: Session not authenticated.");
                }
            }

            return result;
        }

        /// <summary>
        /// Record the last sync date and time on the users record in the database.
        /// </summary>
        public bool UpdateLastSync(Common user)
        {
            bool result = false;

            if (user.sp_GetLogin == null)
            {
                return result;
            }

            lock (Authenticated)
            {
                if (Authenticated.Count > 0 && Authenticated.Contains(user.sp_GetLogin.FirstOrDefault().sessionid))
                {
                    using (CommonTableAdapters.QueriesTableAdapter queries = new CommonTableAdapters.QueriesTableAdapter())
                    {
                        queries.sp_UpdateLastSync(user.sp_GetLogin.First().user_id);
                        result = true;
                    }
                }
                else
                {
                    //not authenticated
                    throw new FaultException("UpdateLastSync-1: Session not authenticated.");
                }
            }

            return result;
        }

    }
}
