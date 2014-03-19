using System;
using System.Net;
using System.Reactive.Linq;

namespace Gohla.Shared
{
    public class ObservableStringWebRequest : ObservableWebRequest
    {
        public IObservable<String> Request(String url, Func<HttpWebRequest, HttpWebRequest> requestModifier)
        {
            IObservable<WebResponse> response = CreateResponse(CreateRequest(url, requestModifier));
            return response.SelectMany
            (
                r => ReponseToObservable<String, String>
                (
                    r,
                    s => HTTPUtility.Decode(r, s),
                    x => new String[] { x }.ToObservable()
                )
            );
        }
    }
}
