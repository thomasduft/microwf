using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClient
{
  // see here:
  //  - https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=netcore-2.2
  //  - https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/console-webapiclient
  class Program
  {
    static readonly string HOST = "https://localhost:5002";
    static readonly int AMOUNT_OF_STEPPERS = 100;
    static readonly bool USE_CLIENT_CREDENTIALS = true;

    static void Main(string[] args)
    {
      MainAsync(args).GetAwaiter().GetResult();
      Console.ReadKey();
    }

    static async Task MainAsync(string[] args)
    {
      var httpClient = new HttpClient();
      // Just a sample call with an invalid access token.
      // The expected response from this call is 401 Unauthorized
      var apiResponse = await httpClient.GetAsync($"{HOST}/api/workflow");
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
        "Bearer",
        "invalid_access_token"
      );

      // The API is protected, let's ask the user for credentials and exchanged them
      // with an access token
      if (apiResponse.StatusCode == HttpStatusCode.Unauthorized
        || apiResponse.StatusCode == HttpStatusCode.Forbidden)
      {
        httpClient.DefaultRequestHeaders.Clear();

        //Ask User
        var username = "admin";
        var password = "password";

        // Make the call and get the access token back
        TokenResponse response;
        if (!USE_CLIENT_CREDENTIALS)
        {
          response = await httpClient
            .RequestPasswordTokenAsync(new PasswordTokenRequest
            {
              Address = $"{HOST}/connect/token",
              GrantType = "password",
              ClientId = "ro.client",
              Scope = "api1",
              UserName = username,
              Password = password
            });
        }
        else
        {
          response = await httpClient
            .RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
              Address = $"{HOST}/connect/token",

              ClientId = "console.client",
              ClientSecret = "00000000-0000-0000-0000-000000000001",
              Scope = "api1"
            });
        }

        // all good?
        if (!response.IsError)
        {
          Console.ForegroundColor = ConsoleColor.Green;
          Console.WriteLine();
          Console.WriteLine("SUCCESS!!");
          Console.WriteLine();
          Console.WriteLine("Access Token: ");
          Console.WriteLine(response.AccessToken);

          // Call the API with the correct access token
          // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
          //   "Bearer",
          //   response.AccessToken
          // );
          httpClient.SetBearerToken(response.AccessToken);

          var steppers = GetSteppers(AMOUNT_OF_STEPPERS);
          foreach (var stepper in steppers)
          {
            var createStepperResponse = await CreateStepper(httpClient, stepper);
            if (createStepperResponse.IsSuccessStatusCode)
            {
              var content = await createStepperResponse.Content.ReadAsStringAsync();

              var stepperId = int.Parse(content);
              var processStepperResponse = await ProcessStepper(httpClient, stepperId);
              if (processStepperResponse.IsSuccessStatusCode)
              {
                Console.WriteLine($"Workflow for stepper id ${stepperId} started...");
              }
            }
          }

          var workflowResponse = await httpClient.GetAsync($"{HOST}/api/workflow");
          if (workflowResponse.IsSuccessStatusCode)
          {
            Console.WriteLine(await workflowResponse.Content.ReadAsStringAsync());
          }
        }
        else
        {
          Console.ForegroundColor = ConsoleColor.Red;
          Console.WriteLine();
          Console.WriteLine("Failed to login with error:");
          Console.WriteLine(response.ErrorDescription);
        }
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine("YOU ARE UNAUTHORIZED!!!");
      }
    }

    static IEnumerable<string> GetSteppers(int amount)
    {
      List<string> steppers = new List<string>();

      for (int i = 1; i <= amount; i++)
      {
        steppers.Add($"stepper{i}");
      }

      return steppers;
    }

    static async Task<HttpResponseMessage> CreateStepper(HttpClient client, string stepper)
    {
      var uri = $"{HOST}/api/stepper";

      return await client.PostAsync(
        uri,
        new StringContent(
          JsonConvert.SerializeObject(stepper),
          Encoding.UTF8,
          "application/json"
         )
      );
    }

    static async Task<HttpResponseMessage> ProcessStepper(HttpClient client, int stepperId)
    {
      var uri = $"{HOST}/api/stepper/process";
      var jsonString = JsonConvert.SerializeObject(new
      {
        Id = stepperId,
        Trigger = "goto1"
      });

      return await client.PostAsync(
        uri,
        new StringContent(
          jsonString,
          Encoding.UTF8,
          "application/json"
        )
      );
    }
  }
}
