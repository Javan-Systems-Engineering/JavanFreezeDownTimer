using System;

namespace FreezeDownTimer.Models.Interfaces
{
    public interface iModficationHistory
    {
        DateTime InsertDate { get; set; }
        DateTime? LastUpdate { get; set; }

        int? LastUpdateBy { get; set; }

        bool IsDirty { get; set; }


    }
}