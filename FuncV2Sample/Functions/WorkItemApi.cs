using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host;
using System.Linq;
using Microsoft.Extensions.Primitives;
using FuncV2Sample.Models;

namespace FuncV2Sample
{
        public static class WorkItemApi
        {
            static List<WorkItem> workItems = new List<WorkItem>();            

            [FunctionName("FunctionDemo_CreateWorkItem")]
            public static async Task<ActionResult> CreateTodo([HttpTrigger( AuthorizationLevel.Anonymous, 
                                                                           "post",
                                                                            Route = "createworkitem")] HttpRequest req, 
                                                                            TraceWriter  log)
            {
                log.Info($"Has Request Content Type { req.HasFormContentType}");
                log.Info($"Request Content Type { req.ContentType}");
                log.Info($"Request Content Length { req.ContentLength}");

                StringValues values;
                req.Headers.TryGetValue("sourcename", out values);
                string sourcename = values[0];
                log.Info($"Creating a new todo list item as request was recevied from source {sourcename}");
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();                
                WorkItemRequest requestmodel = JsonConvert.DeserializeObject<WorkItemRequest>(requestBody);
                WorkItem workItem = new WorkItem() { Id = Guid.NewGuid().ToString(),
                                                     SourceName = sourcename,
                                                     WorkItemName = requestmodel.WorkItemName,
                                                     WorkItemDescription = requestmodel.WorkItemName,
                                                     IsCompleted = false,
                                                     CreatedDateTime = DateTime.Now.ToString() };
                workItems.Add(workItem);
                return new OkObjectResult(workItem);
            }

            [FunctionName("FunctionDemo_GetAllWorkItems")]
            public static IActionResult GetWorkItems([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "WorkItems")]HttpRequest req)
            {
                return new OkObjectResult(workItems);
            }

            [FunctionName("FunctionDemo_GetWorkItemByID")]
            public static IActionResult GetWorkItemById([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "WorkItems/{id}")]HttpRequest req, string id)
            {
                var workItem = workItems.FirstOrDefault(t => t.Id == id);
                if (workItem == null)
                {
                    return new NotFoundResult();
                }
                return new OkObjectResult(workItem);
            }

            [FunctionName("FunctionDemo_UpdateWorkItemById")]
            public static async Task<IActionResult> UpdateWorkItem([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "WorkItems/{id}")]HttpRequest req, TraceWriter log, string id)
            {
                var workItem = workItems.FirstOrDefault(t => t.Id == id);
                if (workItem == null)
                {
                    return new NotFoundResult();
                }

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var updaterequest = JsonConvert.DeserializeObject<WorkItemRequest>(requestBody);

                workItem.WorkItemName = updaterequest.WorkItemName;
                if (!string.IsNullOrEmpty(updaterequest.WorkItemDescription))
                {
                    workItem.WorkItemDescription = updaterequest.WorkItemDescription;
                }

                return new OkObjectResult(workItem);
            }

            [FunctionName("FunctionDemo_DeleteWorkItemById")]
            public static IActionResult DeleteWorkItem([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "WorkItems/{id}")]HttpRequest req, TraceWriter log, string id)
            {
                var workItem = workItems.FirstOrDefault(t => t.Id == id);
                if (workItem == null)
                {
                    return new NotFoundResult();
                }
                workItems.Remove(workItem);
                return new OkResult();
            }
        }
    }

