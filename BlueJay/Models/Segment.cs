namespace BlueJay
{
    public class Segment<T>
    {
        // Total number of items in all segments. 
        public int count { get; set; }

        // Pass this as as a "cursor" parameter to paginate. 
        public string next { get; set; }

        public T[] results { get; set; }
    }
}