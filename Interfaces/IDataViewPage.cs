using WorkLog.Structure;

namespace WorkLog.Interfaces;

public interface IDataViewPage
{
    void RefreshEntryList(string year = null, Month month = null);
    void ShowDataError(string title, string content);
}
