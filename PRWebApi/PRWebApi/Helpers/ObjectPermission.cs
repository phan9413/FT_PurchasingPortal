using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRWebApi.Helpers
{
    public class ObjectPermission
    {
		public IDictionary<string, object> Data { get; set; }
		public string Key { get; set; }
		public bool Write { get; set; }
		public bool Delete { get; set; }
		public ObjectPermission()
		{
			Data = new Dictionary<string, object>();
		}
	}
}
