namespace EquipmentRental.Common.DataModels
{
    public class EquipmentItem
    {
        public string Name { get; set; }

        //TODO fix me
        // This doesn't work. I suspect because razor does some internal
        // mapping instead of using json conversions. I'll make api requests with short value instead
        //[JsonConverter(typeof(StringEnumConverter))]
        public EquipmentType EquipmentType { get; set; }

        public int RentalDays { get; set; }
    }
}