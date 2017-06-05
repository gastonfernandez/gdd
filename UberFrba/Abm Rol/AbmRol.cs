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

namespace UberFrba.Abm_Rol
{
    public partial class AbmRol : Form
    {
        public AbmRol()
        {
            InitializeComponent();
         
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<Combo> lista = new List<Combo>();

            List<Rol> roles = Rol.recuperarRoles();
            foreach (Rol rol in roles)
                lista.Add(new Combo(rol.nombre, rol.rolId));


            comboBox1.DisplayMember = "rolId";
            comboBox1.ValueMember = "rol_nombre";
            comboBox1.DataSource = lista; 
        }

        private void btnelim_Click(object sender, EventArgs e)
        {
            Combo comb = (Combo)comboBox1.SelectedItem;

            Rol rol = Rol.recuperarRolPorId(comb.Value);

            if (rol.habilitado)
            {
                rol.habilitado = false;
                rol.Guardar();
                MessageBox.Show("El rol ha sido inhabilitado");
            }
            else
                MessageBox.Show("El rol ya se encuentra inhabilitado, si desea editarlo presione modificar");

            
        }

        private void btnmod_Click(object sender, EventArgs e)
        {
            EditarRol editRol = new EditarRol();
            Combo comb = (Combo)comboBox1.SelectedItem;
            Rol rol = Rol.recuperarRolPorId(comb.Value);
            editRol.Show(rol);
        }

        private void btnnuevo_Click(object sender, EventArgs e)
        {
            EditarRol editRol = new EditarRol();
            editRol.Show(null);

        }

        

        
    }
}
