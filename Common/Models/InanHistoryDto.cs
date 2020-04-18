using System;

namespace Common.Models
{
    public class InanHistoryDto
    {
        public DateTime IncidentDate { get; set; }
        public long IncidentTypeId { get; set; }
        public long RegionId { get; set; }
        public long DistrictId { get; set; }
        public string Address { get; set; }
        public string MainReason { get; set; }
        public long IncidentAnnouncementId { get; set; }
        public long ChildId { get; set; }
        public byte ChildTypeId { get; set; }
    }
}
