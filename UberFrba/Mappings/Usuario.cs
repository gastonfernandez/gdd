using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UberFrba.Mappings;

namespace UberFrba.Mappings
{

    class Usuario
    {
        public Int32 Id = 0;
        public Int32 dni;
        public String nombre;
        public String apellido;
        public String direccion;
        public String telefono;
        public String mail;
        public DateTime? fecNac;
        public String username = "";
        public String pass = "";
        public Int32 intentos;
        public Int32 habilitado;
        public List<Rol> roles;


        public Usuario getUser(String login, String pass)
        {
            string valorEncriptado = SHA256Encripta(pass);

            #region ValidarUsuarioyPass Contra la base
            BaseDeDatos db = new BaseDeDatos();

            DataTable dt = db.select_query("  select usu_id,usu_dni,usu_nombre,usu_apellido,usu_direccion ,usu_telefono,usu_mail,usu_fecha_nacimiento,usu_login,convert(varchar(2000),usu_password,2) as usu_password,usu_cantidad_intentos,usu_habilitado from [OSNR].USUARIO where usu_login= '" + login + "'");

            if (dt.Rows.Count > 1)
            {
                throw new Exception("Se produjo un problema al intentar iniciar sesion por favor concatese con el administrador");

            }
            else
             {
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("El usuario ingresado es inexistente");
                }
                else
                {


                    foreach (DataRow row in dt.Rows)
                    {
                        this.Id = Convert.ToInt32(row["usu_id"]);
                        this.dni = Convert.ToInt32(row["usu_dni"]);
                        this.nombre = Convert.ToString(row["usu_nombre"]);
                        this.apellido = Convert.ToString(row["usu_apellido"]);
                        this.direccion = Convert.ToString(row["usu_direccion"]);
                        this.telefono = Convert.ToString(row["usu_telefono"]);
                        this.mail = Convert.ToString(row["usu_mail"]);
                         if (fecNac!= null) 
                            this.fecNac = Convert.ToDateTime(row["usu_fecha_nacimiento"]);
                        this.username = Convert.ToString(row["usu_login"]);
                        this.pass = Convert.ToString(row["usu_password"]);
                        this.intentos = Convert.ToInt32(row["usu_cantidad_intentos"]);
                        this.habilitado = Convert.ToInt32(row["usu_habilitado"]);

                    }
                }
            }

            if (this.habilitado == 0)
            {
                throw new Exception("El usuario se encuentra bloqueado");
            }
            else
            {
                #region CompararValor Ingresado contra la base
                String Msg = String.Empty;
                if (this.pass == valorEncriptado.ToUpper())
                {
                    Msg = "OK";
                    this.habilitado = 1;
                    this.intentos = 0;
                }
                else
                {
                    Msg = "ERR";
                    if (this.intentos == 2)
                        this.habilitado = 0;
                    else
                        this.intentos++;
                }
                #endregion

                #region ModificarValor en base a lo procesado
                string update = "update [OSNR].usuario " +
                                 " set usu_cantidad_intentos= " + this.intentos + "," +
                                 "usu_habilitado= " + this.habilitado +
                                 " where usu_id= " + this.Id;
                db.query(update);
                #endregion

                if (Msg == "ERR")
                    return null;
                else
                    return this;
            }
            #endregion


        }

        private string SHA256Encripta(string input)
        {
            SHA256CryptoServiceProvider provider = new SHA256CryptoServiceProvider();

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashedBytes = provider.ComputeHash(inputBytes);

            StringBuilder output = new StringBuilder();

            for (int i = 0; i < hashedBytes.Length; i++)
                output.Append(hashedBytes[i].ToString("x2").ToLower());

            return output.ToString();
        }
    }
}
