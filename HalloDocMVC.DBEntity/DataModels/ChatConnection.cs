using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.DBEntity.DataModels;

[Table("ChatConnection")]
public partial class ChatConnection
{
    [Key]
    public int ConnectionId { get; set; }

    [Column(TypeName = "character varying")]
    public string? ConnectionString { get; set; }

    [Column(TypeName = "character varying")]
    public string? Aspnetuserid { get; set; }

    [ForeignKey("Aspnetuserid")]
    [InverseProperty("ChatConnections")]
    public virtual Aspnetuser? Aspnetuser { get; set; }
}
