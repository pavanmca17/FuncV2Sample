namespace FuncV2Sample
{
     public class WorkItem
    {
        public string Id { get; set; }
        public string SourceName { get; set; }
        public string WorkItemName { get; set; }
        public string WorkItemDescription { get;  set; }       
        public bool IsCompleted { get;  set; }    
        public string CreatedDateTime { get; set; }
    }
}