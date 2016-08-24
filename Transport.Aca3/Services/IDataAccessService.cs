using System.Collections.Generic;
using Transport.Aca3.Models;
using Transport.Aca3.ViewModels;

namespace Transport.Aca3.Services
{
    public interface IDataAccessService
    {
        List<NodeViewModel> GetGraphNodes();
    }
}