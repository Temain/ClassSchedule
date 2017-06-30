namespace ClassSchedule.Business.Models.SelectFlow
{
    public class GroupViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int? Order { get; set; }

        public bool IsSelected { get; set; }
    }
}