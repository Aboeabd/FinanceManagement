using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Duende.IdentityServer.Models;
using Newtonsoft.Json.Linq;

namespace FinanceManagement.Api.utils
{
    public class HashedClientSecret
    {
        public void Write(string secretToReplace)
        {
            try
            {
                // Read the existing configuration
                var json = File.ReadAllText("FinanceManagement.Api/appsettings.json");
                var root = JObject.Parse(json);

                // Modify the configuration
                var clients = root["IdentityServer"]["Clients"];
                var client = clients[0];
                var clientSecrets = client["ClientSecrets"];
                var secretObject = clientSecrets[0];
                secretObject["Value"] = secretToReplace.Sha256(); // Replace with your new secret

                // Write the modified configuration back to the file
                File.WriteAllText("appsettings.json", root.ToString());
                Console.WriteLine("Secret has been updated successfully in appsettings.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
