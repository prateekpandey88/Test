using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using SignixDemo.DataLayer;
using SignixDemo.Helpers;
using SignixDemo.Models;
using SignixDemo.Models.Entities;
using SignixDemo.ViewModels;

namespace SignixDemo.Controllers
{
    public class SignixController : ApiController
    {
        #region Properties

        private SignixUtility _signixUtility;
        public SignixUtility SignixUtility => _signixUtility ?? (_signixUtility = new SignixUtility());

        #endregion

        //test
        [HttpPost]
        public string SubmitDocument([FromBody] SubmitDocumentModel data)
        {
            //return "1562237169d:-4471:3b8c4d96:2x3f39";
            try
            {
                var documentSetId = SignixUtility.SubmitDocument(data);
                var submitDocument = new SendRequest
                {
                    DocumentName = data.FileName,
                    OriginalDocumentName = data.OriginalFileName,
                    Signer1Name = $"{data.Signers[0].FirstName} {data.Signers[0].MiddleName} {data.Signers[0].LastName}",
                    Signer2Name = $"{data.Signers[1].FirstName} {data.Signers[1].MiddleName} {data.Signers[1].LastName}",
                    SubmitterEmail = data.SubmitterEmail,
                    DocumentSetId = documentSetId,
                    CreatedOn = DateTime.Now
                };
                Repository.Instance.Insert(submitDocument);
                return documentSetId;
            }
            catch (Exception ex)
            {
                Common.WriteLog(ex.Message + "   " + ex.StackTrace);
                return null;
            }
        }

        public string GetAccessLink(string documentSetId)
        {
            try
            {
                return SignixUtility.GetAccessLink(documentSetId);
            }
            catch (Exception ex)
            {
                Common.WriteLog(ex.Message + "   " + ex.StackTrace);
                return null;
            }
        }

        [HttpPost]
        public string UploadContract()
        {
            try
            {
                var httpPostedFile = HttpContext.Current.Request.Files["file"];

                if (httpPostedFile == null) return string.Empty;

                var lBytes = httpPostedFile.ContentLength;

                var fileData = new byte[lBytes];
                httpPostedFile.InputStream.Read(fileData, 0, lBytes);

                return FileUploaderHelper.SaveFile(fileData).ToString();
            }
            catch (Exception ex)
            {
                Common.WriteLog(ex.Message + "   " + ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// This listenes the notifications send from the signix
        /// </summary>
        /// <param name="action">Type of the event occurred</param>
        /// <param name="id">Signix Document Set Id for the transaction</param>
        /// <param name="ts">Time of event occurrence in UTC (Zulu) Time Zone. Format is "yyyy-MM-dd'T'HH:mm:ss"</param>
        /// <param name="pid">The ordinal Id of the party that causes the event.</param>
        [HttpGet]
        public HttpStatusCode NotificationListener(string action, string id, string ts)
        {
            var notification = new Notification
            {
                Action = action,
                DocumentSetId = id,
                EventDateTime = ts,
                CreatedOn = DateTime.Now
            };

            Repository.Instance.Insert(notification);

            Common.WriteLog("GET: SignixNotificationListener executed");

            return HttpStatusCode.OK;
        }

        /// <summary>
        /// This listenes the notifications send from the signix
        /// </summary>
        /// <param name="action">Type of the event occurred</param>
        /// <param name="id">Signix Document Set Id for the transaction</param>
        /// <param name="ts">Time of event occurrence in UTC (Zulu) Time Zone. Format is "yyyy-MM-dd'T'HH:mm:ss"</param>
        /// <param name="pid">The ordinal Id of the party that causes the event.</param>
        [HttpGet]
        public HttpStatusCode NotificationListener(string action, string id, string ts, string pid)
        {
            var notification = new Notification
            {
                Action = action,
                DocumentSetId = id,
                EventDateTime = ts,
                PartyId = pid,
                CreatedOn = DateTime.Now
            };

            Repository.Instance.Insert(notification);

            Common.WriteLog("GET: SignixNotificationListener executed");

            return HttpStatusCode.OK;
        }

        public IEnumerable<RequestStatusVm> GetStatusData()
        {
            var requests = Repository.Instance.All<SendRequest>();
            var responses = Repository.Instance.All<Notification>();

            var requestStatuses = new List<RequestStatusVm>();

            foreach (var request in requests)
            {
                var matchedData = responses.Where(x => x.DocumentSetId.ToLower() == request.DocumentSetId.ToLower()).OrderByDescending(x => x.CreatedOn).FirstOrDefault();

                if (matchedData == null) continue;

                var status = string.Empty;

                if (matchedData.Action.Equals(SignixEvents.Complete, StringComparison.OrdinalIgnoreCase))
                {
                    status = "Completed";
                }
                else if (matchedData.Action.Equals(SignixEvents.Cancel, StringComparison.OrdinalIgnoreCase))
                {
                    status = "Cancelled";
                }
                else if (matchedData.Action.Equals(SignixEvents.Expire, StringComparison.OrdinalIgnoreCase))
                {
                    status = "Expired";
                }
                else if (matchedData.Action.Equals(SignixEvents.Suspend, StringComparison.OrdinalIgnoreCase))
                {
                    status = "Suspended";
                }
                else if (matchedData.Action.Equals(SignixEvents.PartyComplete, StringComparison.OrdinalIgnoreCase))
                {
                    status = "Signer 1 has completed its task.";
                }
                else if (matchedData.Action.Equals(SignixEvents.Send, StringComparison.OrdinalIgnoreCase))
                {
                    status = "Sent";
                }

                var item = new RequestStatusVm
                {
                    CreatedOn = request.CreatedOn,
                    DocumentSetId = request.DocumentSetId,
                    SubmitterEmail = request.SubmitterEmail,
                    Signer2Name = request.Signer2Name,
                    Signer1Name = request.Signer1Name,
                    OriginalDocumentName = request.OriginalDocumentName,
                    DocumentName = request.DocumentName,
                    Status = status
                };
                requestStatuses.Add(item);
            }
            return requestStatuses.OrderByDescending(x => x.CreatedOn);
        }

        public IEnumerable<NotificationUrls> GetNotificationUrlsData()
        {
            return Repository.Instance.All<NotificationUrls>();
        }

        protected override void Dispose(bool dispose)
        {
            Repository.Dispose();
            base.Dispose(dispose);
        }
    }
}
