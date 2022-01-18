using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using Nancy.ErrorHandling;
using Nancy.ViewEngines;
using System.IO;
using System.Reflection;
using System.Drawing;

namespace CommService
{
    // Custom error handler class for HTTP 404 errors (PageNotFound)
    public class PageNotFoundHandler : DefaultViewRenderer, IStatusCodeHandler
    {
        public PageNotFoundHandler(IViewFactory factory) : base(factory)
        {
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return (statusCode == HttpStatusCode.NotFound);
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = RenderView(context, "content/views/error.html", Global.svc);
            Global.svc.val.lastError = "HTTP 404: Requested resource was not found.";
            response.StatusCode = HttpStatusCode.NotFound;
            context.Response = response;
        }
    }

    // Custom error handler class for HTTP 500 errors (InternalServerError)
    public class InternalServerErrorHandler : DefaultViewRenderer, IStatusCodeHandler
    {
        public InternalServerErrorHandler(IViewFactory factory) : base(factory)
        {
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return (statusCode == HttpStatusCode.InternalServerError);
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = RenderView(context, "content/views/error.html", Global.svc);
            Global.svc.val.lastError = "HTTP 500: Internal server error occurred.";
            response.StatusCode = HttpStatusCode.InternalServerError;
            context.Response = response;
        }
    }

    // Custom error handler class for HTTP 418 errors (I'm a teapot)
    public class ImATeapotHandler : DefaultViewRenderer, IStatusCodeHandler
    {
        public ImATeapotHandler(IViewFactory factory) : base(factory)
        {
        }

        public bool HandlesStatusCode(HttpStatusCode statusCode, NancyContext context)
        {
            return (statusCode == HttpStatusCode.ImATeapot);
        }

        public void Handle(HttpStatusCode statusCode, NancyContext context)
        {
            var response = RenderView(context, "content/views/error.html", Global.svc);
            Global.svc.val.lastError = "HTTP 418: I'm a teapot, not a coffee pot!";
            response.StatusCode = HttpStatusCode.ImATeapot;
            context.Response = response;
        }
    }

    public class BootStrapper : DefaultNancyBootstrapper
    {
        private byte[] favicon;

        protected override byte[] FavIcon
        {
            get { return this.favicon ?? (this.favicon = LoadFavIcon()); }
        }

        private byte[] LoadFavIcon()
        {
            // Get icon with reflection from currently executing assembly and return byte buffer to icon
            using (Icon appIcon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location))
            {
                var memoryStream = new MemoryStream();
                appIcon.Save(memoryStream);

                return (memoryStream.GetBuffer());
            }
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            // Enable GZIP compression for website with default settings
            pipelines.EnableGzipCompression();

            base.ApplicationStartup(container, pipelines);
            container.Register<Service>(Global.svc);
        }

        protected override NancyInternalConfiguration InternalConfiguration
        {
            get
            {
                NancyInternalConfiguration config = NancyInternalConfiguration.Default;

                return (config);
            }
        }

        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            nancyConventions.StaticContentsConventions.Clear();

            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("css", "/content/css"));
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("js", "/content/js"));
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("img", "/content/img"));
            nancyConventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("views", "/content/views"));
        }
    }
}
