using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Assignment.DBEntity.DataModels;

[Table("Task")]
public partial class Task
{
    [Key]
    public int Id { get; set; }

    [StringLength(128)]
    public string? TaskName { get; set; }

    [StringLength(128)]
    public string? Assignee { get; set; }

    public int? CategoryId { get; set; }

    [StringLength(128)]
    public string? Description { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? DueDate { get; set; }

    [StringLength(128)]
    public string? Category { get; set; }

    [StringLength(128)]
    public string? City { get; set; }

    [Column(TypeName = "bit(1)")]
    public BitArray? Isdeleted { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Tasks")]
    public virtual Category? CategoryNavigation { get; set; }
}
