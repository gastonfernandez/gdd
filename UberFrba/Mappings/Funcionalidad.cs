using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberFrba.Mappings
{
    class Funcionalidad
    {
        public Int32 funcId;
        public String nombre;
    

        public static List<Funcionalidad> RecuperarFuncionalidades ()
        {
            List<Funcionalidad> funcionalidades = new List<Funcionalidad>();

            BaseDeDatos db = new BaseDeDatos();

            DataTable dt = db.select_query("  select fun_id,fun_nombre from    OSNR.Funcionalidad ");


            foreach (DataRow row in dt.Rows)
            {
                Funcionalidad func = new Funcionalidad();
                func.funcId = Convert.ToInt32(row["fun_id"]);
                func.nombre = Convert.ToString(row["fun_nombre"]);
                
                funcionalidades.Add(func);
            }

            return funcionalidades;


        }

        public static List<Funcionalidad> RecuperarFuncionalidadesPorIdRol(Int32 idRol)
        {
            List<Funcionalidad> funcionalidades = new List<Funcionalidad>();

            BaseDeDatos db = new BaseDeDatos();

            DataTable dt = db.select_query("  select f.fun_id,f.fun_nombre from    OSNR.Funcionalidad f Join OSNR.FuncionalidadRol fr on fr.funcrol_id_funcionalidad=f.fun_id and fr.funcrol_id_rol= "+idRol);


            foreach (DataRow row in dt.Rows)
            {
                Funcionalidad func = new Funcionalidad();
                func.funcId = Convert.ToInt32(row["fun_id"]);
                func.nombre = Convert.ToString(row["fun_nombre"]);

                funcionalidades.Add(func);
            }

            return funcionalidades;


        }
    }
}
