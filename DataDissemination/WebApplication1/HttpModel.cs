using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1
{

    public class WcfReadEntityBodyModeWorkaroundModule : IHttpModule
    {
        public void Dispose()
        {

        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
        }
        public void context_BeginRequest(object sender, EventArgs e)
        {
            //这将强制HttpContext.Request.ReadEntityBody为“Classic”并确保兼容性。
            System.IO.Stream stream = (sender as HttpApplication).Request.InputStream;
        }
    }

}