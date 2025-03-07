using System;
using System.Collections.Generic;

namespace identity.Core.Domain.Entities;

public partial class Zone
{
    public long ZoneId { get; set; }

    public string? ZoneName { get; set; }

    public string? ZoneCode { get; set; }

    public byte? ZoneStatus { get; set; }

    public string? EstablishDate { get; set; }

    public string? LatinName { get; set; }

    public string? BreakupDate { get; set; }

    public long? ParentId { get; set; }

    public string? PishsShomareh { get; set; }

    public int? HczoneLevelCode { get; set; }

    public DateTime? InsertDate { get; set; }

    public string? InsertUser { get; set; }

    public DateTime? UpdateDate { get; set; }

    public string? UpdateUser { get; set; }

    public string HczoneLevelName { get; set; } = null!;

    public string? ZoneStatusName { get; set; }

    public string? TarikhSabt { get; set; }

    public bool? IsMetropolis { get; set; }

    public bool IsCenter { get; set; }

    public int? Ostan { get; set; }

    public int? City { get; set; }

    public int? Bakhsh { get; set; }

    public string? Eteuser { get; set; }

    public string? Etepass { get; set; }

    public double? Lat { get; set; }

    public double? Lng { get; set; }

    public double? Khanevar { get; set; }

    public double? Jameiat { get; set; }

    public double? Man { get; set; }

    public double? Woman { get; set; }

    public int? Gol { get; set; }

    public string? VillageCode { get; set; }

    public int? Grade { get; set; }

    public string? Parentname { get; set; }

    public string? Ostanname { get; set; }

    public string? Cityname { get; set; }

    public string? Bakhshname { get; set; }

    public bool? IsLockWhenInfoNotComplete { get; set; }

    public bool? IsDehyari { get; set; }

    public bool? IsUnderBakhshConcil { get; set; }

    public string? NationalCode { get; set; }

    public bool LocationFlag { get; set; }

    public int? UserVerifiesLocation { get; set; }

    public bool? FireStations { get; set; }

    public Guid FileId { get; set; }

    public string? MojavezTasis { get; set; }

    public string? MojavezDate { get; set; }

    public string? StaticIp { get; set; }

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
