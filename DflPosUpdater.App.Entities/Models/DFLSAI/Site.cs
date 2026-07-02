using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DflPosUpdater.App.Entities.DFLSAI;

[Table("SITES")]
[Index("SiteCode", Name = "IX_SITES_Code")]
public partial class Site
{
    [Key]
    [Column("site_code")]
    [StringLength(4)]
    [Unicode(false)]
    public string SiteCode { get; set; } = null!;

    [Column("site_name")]
    [StringLength(50)]
    public string? SiteName { get; set; }

    [Column("site_type")]
    [StringLength(20)]
    public string? SiteType { get; set; }

    [Column("ip_address")]
    [StringLength(20)]
    public string? IpAddress { get; set; }

    [Column("port")]
    [StringLength(4)]
    public string? Port { get; set; }

    [Column("address")]
    [StringLength(100)]
    public string? Address { get; set; }

    [Column("postal_code")]
    [StringLength(10)]
    public string? PostalCode { get; set; }

    [Column("city")]
    [StringLength(50)]
    public string? City { get; set; }

    [Column("state")]
    [StringLength(50)]
    public string? State { get; set; }

    [Column("email_manager")]
    [StringLength(50)]
    public string? EmailManager { get; set; }

    [Column("phone")]
    [StringLength(50)]
    public string? Phone { get; set; }

    [Column("acc_store_id")]
    [StringLength(10)]
    public string? AccStoreId { get; set; }

    [Column("district_code")]
    [StringLength(4)]
    [Unicode(false)]
    public string? DistrictCode { get; set; }

    [Column("delete_flag")]
    public bool? DeleteFlag { get; set; }

    [Column("cdate", TypeName = "datetime")]
    public DateTime? Cdate { get; set; }

    [Column("cuser")]
    [StringLength(20)]
    public string? Cuser { get; set; }

    [Column("udate")]
    public DateOnly? Udate { get; set; }

    [Column("uuser")]
    [StringLength(20)]
    public string? Uuser { get; set; }

    [Column("site_connection_string", TypeName = "text")]
    public string? SiteConnectionString { get; set; }

    [Column("site_serie")]
    [StringLength(3)]
    [Unicode(false)]
    public string? SiteSerie { get; set; }

    [Column("new_erp")]
    public bool NewErp { get; set; }

    [Column("new_erp_date", TypeName = "datetime")]
    public DateTime? NewErpDate { get; set; }

    [Column("corresponsal_bank")]
    [StringLength(20)]
    [Unicode(false)]
    public string? CorresponsalBank { get; set; }

    [Column("corresponsal_id")]
    public long? CorresponsalId { get; set; }

    [Column("host_name")]
    [StringLength(30)]
    public string? HostName { get; set; }

    [Column("store_sale_iva", TypeName = "decimal(5, 2)")]
    public decimal? StoreSaleIva { get; set; }

    [Column("desk_app_con")]
    [MaxLength(300)]
    public byte[]? DeskAppCon { get; set; }
}
