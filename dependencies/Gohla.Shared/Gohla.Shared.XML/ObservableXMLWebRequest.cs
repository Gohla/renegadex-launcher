using System;
using System.Net;
using System.Reactive.Linq;
using System.Xml.Linq;

namespace Gohla.Shared.XML
{
    public class ObservableXMLWebRequest : ObservableWebRequest
    {
        public IObservable<XDocument> Request(String url, Func<HttpWebRequest, HttpWebRequest> requestModifier)
        {
            IObservable<WebResponse> response = CreateResponse(CreateRequest(url, requestModifier));
            return response.SelectMany
            (
                r => ReponseToObservable<XDocument, XDocument>
                (
                    r,
                    s => XDocument.Load(s),
                    x => new XDocument[] { x }.ToObservable()
                )
            );
        }
    }
}
