using System;
using System.Collections.Generic;

namespace AtlasPatient.Models.Models;

public partial class PatientVisitHistory
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public DateTime? VisitDate { get; set; }

    public string? DoctorName { get; set; }

    public string? NurseName1 { get; set; }

    public string? NurseName2 { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
