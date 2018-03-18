using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NbgHackathon.Passport
{
    public class PassportParseResult
    {        
        public string TaskId { get; set; }
        public string ErrorMessage { get; set; }
        public string ExceptionInfo { get; set; }
        public PassportDocumentInfo PassportDocument { get; set; }

        public bool Success()
        {
            return string.IsNullOrEmpty(ErrorMessage) 
                && PassportDocument != null;
        }

        public void SetError(Exception ex)
        {
            ErrorMessage = ex?.Message;
            ExceptionInfo = ex?.ToString();
        }
    }

    public class PassportDocumentInfo
    {
        public string DocumentUrl { get; set; }
        public string MrzType { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string DocumentType { get; set; }
        public string DocumentSubtype { get; set; }
        public string IssuingCountry { get; set; }
        public string LastName { get; set; }
        public string GivenName { get; set; }
        public string DocumentNumber { get; set; }
        public bool DocumentNumberVerified { get; set; }
        public string DocumentNumberCheck { get; set; }
        public string Nationality { get; set; }
        public string BirthDate { get; set; }
        public bool BirthDateVerified { get; set; }
        public string BirthDateCheck { get; set; }
        public string Sex { get; set; }
        public string ExpiryDate { get; set; }
        public bool ExpiryDateVerified { get; set; }
        public string ExpiryDateCheck { get; set; }
        public string PersonalNumber { get; set; }
        public bool PersonalNumberVerified { get; set; }
        public string PersonalNumberCheck { get; set; }
        public bool ChecksumVerified { get; set; }
        public string Checksum { get; set; }

        internal static PassportDocumentInfo CreateFromDocumentUrl(string documentUrl)
        {
            var document = new PassportDocumentInfo();
            document.DocumentUrl = documentUrl;

            var xDoc = XDocument.Load(documentUrl);
            var ns = xDoc.Root.GetDefaultNamespace();

            var fieldElements = xDoc.Descendants(ns + "field").ToDictionary(xe => xe.Attribute("type").Value);
            var propertyInfos = typeof(PassportDocumentInfo).GetProperties().Where(pi => pi.Name != nameof(DocumentUrl)).ToDictionary(pi => pi.Name);

            foreach (var field in fieldElements)
            {
                var fieldName = field.Key;
                if (propertyInfos.TryGetValue(fieldName, out var prop))
                {
                    var valueElement = field.Value.Element(ns + "value");
                    var fieldValue = prop.PropertyType == typeof(bool) 
                        ? (object)(bool)valueElement 
                        : (string)valueElement;

                    prop.SetValue(document, fieldValue);
                }
            }

            return document;
        }
    }
}
