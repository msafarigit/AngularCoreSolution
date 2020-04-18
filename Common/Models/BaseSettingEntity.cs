using System;

namespace Common.Models
{
    public class BaseSettingEntity : TrackableEntity
    {
        public long Id { get; set; }
        public byte Order { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
    }

    public abstract class TrackableEntity : IEntityCreator, IEntityModifier
    {
        public DateTime DateCreated { get; set; }
        public string UserCreated { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserModified { get; set; }
    }


    public interface IEntityCreator
    {
        string UserCreated { get; set; }
        DateTime DateCreated { get; set; }
    }

    public interface IEntityModifier
    {
        string UserModified { get; set; }
        DateTime? DateModified { get; set; }
    }

    public interface IPartitionedEntity
    {
        int PersianYear { get; set; }
        int PersianMonth { get; set; }
    }
}
