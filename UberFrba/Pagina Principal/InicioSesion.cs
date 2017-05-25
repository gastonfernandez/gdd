using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UberFrba.Mappings;

namespace UberFrba.Pagina_Principal
{
    public partial class InicioSesion : Form
    {
        public InicioSesion()
        {
            InitializeComponent();
        }

        private void Ingresar_click(object sender, EventArgs e)
        {
           

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Ingresar_Click(object sender, EventArgs e)
        {
            // Llamo al Menú(comentario anterior)
            MenuPpal frm = new Menu.MenuPpal();//Comentario actual: no entiendo que deberia hacer esto.
            this.Hide();
            frm.Show();
            /* el pass de prueba que use es AAAA para el usuario PEPE(comentario anterior)*/

            try
            {
                #region ValidarParametros y Usuario
                if (!validoParametros(textoUsuario, textoPassword))
                {
                    MessageBox.Show("Ingrese todos los valores");

                }
                else
                {

                    #region ObtenerUsuario
                    Usuario usu = new Usuario();

                    usu = usu.getUser(textoUsuario.Text, textoPassword.Text);
                    #endregion

                    if (usu == null)
                        MessageBox.Show("La contraseña ingresada es incorrecta");
                    else
                        MessageBox.Show("Se ha logueado correctamente"); /* aca hay que hacer lo que siga*/


                }

                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message); //throw new Exception("Se ha producido un error en el momento de realizar el logueo consulte al administrador" + ex.Message);
            }

        }

        private Boolean validoParametros(TextBox usuario, TextBox pass)
        {
            if (usuario.Text == string.Empty || pass.Text == string.Empty)
                return false;
            else
                return true;
        }

    }
}
