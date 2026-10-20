using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace SharedCode
{
    public static class Config
    {
        // A standard way to obtain a connection string from the config file

        public static System.Configuration.ConnectionStringSettings GetDBConnectionSettings(string DBName)
        {
            System.Configuration.ConnectionStringSettingsCollection connections = System.Configuration.ConfigurationManager.ConnectionStrings;

            System.Configuration.ConnectionStringSettings result = new System.Configuration.ConnectionStringSettings();

            foreach (System.Configuration.ConnectionStringSettings connection in connections)
            {
                if (connection.Name == DBName)
                {
                    result = connection;
                }
            }

            return result;
        }
    }

    public static class ControlExtensions
    {
        // Recursively finds a child control of the specified parent.

        public static Control FindControlRecursive(this Control control, string id)
        {
            if (control == null) return null;

            //try to find the control at the current level
            Control ctrl = control.FindControl(id);

            if (ctrl == null)
            {
                //search the children
                foreach (Control child in control.Controls)
                {
                    ctrl = FindControlRecursive(child, id);
                    if (ctrl != null) break;
                }
            }

            return ctrl;
        }
    }

}