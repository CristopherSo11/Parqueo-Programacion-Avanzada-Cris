using System;
using System.Collections.Generic;

namespace Parqueo.Model.Entities;

public partial class PrqIngresoAutomovil
{
    public int Consecutivo { get; set; }

    public int IdParqueo { get; set; }

    public int IdAutomovil { get; set; }

    public DateTime FechaHoraEntrada { get; set; }

    public DateTime? FechaHoraSalida { get; set; }

    public virtual PrqAutomovil IdAutomovilNavigation { get; set; } = null!;

    public virtual PrqParqueo IdParqueoNavigation { get; set; } = null!;
}
