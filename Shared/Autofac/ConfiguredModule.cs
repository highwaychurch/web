using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;

namespace Highway.Shared.Autofac
{
    public class ConfiguredModules : Module
    {
        private readonly IList<Module> _modules;

        public ConfiguredModules(IList<Module> modules)
        {
            _modules = modules;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var settings = ConfigurationManager.AppSettings;
            var keys = settings.AllKeys;

            foreach (var setting in keys)
            {
                var parts = setting.Split('.');
                if (parts.Count() == 2)
                {
                    var moduleName = parts[0];
                    var propertyName = parts[1];
                    var value = settings[setting];

                    var module = _modules.First(x => x.GetType().Name == moduleName + "Module");
                    var property = module.GetType().GetProperty(propertyName);
                    property.SetValue(module, Convert.ChangeType(value, property.PropertyType), null);
                }
            }

            foreach (var module in _modules)
            {
                builder.RegisterModule(module);
            }
        }
    }
}
