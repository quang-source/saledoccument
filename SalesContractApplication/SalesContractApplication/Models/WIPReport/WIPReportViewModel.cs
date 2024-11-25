

namespace SalesContractApplication.Models
{
    public class WIPReportViewModel
    {
        public List<WIPReportModel> WIPReportList { get; set; }

        public WIPReportViewModel()
        {
            WIPReportList = [];
        }
    }
}
