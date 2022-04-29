using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class SystemLogModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string module { get; set; }
        public string name { get; set; }
        public string perform_tasks { get; set; }
        public string version { get; set; }
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime datetime { get; set; }
    }

    public class SystemLogDataModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string module { get; set; }
        public string name { get; set; }
        public string perform_tasks { get; set; }
        public string version { get; set; }
        public string datetime { get; set; }
    }

    public class SystemLogSearchModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string module { get; set; }
        public string name { get; set; }
        public string perform_tasks { get; set; }
        public string version { get; set; }
        public DateTime? datetime { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int start_number { get; set; }
        public int page_size { get; set; }
    }
    public class SystemLogCreateModel
    {
        public string module { get; set; }
        //public string name { get; set; }
        public string perform_tasks { get; set; }
        public string version { get; set; }
    }

}
