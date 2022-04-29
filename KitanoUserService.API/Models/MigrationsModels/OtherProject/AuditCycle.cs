﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.MigrationsModels
{
    
    [Table("AUDIT_CYCLE")]
    public class AuditCycle
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(254)]
        public string Code { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public bool Status { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public string Description { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int? UserCreate { get; set; }
        public DateTime? LastModified { get; set; }
        public int? ModifiedBy { get; set; }
        public int DomainId { get; set; }
        public int RatingPoint { get; set; }
        public string RatingPointName { get; set; }
    }
}
