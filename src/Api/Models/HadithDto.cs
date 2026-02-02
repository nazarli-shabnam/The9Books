namespace The9Books.Models
{
    public class HadithDto
    {
        public int Number { get; set; }
        public string Hadith { get; set; }
        public string Tafseel { get; set; }
        public string Book { get; set; }

        public HadithDto(Hadith hadith)
        {
            Number = hadith.Number;
            Hadith = hadith.HadithText ?? string.Empty;
            Tafseel = hadith.Tafseel ?? string.Empty;
            Book = hadith.Book ?? string.Empty;
        }
    }
}