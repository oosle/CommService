using Nancy;
using System;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using SG.GeneralLib;

namespace CommService
{
    public class MainMod : NancyModule
    {
        public MainMod(Service service)
        {
            //-------------------------------------------------------------------------------------
            // Main website endpoints, status and config
            //-------------------------------------------------------------------------------------

            Get["/"] = index =>
            {
                service.val.configJson = new JavaScriptSerializer().Serialize(service.cfg);

                return (View["content/views/index.html", service]);
            };

            Get["/config"] = config =>
            {
                service.val.configJson = new JavaScriptSerializer().Serialize(service.cfg);
                service.val.configJson = service.val.configJson.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                service.val.configJson = service.val.configJson.Replace("\r\n", "<br>");

                return (View["content/views/config.html", service]);
            };

            //-------------------------------------------------------------------------------------
            // REST endpoints, what you want really, bind JSON directly to model, no WCF!
            //-------------------------------------------------------------------------------------

            // NOTES:
            // Nancy is an embedded light weight webserver and can host a WCF base with a few
            // modifications if existing code needs to be ported without the dreaded IIS bloatware!
            // 
            // PM> Install-Package Nancy.Hosting.Wcf
            // Then host it with WCF
            //
            // var host = new WebServiceHost(new NancyWcfGenericService(), new Uri("http://localhost:1234/base/"));
            // host.AddServiceEndpoint(typeof(NancyWcfGenericService), new WebHttpBinding(), "");
            // host.Open();
            //
            // Nancy will now handle requests to http://localhost:1234/base/

            Get["/model/{id}"] = p_model =>
            {
                var v_model = new string[] { "model" };
                string id = p_model.id.ToString();

                return (Response.AsJson(v_model));
            };
        }
    }
}
