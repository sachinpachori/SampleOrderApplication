using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sample.Order.BE.Data.HttpClients
{
    public abstract class HttpClientBase
    {

        protected HttpClient client;
        protected readonly ILogger logger;

        public HttpClientBase(HttpClient client, ILogger logger)
        {
            this.client = client;
            this.logger = logger;
        }

        #region Generic, Async, static HTTP functions for GET, POST, PUT, and DELETE  

        protected async Task<T> GetAsync<T>(string url)
        {
            T data;

            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                var uri = new Uri(client.BaseAddress, url);

                var response = await client.GetAsync(uri);
                using (HttpContent content = response.Content)
                {
                    string d = await content.ReadAsStringAsync();
                    if (d != null)
                    {
                        data = JsonConvert.DeserializeObject<T>(d);
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception GetAsync - base address: {0},  url :{1}", client.BaseAddress, url);
                throw;
            }
            finally
            {
                sw.Stop();
                logger.LogDebug("GetAsync {Url} Elaspsed time:{et}", url, sw.ElapsedMilliseconds);
            }
            object o = new object();
            return (T)o;
        }

        protected async Task<T> PostAsync<T>(string url, HttpContent contentPost)
        {
            T data;
            Stopwatch sw = new Stopwatch();
            try
            {
                sw.Start();
                var response = await client.PostAsync(url, contentPost);
                using (HttpContent content = response.Content)
                {
                    string d = await content.ReadAsStringAsync();
                    if (d != null)
                    {
                        data = JsonConvert.DeserializeObject<T>(d);
                        return data;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception PostAsync - base address: {0},  url :{1}", client.BaseAddress, url);
                throw;
            }
            finally
            {
                sw.Stop();
                logger.LogDebug("PostAsync {Url} Elaspsed time:{et}", url, sw.ElapsedMilliseconds);
            }

            object o = new object();
            return (T)o;
        }

        protected async Task<T> PutAsync<T>(string url, HttpContent contentPut)
        {
            T data;

            using (HttpResponseMessage response = await client.PutAsync(url, contentPut))
            using (HttpContent content = response.Content)
            {
                string d = await content.ReadAsStringAsync();
                if (d != null)
                {
                    data = JsonConvert.DeserializeObject<T>(d);
                    return data;
                }
            }
            object o = new object();
            return (T)o;
        }

        protected async Task<T> DeleteAsync<T>(string url)
        {
            T newT;

            using (HttpResponseMessage response = await client.DeleteAsync(url))
            using (HttpContent content = response.Content)
            {
                string data = await content.ReadAsStringAsync();
                if (data != null)
                {
                    newT = JsonConvert.DeserializeObject<T>(data);
                    return newT;
                }
            }
            object o = new object();
            return (T)o;
        }
        #endregion
    }


}
