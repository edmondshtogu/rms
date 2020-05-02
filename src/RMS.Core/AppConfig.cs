using System;
using System.Xml;
using System.Configuration;

namespace RMS.Core
{
    /// <summary>
    /// Represents a AppConfig
    /// </summary>
    public sealed class AppConfig : IConfigurationSectionHandler
    {
        /// <summary>
        /// Creates a configuration section handler.
        /// </summary>
        /// <param name="parent">Parent object.</param>
        /// <param name="configContext">Configuration context object.</param>
        /// <param name="section">Section XML node.</param>
        /// <returns>The created section handler object.</returns>
        public object Create(object parent, object configContext, XmlNode section)
        {
            var config = new AppConfig();

            var provider = GetString(section.SelectSingleNode("dataProvider"), "value");
            switch (provider.ToLowerInvariant())
            {
                case "sqlserver":
                case "sqlce":
                    config.DataProvider = provider;
                    break;
                default:
                    throw new GeneralException(string.Format("Not supported dataprovider name: {0}", provider));
            }

            var connectionString = section.SelectSingleNode("dataConnectionString");
            config.DataConnectionString = GetString(connectionString, "value");

            return config;
        }


        /// <summary>
        /// Data provider
        /// </summary>
        /// <value>
        /// The data provider.
        /// </value>
        public string DataProvider { get; set; }

        /// <summary>
        /// Gets the data connection string.
        /// </summary>
        /// <value>
        /// The connection string.
        /// </value>
        public string DataConnectionString { get; private set; }

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns></returns>
        private string GetString(XmlNode node, string attrName)
        {
            return SetByXElement(node, attrName, Convert.ToString);
        }

        /// <summary>
        /// Gets the bool.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <returns></returns>
#pragma warning disable IDE0051 // Remove unused private members
        private bool GetBool(XmlNode node, string attrName)
#pragma warning restore IDE0051 // Remove unused private members
        {
            return SetByXElement(node, attrName, Convert.ToBoolean);
        }

        /// <summary>
        /// Sets the by x element.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node">The node.</param>
        /// <param name="attrName">Name of the attribute.</param>
        /// <param name="converter">The converter.</param>
        /// <returns></returns>
        private T SetByXElement<T>(XmlNode node, string attrName, Func<string, T> converter)
        {
            if (node == null || node.Attributes == null) return default;
            var attr = node.Attributes[attrName];
            if (attr == null) return default;
            var attrVal = attr.Value;
            return converter(attrVal);
        }
    }
}
