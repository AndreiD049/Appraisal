using System;
using System.Collections.Generic;
using System.Text;

namespace AppRaisal.Data
{
    public enum PeriodStatus
    {
        Active,
        Finished
    }

    public enum ItemStatus
    {
        Active,
        Finished,
        Cancelled,
        InProgress
    }

    public enum ItemType
    {
        Planned,
        Achieved,
        SWOT_Strength,
        SWOT_Weakness,
        SWOT_Threat,
        SWOT_Opportunity,
        Training,
        Training_Suggested
    }

}
