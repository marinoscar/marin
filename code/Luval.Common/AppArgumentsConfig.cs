using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace Luval.Common
{
    public class AppArgumentsConfig : IConfiguration
    {
        public AppArgumentsConfig(IEnumerable<string> args)
        {
            AppArguments = new AppArguments(args);
        }

        public string this[string key] { get {
                if (AppArguments.ContainsSwitch("/" + key))
                    return AppArguments["/" + key];
                else
                    return null;
            } set => throw new NotImplementedException(); }

        protected virtual AppArguments AppArguments { get; private set; }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public IConfigurationSection GetSection(string key)
        {
            throw new NotImplementedException();
        }
    }
}
