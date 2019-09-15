namespace PageNotFoundManager
{
    public interface IPageNotFoundManagerConfig
    {
        int GetNotFoundPage(int parentId);
        void RefreshCache();
        void SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache);
    }
}