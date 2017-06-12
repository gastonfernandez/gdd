using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberFrba.Mappings
{
    public class Rol
    {
        public Int32 rolId;
        public String nombre;
        public Boolean habilitado;
        public Boolean seleccionado;

        public static Rol AgregarRol(Rol rolNuevo)
        {
            BaseDeDatos db = new BaseDeDatos();

            Int64 Habilitado = rolNuevo.habilitado ? 1 : 0;

            db.query("Insert into  OSNR.Rol (rol_nombre, rol_habilitado)" +
                     "values('" + rolNuevo.nombre + "'," +Habilitado+")");

            return recuperarRolPorNombre(rolNuevo.nombre);
            
        }




        public static Rol recuperarRolPorNombre(String nombreRol)
        {


            BaseDeDatos db = new BaseDeDatos();

            DataTable dt = db.select_query("  select rol_id,rol_nombre,rol_habilitado from    OSNR.Rol   where rol_nombre='" + nombreRol+"'");

            if (dt.Rows.Count == 0)
                return null;


            Rol rol = new Rol();
            foreach (DataRow row in dt.Rows)
            {

                rol.rolId = Convert.ToInt32(row["rol_id"]);
                rol.nombre = Convert.ToString(row["rol_nombre"]);
                rol.habilitado = Convert.ToBoolean(row["rol_habilitado"]);


            }
            return rol;
        }

        public static Rol recuperarRolPorId(Int64 idRol)
        {
            
            BaseDeDatos db = new BaseDeDatos();

            DataTable dt = db.select_query("  select rol_id,rol_nombre,rol_habilitado from    OSNR.Rol   where rol_id="+idRol );

            if (dt.Rows.Count > 1)
                throw new Exception("Id Duplicado");

            Rol rol = new Rol();
            foreach (DataRow row in dt.Rows)
            {
                
                rol.rolId = Convert.ToInt32(row["rol_id"]);
                rol.nombre = Convert.ToString(row["rol_nombre"]);
                rol.habilitado = Convert.ToBoolean(row["rol_habilitado"]);


            }

            return rol;

        }

        public void BorrarFuncionalidades()
        {
            BaseDeDatos db = new BaseDeDatos();
            db.query("delete from   OSNR.FuncionalidadRol " +
                    "where funcrol_id_rol=" + this.rolId); 
  

        }

        public void AgregarFuncionalidad( Int32 idFuncionalidad)
        {
            BaseDeDatos db = new BaseDeDatos();
            db.query("Insert into OSNR.FuncionalidadRol  (funcrol_id_rol,funcrol_id_funcionalidad)" +
                    "values ("+ this.rolId+","+idFuncionalidad+")");


        }

        public List<Int32> ObtenerFuncionalidades()
        {
            BaseDeDatos db = new BaseDeDatos();
            DataTable dt = db.select_query("SELECT * FROM OSNR.FuncionalidadRol" +
                    " WHERE funcrol_id_rol=" + this.rolId);

            List<Int32> idsFuncionalidades = new List<Int32>();

            foreach (DataRow row in dt.Rows)
            {
                idsFuncionalidades.Add(Convert.ToInt32(row["funcrol_id_funcionalidad"]));
            }

            return idsFuncionalidades;
        }

        public void Guardar()
        {
            BaseDeDatos db = new BaseDeDatos();

            Int64 Habilitado = this.habilitado ? 1 : 0;

            db.query("update  OSNR.Rol "+
                     "set     rol_nombre='"+this.nombre+"',"+
                     "        rol_habilitado="+ Habilitado  +
                     "where   rol_id="+this.rolId
                     );
                
            
        }





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

        public static List<Rol> recuperarRoles()
        {
            List<Rol> roles = new List<Rol>();

            BaseDeDatos db = new BaseDeDatos();
            DataTable dt = db.select_query("select rol_id,rol_nombre,rol_habilitado from OSNR.Rol");

            foreach (DataRow row in dt.Rows)
            {
                Rol rol = new Rol();
                rol.rolId = Convert.ToInt32(row["rol_id"]);
                rol.nombre = Convert.ToString(row["rol_nombre"]);
                rol.habilitado = Convert.ToBoolean(row["rol_habilitado"]);
                roles.Add(rol);
            }

            return roles;
        }

    }
}
