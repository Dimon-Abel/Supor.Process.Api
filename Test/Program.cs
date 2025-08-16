// See https://aka.ms/new-console-template for more information

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using ESign.Entity.Request;
using ESign.Helper;

Console.WriteLine("HTTP请求示例");

// 创建HttpClient实例
using var client = new HttpClient();

try
{
    var docPath = "D:\\Code\\doc\\e签宝调用接口清单.xlsx";
    var attachmentPath = "D:\\Code\\doc\\e签宝流程-updated.xlsx";

    var request = new SendRequest()
    {
        title = "测试传输文件流",
        Docs = new List<FileInformation>()
        {
            new FileInformation()
            {
                Name = Path.GetFileName(docPath),
                FileBytes = File.ReadAllBytes(docPath)
            }
        },
        Attachments = new List<FileInformation>()
        {
            new FileInformation()
            {
                Name = Path.GetFileName(attachmentPath),
                FileBytes = File.ReadAllBytes(attachmentPath)
            }
        }
    };

    // 发送POST请求示例
    Console.WriteLine("\n发送POST请求到示例API...");
    var postData = new StringContent(JsonSerializer.Serialize(request),
        System.Text.Encoding.UTF8, "application/json");

    client.Timeout = TimeSpan.FromSeconds(30000);
    HttpResponseMessage postResponse = await client.PostAsync(
        "https://localhost:44338/api/Task/Test", postData);

    string postResponseBody = await postResponse.Content.ReadAsStringAsync();
    Console.WriteLine($"响应状态: {postResponse.StatusCode}");
    Console.WriteLine($"创建的资源:\n{postResponseBody}");
}
catch (HttpRequestException e)
{
    Console.WriteLine($"\n请求错误: {e.Message}");
}