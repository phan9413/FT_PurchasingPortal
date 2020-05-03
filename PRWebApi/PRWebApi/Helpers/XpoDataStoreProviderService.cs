using DevExpress.ExpressApp.Xpo;
using System.Data;

namespace PRWebApi.Helpers
{
    public class XpoDataStoreProviderService
    {
		private IXpoDataStoreProvider dataStoreProvider;
		public IXpoDataStoreProvider GetDataStoreProvider(string connectionString, IDbConnection connection, bool enablePoolingInConnectionString)
		{
			if (dataStoreProvider == null)
			{
				dataStoreProvider = XPObjectSpaceProvider.GetDataStoreProvider(connectionString, connection, enablePoolingInConnectionString);
			}
			return dataStoreProvider;
		}
	}
}
