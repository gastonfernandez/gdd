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

namespace UberFrba
{
    public partial class EleccionRol : Form
    {
        private Usuario usuario;
        public EleccionRol()
        {
            InitializeComponent();
        }

        private void EleccionRol_Load(object sender, EventArgs e)
        {
            List<Combo> lista = new List<Combo>();

            foreach (Rol rol in usuario.roles)
                lista.Add(new Combo (rol.nombre, rol.rolId));


            comboBox1.DisplayMember = "rolId";
            comboBox1.ValueMember = "rol_nombre";
            comboBox1.DataSource = lista; 
                 
        }

        internal void Show(Mappings.Usuario usu)
        {
            usuario = usu;
            
            this.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Combo comb = (Combo)comboBox1.SelectedItem;
            foreach (Rol rol in usuario.roles)
                if (comb.Value == rol.rolId)
                    rol.seleccionado = true;

            //Hay que llamar al menu principal con el usuario, una opcion es eliminar el rol que no va la otra es dejarlo con el selected

        }   

    }
}
