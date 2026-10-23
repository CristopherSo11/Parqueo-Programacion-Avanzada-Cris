using System;
using System.Collections.Generic;

namespace Parqueo.Model.Entities;

public partial class PrqAutomovil
{
    public int Id { get; set; }

    public string Color { get; set; } = null!;

    public int Anio { get; set; }

    public string Fabricante { get; set; } = null!;

    public string Tipo { get; set; } = null!;

    public virtual ICollection<PrqIngresoAutomovil> PrqIngresoAutomovils { get; set; } = new List<PrqIngresoAutomovil>();
}
