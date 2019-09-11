using Newtonsoft.Json.Linq;
using System;

namespace PetoGraphics.Providers
{
    public class Store
    {
        private static Store instance;

        private JObject data = new JObject();

        public event EventHandler<JObject> OnUpdate;

        public static Store Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Store();
                }
                return instance;
            }
        }

        public JObject Data
        {
            get
            {
                return data;
            }
        }

        public void UpdateStore(string key, JObject value)
        {
            data[key] = value;
            OnUpdate?.Invoke(this, data);
        }

    }
}
