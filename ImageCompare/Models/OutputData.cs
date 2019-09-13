namespace ImageCompare.Models
{
    public class OutputData
    {
        public string Image1 { get; set; }
        public string Image2 { get; set; }

        //Bjorn is entrusting you to figure out an appropriate scoring algorithm, although he is requesting that 0 indicates that the pair are the same image.
        public float Similar { get; set; }

        //time spent to calculate the Similar
        public float Elapsed { get; set; }
    }
}
