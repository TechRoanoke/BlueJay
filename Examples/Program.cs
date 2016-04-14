using BlueJay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            // !!! Put your config information here !!!
            var endpoint = new Uri(File.ReadAllText(@"C:\temp\gop\config\endpoint.txt"));
            string clientId = File.ReadAllText(@"c:\temp\gop\config\clientId.txt");
            string secret  = File.ReadAllText(@"c:\temp\gop\config\secret.txt");

            BlueJayClient client = new BlueJayClient(endpoint, clientId, secret);

            Demo(client).Wait();
        }

        static async Task Demo(IBlueJayClient client)
        {
            // Get all suggested tags
            var myTags = await client.GetMyTagListAsync();

            var tags = await client.GetSuggestedTagListAsync("WA");

            // Lookup all voters that meet a criteria. 
            var q = new ExtendedLookupParameters
            {
                statelowerhousedistrict = "45",
                lastname = "Smith",
                state = "WA"
            };

            var all = await client.GetAllVotersAsync(q, (current, total) =>
                {
                    Console.WriteLine("Downloaded .. {0} of {1}", current, total);
                });


            // Lookup With explicit continuation 
            {
                var segment = await client.GetExtendedVoterLookupAsync(q);
                Console.WriteLine(segment.results[0].ToString());

                var s2 = await client.GetExtendedVoterLookupAsync(q, segment.next);
                Console.WriteLine(s2.results[0].ToString()); // verify different
            }

            // Post a contact result
            var person = all[0]; // pick any person

            var contact = await client.PostVoterContactAsync(person);
       
            var q1 = contact.NewDetails("Favorite color?", "Blue", "q1-color-blue");
            var q2 = contact.NewDetails("Favorite food?", "Pizza", "q2-pizza");
            await client.PostVoterContactDetailsAsync(q1, q2);
        }
    }
}
