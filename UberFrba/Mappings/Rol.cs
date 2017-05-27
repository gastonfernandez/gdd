using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberFrba.Mappings
{
    class Rol
    {
        public Int32 rolId;
        public String nombre;
        public Boolean habilitado;
        public Boolean seleccionado;

        public static List<Rol> recuperarRolPorUsuario (Int32 Id)
        {
            List<Rol> rolesUsuario = new List<Rol>();

            BaseDeDatos db = new BaseDeDatos();

            DataTable dt = db.select_query("  select r.rol_id rol_id,rol_nombre,rol_habilitado from    OSNR.Usuario u	join OSNR.UsuarioRol ur on ur.usurol_id_usuario=u.usu_id join OSNR.Rol r on ur.usurol_id_rol=r.rol_id where	r.rol_habilitado=1 and  u.usu_id="+Id.ToString() );


            foreach (DataRow row in dt.Rows)
            {
                Rol rol = new Rol();
                rol.rolId = Convert.ToInt32(row["rol_id"]);
                rol.nombre = Convert.ToString(row["rol_nombre"]);
                rol.habilitado = Convert.ToBoolean(row["rol_habilitado"]);
                

                rolesUsuario.Add(rol);
            }

            return rolesUsuario;

        }
    }
}
