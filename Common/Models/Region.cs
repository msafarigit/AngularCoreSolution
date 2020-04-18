namespace Common.Models
{
    public class Region : BaseSettingEntity
    {
        public long CompanyId { get; set; }

        // public virtual Customer Customer { get; set; }
        // public virtual IncidentAnnouncementEntity IncidentAnnouncement { get; set; }
        // public virtual IrrelevantCallTo121 IrrelevantCallTo121 { get; set; }
        // public virtual ICollection<CustomerCallTelephoneNumber> CustomerCallTelephoneNumbers { get; set; } = new List<CustomerCallTelephoneNumber>();
    }
}
