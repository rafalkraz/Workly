using System.Threading.Tasks;
using Workly.Structure;

namespace Workly.Interfaces;

public interface IDataViewPage
{
    void RefreshEntryList(string year = null, Month month = null);
    void ShowDataError(string title, string content);
}
