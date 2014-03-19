using System;
using System.Net;
using System.Reactive.Linq;
using Newtonsoft.Json.Linq;

namespace Gohla.Shared.Json
{
    public class ObservableJSONWebRequest : ObservableWebRequest
    {
        public IObservable<JObject> Request(String url, Func<HttpWebRequest, HttpWebRequest> requestModifier)
        {
            IObservable<WebResponse> response = CreateResponse(CreateRequest(url, requestModifier));
            return response.SelectMany
            (
                r => ReponseToObservable<JObject, JObject>
                (
                    r,
                    s => JObject.Parse(HTTPUtility.Decode(r, s).Replace("\\x", "\\u00")), // JSON.net does not support hexadecimal escapes.
                    x => Observable.Return(x)
                )
            );
        }
    }
}
