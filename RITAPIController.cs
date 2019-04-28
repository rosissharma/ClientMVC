using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Mvc;
using System.Web.Util;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Diagnostics;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FirstMVCApp.Controllers
{ 
    public class RITAPIController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult UnderGrad()
        {
            return View("UnderGrad");
        }

        public ActionResult Degrees() {
            return View("Degrees");
        }

        public ActionResult Map() {
            return View("Map");
        }

        //public async Task<JsonResult> SelectLocation()
        //{
        //    var returnedJSON = await GetLocationData();
        //    if (returnedJSON == null)
        //    {
        //        return ThrowJsonError(new Exception(String.Format("No location info found")));
        //    }
        //    Session["returnedJSON"] = returnedJSON;
        //    return null;
        //}


        //public static async Task<Object> GetLocationData() {
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri("http://www.ist.rit.edu/");
        //        client.DefaultRequestHeaders.Accept.Clear();
        //        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        //        /* ADD THE APPROPRIATE CODE TO RETRIEVE THE LOCATION DATA FROM THE API
                 
        //           The routine name is underlined in red because there is no "await" in it.  That is the code you have to add
        //           for retrieving the http data.  You have examples in the original FirstMVCApp program you downloaded from
        //           ourse content. */
        //    }
        //}



        public async Task<JsonResult> GetUnderGrad()
        {
            var returnedJSON = await GetUndergradDegrees("under");
            if (returnedJSON == null)
            {
                return ThrowJsonError(new Exception(String.Format("No Degree Programs found.")));
            }
            Session["returnedJSON"] = returnedJSON;
            return null;
        }

        public async Task<JsonResult> GetGrad()
        {
            var returnedJSON = await GetUndergradDegrees("grad");
            if (returnedJSON == null)
            {
                return ThrowJsonError(new Exception(String.Format("No Degree Programs found.")));
            }
            Session["returnedJSON"] = returnedJSON;
            return null;
        }

        public static async Task<Object> GetUndergradDegrees(string degree)
        {
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri("http://www.ist.rit.edu/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    if (degree == "under")
                    {
                        HttpResponseMessage response = await client.GetAsync("api/degrees/undergraduate/", HttpCompletionOption.ResponseHeadersRead);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsStringAsync();
                        var rtnResults = JsonConvert.DeserializeObject(data);
                        return rtnResults;
                    } else if(degree == "grad")
                    {
                        HttpResponseMessage response = await client.GetAsync("api/degrees/graduate/", HttpCompletionOption.ResponseHeadersRead);
                        response.EnsureSuccessStatusCode();
                        var data = await response.Content.ReadAsStringAsync();
                        var rtnResults = JsonConvert.DeserializeObject(data);
                        return rtnResults;
                    }
                    return "Not undergrad.";

                }
                catch (HttpRequestException hre)
                {
                    var msg = hre.Message;
                    return "HttpRequestException";
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    return "Exception";
                }
            }
        }

        public ActionResult SelectedDegree()
        {
            if (Session["returnedJSON"] != null)
            {
                return View("SelectDegree");
            }
            return RedirectToAction("Index"); 
        }

       
        public JsonResult ThrowJsonError(Exception e)
        {
            Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
            Response.StatusDescription = e.Message;

            return Json(new { Message = e.Message }, JsonRequestBehavior.AllowGet);
        }
    }
}
