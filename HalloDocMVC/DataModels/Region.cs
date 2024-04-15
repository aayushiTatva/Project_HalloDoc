using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.DataModels;

[Table("region")]
public partial class Region
{
    [Key]
    [Column("regionid")]
    public int Regionid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("abbreviation")]
    [StringLength(50)]
    public string? Abbreviation { get; set; }

    [InverseProperty("Region")]
    public virtual ICollection<Adminregion> Adminregions { get; set; } = new List<Adminregion>();

    [InverseProperty("Region")]
    public virtual ICollection<Concierge> Concierges { get; set; } = new List<Concierge>();

    [InverseProperty("Region")]
    public virtual ICollection<Physicianregion> Physicianregions { get; set; } = new List<Physicianregion>();

    [InverseProperty("Region")]
    public virtual ICollection<Physician> Physicians { get; set; } = new List<Physician>();

    [InverseProperty("Region")]
    public virtual ICollection<Requestclient> Requestclients { get; set; } = new List<Requestclient>();

    [InverseProperty("Region")]
    public virtual ICollection<Shiftdetailregion> Shiftdetailregions { get; set; } = new List<Shiftdetailregion>();
}
