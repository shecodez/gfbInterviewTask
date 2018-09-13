using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace ConsoleApplicationTaskGFB
{
    public class PostModel
    {   
        public int userId { get; set; }    
        public int id { get; set; }     
        public string title { get; set; }    
        public string body { get; set; }
    }

    class Program
    {
        private static HttpClient client = new HttpClient();

        static List<PostModel> posts { get; set; }

        public static async Task<List<PostModel>> getPostsAsync()
        {
            string uri = "https://jsonplaceholder.typicode.com/Posts";

            var response = await client.GetAsync(uri);
            
            // 3. Verify that the server responds to that call with an HTTP status code of 200 / OK
            response.EnsureSuccessStatusCode();
            
            // 4. Read the content of the response into a string
            string content = await response.Content.ReadAsStringAsync();
            
            // 5. Deserialize the response string into an array or list of type PostModel
            var data = JsonConvert.DeserializeObject<List<PostModel>>(content);

            return data;
        }

        public static void saveToTxt(string filePath, List<PostModel> posts)
        {
            try
            {
                TextWriter tw = new StreamWriter(@filePath);
                foreach (var post in posts)
                {
                    string body = post.body.ToString().Replace("\n", " ");
                    tw.WriteLine(string.Format("{0};{1};{2};{3}", post.id.ToString(), body, post.title, post.userId.ToString()));
                }
                tw.Close();
                Console.WriteLine("Data saved! " + filePath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        static void Main(string[] args)
        {
            // 1. Accept an optional parameter containing a valid file path (e.g.: "C:\temp\output.txt")            
            string filePath;
            Console.WriteLine("Enter a file path to save the API data: ");
            filePath = Console.ReadLine(); 
            
            try
            {
                // 2. Call the API at https://jsonplaceholder.typicode.com/Posts using the HttpClient class
                var t = getPostsAsync();
                t.Wait();
                
                // 6. Order that array or list by the "Title" property, in ascending order
                List<PostModel> postSortedByTitleAsc = t.Result.OrderBy(o => o.title).ToList();

                foreach (var post in postSortedByTitleAsc)
                {
                    // 7. Remove all newline characters from the "Body" property
                    string body = post.body.ToString().Replace("\n", " ");

                    // 8. Write all of the properties of each PostModel to the display console on a single line
                    // separated by semi-colons in the order of Id, Body, Title, UserId (e.g.: "Id;Body;Title;UserId")
                    Console.WriteLine($"{post.id};{body};{post.title};{post.userId}");
                }

                // 9. Write the same output that is being written to the console to the specified file, in the same order
                if (!string.IsNullOrEmpty(filePath.Trim()))
                {
                    saveToTxt(filePath, postSortedByTitleAsc);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Press any key to Exit.");
            Console.ReadKey();
        }
    }
}