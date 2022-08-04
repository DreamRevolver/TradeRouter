using System.Configuration;

namespace SharedBinance.Models.ProxyConfigurationSection
{
    public class ProxyInstanceElement: ConfigurationElement
    {
        [ConfigurationProperty("server", IsKey = true, IsRequired = true)]
        public string Server
        {
            get
            {
                return (string)base["server"];
            }
            set
            {
                base["server"] = value;
            }
        }
        [ConfigurationProperty("login", IsRequired = true)]
        public string Login
        {
            get
            {
                return (string)base["login"];
            }
            set
            {
                base["login"] = value;
            }
        }
        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)base["password"];
            }
            set
            {
                base["password"] = value;
            }
        }
        [ConfigurationProperty("port", IsRequired = true)]
        public string Port
        {
            get
            {
                return (string)base["port"];
            }
            set
            {
                base["port"] = value;
            }
        }
        [ConfigurationProperty("installationName", IsRequired = false, DefaultValue = "PROXY")]
        public string InstallationName
        {
            get
            {
                return (string)base["installationName"];
            }
            set
            {
                base["installationName"] = value;
            }
        }
    }
    public class ProxyInstanceCollection : ConfigurationElementCollection
    {
        public ProxyInstanceElement this[int index]
        {
            get
            {
                return (ProxyInstanceElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }
        public new ProxyInstanceElement this[string key]
        {
            get
            {
                return (ProxyInstanceElement)BaseGet(key);
            }
            set
            {
                if (BaseGet(key) != null)
                    BaseRemoveAt(BaseIndexOf(BaseGet(key)));
                BaseAdd(value);
            }
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProxyInstanceElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProxyInstanceElement)element).Server;
        }
    }
    public class ProxyConfig : ConfigurationSection
    {
        [ConfigurationProperty("instances")]
        [ConfigurationCollection(typeof(ProxyInstanceCollection))]
        public ProxyInstanceCollection ProxyInstance
        {
            get
            {
                return (ProxyInstanceCollection)this["instances"];
            }
        }
    }

}
