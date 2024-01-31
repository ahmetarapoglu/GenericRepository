using System.ComponentModel;

namespace GenericRepository.Models.DataTableRequests
{
    public class DataTableRequest
    {
        [DefaultValue(1)]
        public int Current { get; set; }
        [DefaultValue(100)]
        public int PageSize { get; set; }
        [DefaultValue(null)]
        public string? Search { get; set; }
        [DefaultValue("id")]
        public string? Order { get; set; }
        [DefaultValue("ascend")]
        public string? SortDir { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public int Skip => (Current - 1) * PageSize;
        public int Take => PageSize;
    }
}
