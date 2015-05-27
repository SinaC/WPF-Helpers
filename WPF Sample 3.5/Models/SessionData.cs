namespace SampleWPF.Models
{
    public class SessionData
    {
        public RequestDetailData RequestDetailData { get; set; }

        public SessionData()
        {
            RequestDetailData = new RequestDetailData();
        }
    }
}
