namespace Fanex.NCB.Common.DTO
{
    public class Menu
    {
        public string Url { get; set; }
        public short DisplayOrder { get; set; }
        public int Policy { get; set; }
        public Menu Parent { get; set; }
        public string Name { get; set; }
    }
}
