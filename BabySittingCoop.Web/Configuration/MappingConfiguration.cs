using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BabySittingCoop.Domain;
using FluentNHibernate.Automapping;

namespace BabySittingCoop.Migrations
{
    public class MappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.Namespace == typeof(BabySitter).Namespace;
        }
    }
}
