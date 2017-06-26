<%@ WebHandler Language="C#" Class="BarcodeHandler" %>

using System;
using System.Web;

public class BarcodeHandler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
            string code=context.Request.QueryString.ToString().Trim();
            context.Response.ClearContent();
            context.Response.ContentType = "image/png";            
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            MESComment.CommFunction.DrawBarCode(code).Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            context.Response.BinaryWrite(ms.ToArray());
            context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}