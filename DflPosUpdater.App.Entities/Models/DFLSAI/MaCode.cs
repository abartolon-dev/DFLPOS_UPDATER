using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.Entities.DFLSAI;

[PrimaryKey("Code", "Vkey")]
[Table("MA_CODE")]
public partial class MaCode
{
    [Key]
    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Key]
    [Column("vkey")]
    [StringLength(50)]
    public string Vkey { get; set; } = null!;

    [Column("vkey_seq")]
    public int? VkeySeq { get; set; }

    [Column("description")]
    [StringLength(200)]
    public string? Description { get; set; }

    [Column("used")]
    [StringLength(1)]
    public string? Used { get; set; }

    [Column("program_id")]
    [StringLength(50)]
    public string? ProgramId { get; set; }

    [Column("cuser")]
    [StringLength(50)]
    public string? Cuser { get; set; }

    [Column("cdate", TypeName = "datetime")]
    public DateTime? Cdate { get; set; }

    [Column("uuser")]
    [StringLength(50)]
    public string? Uuser { get; set; }

    [Column("udate", TypeName = "datetime")]
    public DateTime? Udate { get; set; }
}
