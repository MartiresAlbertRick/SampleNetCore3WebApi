using System;

namespace AD.CAAPS.Entities
{
    public partial class Comment
    {
        public int ID { get; set; }
        public int ManagedTableID { get; set; }
        public int RecordID { get; set; }
        public string UserName { get; set; }
        public string ActionName { get; set; }
        public string ActionComments { get; set; }
        public string RecordStatus { get; set; }
        public string ActionDetail { get; set; }
        public string ActionDetailA { get; set; }
        public string ActionDetailB { get; set; }
        public string ActionDetailC { get; set; }
        public DateTime? ActionStartDate { get; set; }
        public DateTime? ActionEndDate { get; set; }
        public int? ExecutionTime { get; set; }
        public int? ColorIndex { get; set; }
    }
}