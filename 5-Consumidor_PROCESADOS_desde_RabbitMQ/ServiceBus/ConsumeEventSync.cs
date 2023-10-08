using System;  
using System.Collections.Generic;  
using System.Linq;  
using System.Net;  
using System.Text;  
namespace Consumidor_Patentes_Rabbit
{
    public class ConsumeEventSync
    {
        public void GetAllEventData() //Get All Events Records  
        {
            using (var client = new WebClient()) //WebClient  
            {
                client.Headers.Add("Content-Type:application/json"); //Content-Type  
                client.Headers.Add("Accept:application/json");
                var result = client.DownloadString("http://localhost:50024/api/Event"); //URI  
                Console.WriteLine(Environment.NewLine + result);
            }
        }
    }
}
