using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Xml.Serialization;
using SignixDemo.ViewModels;

namespace SignixDemo.Helpers
{
    public class SignixUtility
    {
        private const string Namespace = "urn:com:signix:schema:sdddc-1-1";

        public string UploadFolderDate => DateTime.Now.ToString("MMMM dd, yyyy");
        public string ServerUploadFolderPath => HttpContext.Current.Server.MapPath("~/UploadFiles");

        private static string SignixClient => WebConfigurationManager.AppSettings["SignixClient"];
        private static string SignixSponsor => WebConfigurationManager.AppSettings["SignixSponsor"];
        private static string SignixUserId => WebConfigurationManager.AppSettings["SignixUserId"];
        private static string SignixPassword => WebConfigurationManager.AppSettings["SignixPassword"];
        private static string SignixEmailId => WebConfigurationManager.AppSettings["SignixEmailId"];

        private static string GetSignixUrl()
        {
            bool isProduction;

            if (!bool.TryParse(WebConfigurationManager.AppSettings["IsProduction"], out isProduction))
                isProduction = false;

            return isProduction
                ? WebConfigurationManager.AppSettings["SignixLiveUrl"]
                : WebConfigurationManager.AppSettings["SignixTestUrl"];
        }

        public string PostDataToSignix(string methodName, string xmlData)
        {
            var postString = $"method={HttpUtility.UrlEncode(methodName, Encoding.UTF8)}&request={HttpUtility.UrlEncode(xmlData, Encoding.UTF8)}";

            try
            {
                Common.WriteLog(methodName);
                Common.WriteLog(xmlData);

                //Create WebRequest to communicate with Signix
                var request = (HttpWebRequest)WebRequest.Create(GetSignixUrl());
                request.Method = "POST";
                request.ContentLength = postString.Length;
                request.ContentType = "application/x-www-form-urlencoded";

                //Write request to Signinx
                var requestStream = new StreamWriter(request.GetRequestStream());
                requestStream.Write(postString);
                requestStream.Close();

                //Read Response From Signix
                var response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var signixRepsonse = reader.ReadToEnd();
                reader.Close();

                Common.WriteLog(signixRepsonse);

                return signixRepsonse;

            }
            catch (Exception ex)
            {
                Common.WriteLog(ex.Message + "   " + ex.StackTrace);
                return null;

            }
        }

        public string GetAccessLink(string documentSetId)
        {
            var data = new GetAccessLink
            {
                CustInfo = new CustInfo
                {
                    Client = SignixClient,
                    Sponsor = SignixSponsor,
                    UserId = SignixUserId,
                    Pswd = SignixPassword
                },
                Data = new Data
                {
                    AllowParticipantEditing = false,
                    AllowDocumentEditing = false,
                    AllowDocumentFieldAndTaskEditing = true,
                    AllowTransactionContextEditing = true,
                    AllowTransactionStatusViewing = false,
                    DocumentSetId = documentSetId,
                    TransactionRole = "submitter",
                    UseMyDoXWizard = true,
                    //Embedded = "iframe",
                    //SkinID = "Twilight",
                    ContainerOrigin = "https://webtest.signix.biz",
                    FinishURL = "https://dev-app.ncontracts.com/SignixDemo/api/Signix/NotificationListener"
                }
            };

            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(GetAccessLink), Namespace);
            serializer.Serialize(stringwriter, data);

            var response = PostDataToSignix("GetAccessLink", stringwriter.ToString());

            using (var xmlReader = new StringReader(response))
            {
                var deserializer = new XmlSerializer(typeof(GetAccessLinkRs));
                var result = (GetAccessLinkRs)deserializer.Deserialize(xmlReader);
                return result.Data.AccessLink;
            }
        }

        public string GetAccessToken(string documentSetId)
        {
            var data = new GetAccessTokenRq
            {
                CustInfo = new CustInfo
                {
                    Client = SignixClient,
                    Sponsor = SignixSponsor,
                    UserId = SignixUserId,
                    Pswd = SignixPassword
                },
                Data = new GetAccessTokenRqData
                {
                    DocumentSetId = documentSetId
                }
            };

            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(GetAccessTokenRq), Namespace);
            serializer.Serialize(stringwriter, data);

            var response = PostDataToSignix("GetAccessToken", stringwriter.ToString());

            using (var xmlReader = new StringReader(response))
            {
                var deserializer = new XmlSerializer(typeof(GetAccessTokenRs));
                var result = (GetAccessTokenRs)deserializer.Deserialize(xmlReader);
                return result.Data.AccessToken;
            }
        }

        public string SubmitDocument(SubmitDocumentModel model)
        {
            var fileBytes = FileUploaderHelper.GetFileBytes(new Guid(model.FileName));

            var base64Data = Convert.ToBase64String(fileBytes);

            var fileName = string.IsNullOrWhiteSpace(model.OriginalFileName) ? null : model.OriginalFileName.Split(new[] { ".pdf" }, StringSplitOptions.RemoveEmptyEntries)[0];

           // var workflowOrder = 1;

            var data = new SubmitDocument
            {
                CustInfo = new SubmitDocumentCustInfo
                {
                    Client = SignixClient,
                    Sponsor = SignixSponsor,
                    UserId = SignixUserId,
                    Pswd = SignixPassword,
                    //BillingRef = Substring(0, 50)
                    Demo = "yes",
                    //EmailContent = (model.EmailContent ?? "Your documents for the Sample Application are available online for viewing and signing.").Substring(0, 10000),
                    EmailContent = string.IsNullOrWhiteSpace(model.EmailContent) ? "Your documents for the Sample Application are available online for viewing and signing." : model.EmailContent,
                    Workgroup = "SDD",
                    DelDocsAfter = 20,
                    ExpireAfter = 20
                },
                Data = new SubmitDocumentData
                {
                    TransactionID = DateTime.Now.ToString("Ncontr mm/dd/yyyy hh:mm:ss tt zzz"),
                    //DocSetDescription = (model.DocSetDescription ?? "Sample Document Set").Substring(0, 100),
                    DocSetDescription = string.IsNullOrWhiteSpace(model.DocSetDescription) ? "Sample Document Set" : model.DocSetDescription,
                    //FileName = "Jack-ncontractstest_02242005_032557.zip".Substring(0, 255),
                    FileName = "Jack-ncontractstest_02242005_032557.zip",
                    SubmitterEmail = string.IsNullOrWhiteSpace(model.SubmitterEmail) ? SignixEmailId : model.SubmitterEmail,
                    //ContactInfo = (model.ContactInfo ?? "If you have a question, please contact to submitter at 800-555-1234.").Substring(0, 1000),
                    ContactInfo = string.IsNullOrWhiteSpace(model.ContactInfo) ? "If you have a question, please contact to submitter at 800-555-1234." : model.ContactInfo,
                    DeliveryType = "SDDDC",
                    SuspendOnStart = "yes",
                    UseMyDoX = "latest",
                    DistributionEmailContent = string.IsNullOrWhiteSpace(model.DistributionEmailContent) ? null : model.DistributionEmailContent,
                    DistributeToSubmitter = "yes",
                    //DistributionEmailList = model.DistributionEmailList + "; prateekpandey_19@yahoo.com",
                    ClientPreference = new[] {new ClientPreference
                    {
                        Type = "AutoAdvanceToNextSignature",
                        Value = "no"
                    },
                    new ClientPreference
                    {
                        Type = "AutoAdvanceToNextDocument",
                        Value = "no"
                    },
                    new ClientPreference
                    {
                        Type = "AutoOpenFirstDocument",
                        Value = "no"
                    }, new ClientPreference
                    {
                        Type = "NextPartyLink",
                        Value = "no"
                    },
                    new ClientPreference
                    {
                        Type = "NotificationSchedule",
                        Value = "{5, 10, 15, 20}days"
                    } },
                    MemberInfo = model.Signers.Where(x => !string.IsNullOrWhiteSpace(x.Email)).Select(y =>
                    new MemberInfo
                    {
                        RefID = "First Signer",
                        SSN = "000000000",
                        DOB = "00/00/0000",
                        //FirstName = y.FirstName.Substring(0, 30),
                        //MiddleName = y.MiddleName.Substring(0, 30),
                        //LastName = y.LastName.Substring(0, 30),
                        FirstName = y.FirstName,
                        MiddleName = string.IsNullOrWhiteSpace(y.MiddleName) ? null : y.MiddleName,
                        LastName = y.LastName,
                        Email = y.Email,
                        Service = "select"
                        //WorkflowOrder = workflowOrder++
                    }).ToArray(),
                    Form = new Form
                    {
                        RefID = "ncontractstestForm1",
                        Desc = string.IsNullOrWhiteSpace(fileName) ? "Document" : fileName,
                        FileName = "SampleAgreement.pdf",
                        MimeType = "application/pdf",
                        Length = fileBytes.Length.ToString(),
                        FileData = base64Data
                    }
                }
            };

            var stringwriter = new StringWriter();
            var serializer = new XmlSerializer(typeof(SubmitDocument), Namespace);
            serializer.Serialize(stringwriter, data);

            var response = PostDataToSignix("SubmitDocument", stringwriter.ToString());

            using (var xmlReader = new StringReader(response))
            {
                var deserializer = new XmlSerializer(typeof(SubmitDocumentRs));
                var result = (SubmitDocumentRs)deserializer.Deserialize(xmlReader);
                return result.Data.DocumentSetID;
            }
        }
    }
}