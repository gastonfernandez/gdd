using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberFrba.Mappings
{
    public class DbTypedValue
    {
        private Object value;
        private SqlDbType type;

        public DbTypedValue(object valor, SqlDbType type)
        {
            this.value = value.ToString();
            this.type = type;
        }

        public DbTypedValue(DateTime valor, SqlDbType type)
        {
            this.value = valor;
            this.type = type;
        }


        public DbTypedValue(String value, SqlDbType type)
        {
            this.value = value;
            this.type = type;
        }

        public Object getValue()
        {
            return value;
        }
        public SqlDbType getType()
        {
            return type;
        }
    }
}
