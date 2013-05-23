using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace GlucaTrack.Website.Content
{
    /// <summary>
    /// Returns the user image if passed the user id.
    /// </summary>
    public class UserImage : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        Queries.sp_GetLoginRow LoginRow = null;
        public void ProcessRequest(HttpContext context)
        {
            var pendingImg = ((byte[])context.Session["PendingAvatar"]);

            if (pendingImg != null && pendingImg.Length > 0)
            {
                //pending avatar change present to return it for the image
                context.Response.ContentType = "image/jpg";

                context.Response.BinaryWrite(pendingImg);
                return;
            }

            LoginRow = (Queries.sp_GetLoginRow)context.Session["LoggedInUser"];

            using (QueriesTableAdapters.sp_GetUserImageTableAdapter ta = new QueriesTableAdapters.sp_GetUserImageTableAdapter())
            {
                using (Queries.sp_GetUserImageDataTable userImage = new Queries.sp_GetUserImageDataTable())
                {
                    ta.Fill(userImage, LoginRow.user_id);

                    if (userImage.FirstOrDefault().IsimageNull())
                    {
                        //no image in database
                        context.Response.ContentType = "image/jpg";
                        var webClient = new System.Net.WebClient();
                        var bytes = webClient.DownloadData(new Uri(context.Request.Url, "/Images/master/blankavatar.jpg").AbsoluteUri);
                        context.Response.BinaryWrite(bytes);
                    }
                    else
                    {
                        //image found
                        context.Response.ContentType = "image/jpg";
                        context.Response.BinaryWrite(userImage.FirstOrDefault().image);
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}