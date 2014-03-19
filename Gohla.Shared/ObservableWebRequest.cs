using System;
using System.IO;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Gohla.Shared
{
    public class ObservableWebRequest
    {
        public IObservable<HttpWebRequest> CreateRequest(String request, 
            Func<HttpWebRequest, HttpWebRequest> requestModifier)
        {
            // Run web requests in a background thread using Observable.Return with a scheduler.
            // http://stackoverflow.com/a/7465720/499240
            return Observable
                .Return(requestModifier((HttpWebRequest)HttpWebRequest.Create(request)), Scheduler.Default);
        }

        public IObservable<WebResponse> CreateResponse(IObservable<HttpWebRequest> request)
        {
            return request
                .SelectMany(r => Observable.FromAsyncPattern<WebResponse>(r.BeginGetResponse, r.EndGetResponse)());
        }

        public IObservable<R> ReponseToObservable<R, T>(
            WebResponse response, 
            Func<Stream, T> deserializer,
            Func<T, IObservable<R>> converter,
            Func<Stream, String> errorParser = null
        )
        {
            Stream stream = response.GetResponseStream();

            try
            {
                T item = deserializer(stream);
                return converter(item);
            }
            catch(System.Exception e)
            {
                if(errorParser != null)
                {
                    String error = errorParser(stream);
                    if(error != null)
                        return Observable.Throw<R>(new System.Exception(error));
                    else
                        return Observable.Throw<R>(e);
                }
                else
                    return Observable.Throw<R>(e);
            }
            finally
            {
                stream.Dispose();
                response.Close();
            }
        }
    }
}
