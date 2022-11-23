using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ConsoleClient
{
  class Program
  {
    static readonly string STS_HOST = "https://localhost:5001";
    static readonly string API_HOST = "https://localhost:5001";
    static readonly int AMOUNT_OF_STEPPERS = 100;

    static void Main(string[] args)
    {
      MainAsync(args).GetAwaiter().GetResult();
    }

    static async Task MainAsync(string[] args)
    {
      HttpClientHandler clientHandler = new HttpClientHandler
      {
        ServerCertificateCustomValidationCallback
        = (sender, cert, chain, sslPolicyErrors) => { return true; }
      };

      var httpClient = new HttpClient(clientHandler);
      httpClient.DefaultRequestHeaders.Clear();

      // Make the call and get the access token back
      TokenResponse response = await httpClient
        .RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        {
          Address = $"{STS_HOST}/connect/token",
          ClientId = "console_client",
          ClientSecret = "00000000-0000-0000-0000-000000000001",
          Scope = "webapi_scope"
        });

      // all good?
      if (!response.IsError)
      {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("SUCCESS!!");
        Console.WriteLine();
        Console.WriteLine("Access Token: ");
        Console.WriteLine(response.AccessToken);

        httpClient.SetBearerToken(response.AccessToken);

        var stepperAmount = args.Length == 1 ? int.Parse(args[0]) : AMOUNT_OF_STEPPERS;
        var steppers = GetSteppers(stepperAmount);
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
              Console.WriteLine($"Workflow for stepper id {stepperId} started...");
            }
          }
        }
      }
      else
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine("Failed to login with error:");
        Console.WriteLine(response.Error);
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
      var uri = $"{API_HOST}/api/stepper";

      return await client.PostAsync(
        uri,
        new StringContent(
          JsonSerializer.Serialize(stepper),
          Encoding.UTF8,
          "application/json"
         )
      );
    }

    static async Task<HttpResponseMessage> ProcessStepper(HttpClient client, int stepperId)
    {
      var uri = $"{API_HOST}/api/stepper/process";
      var jsonString = JsonSerializer.Serialize(new
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