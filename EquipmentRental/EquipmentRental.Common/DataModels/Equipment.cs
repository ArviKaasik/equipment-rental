using System.Runtime.Serialization;

namespace EquipmentRental.Common.DataModels
{
    public class Equipment
    {
        public string Name { get; set; }
        
        public EquipmentType EquipmentType { get; set; }
    }

    public enum EquipmentType : short
    {
        [EnumMember(Value = "Regular")]
        Regular = 0,
        [EnumMember(Value = "Heavy")]
        Heavy = 1,
        [EnumMember(Value = "Specialized")]
        Specialized = 2
    }
}