﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MesaCore.Models
{
    public partial class Sp_GetShipmentsResult
    {
        public int Id { get; set; }
        public int? DataNo { get; set; }
        public int? Packer { get; set; }
        public DateTime? Date { get; set; }
        public TimeSpan? Tiempo { get; set; }
        public string ShopOrder { get; set; }
        public string PartNumber { get; set; }
        public int? Qty { get; set; }
    }
}
