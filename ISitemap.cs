namespace X.Web.Sitemap
{
    public interface ISitemap
    {
        bool Save(string path);
        bool SaveToDirectory(string directory);
        string ToXml();
    }
}