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
    public partial class EditarRol : Form
    {
        public EditarRol()
        {
            InitializeComponent();
        }

        Rol rol;
        

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        internal void Show(Mappings.Rol rolUsu)
        {
            rol = rolUsu;
            this.Show();
        }

        private void EditarRol_Load(object sender, EventArgs e)
        {
            
            List<Funcionalidad> listFunc = Funcionalidad.RecuperarFuncionalidades();
            List<Combo> lista = new List<Combo>();

            foreach (Funcionalidad func in listFunc)
                lista.Add(new Combo(func.nombre, func.funcId));


            checkedListBox1.DisplayMember = "rolId";
            checkedListBox1.ValueMember = "rol_nombre";
            checkedListBox1.DataSource = lista;

            

            if (rol != null)
            {
                List<Funcionalidad> funcRol = Funcionalidad.RecuperarFuncionalidadesPorIdRol(rol.rolId);


                List<Int32> itemsCheck  =  new List<Int32>();
                foreach (Funcionalidad fun in funcRol)
                {
                    Int32 i = 0;
                    foreach (object listBox in checkedListBox1.Items)
                    {
                        Combo comb = (Combo)listBox;
                        if (comb.Value == fun.funcId)
                        {
                            itemsCheck.Add(i);
                        }
                        i++;
                    }
                }

                foreach (Int32 j in itemsCheck)
                    checkedListBox1.SetItemChecked(j, true);

                nombRol.Text = rol.nombre;
                checkHab.Checked = Convert.ToBoolean(rol.habilitado);
                        

            }
                
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Boolean esAlta = false;
                if (rol == null) //es nuevo hay que agregarlo
                {
                    esAlta = true;
                    rol = Rol.recuperarRolPorNombre(nombRol.Text);
                    if (rol != null)
                        throw new Exception("Ya existe un rol con ese nombre no es posible de agregar");

                    Rol rolNuevo = new Rol();
                    rolNuevo.nombre = nombRol.Text;
                    rolNuevo.habilitado = checkHab.Checked;

                    rol = Rol.AgregarRol(rolNuevo);
                }
                else
                {
                    rol.nombre = nombRol.Text;
                    rol.habilitado = checkHab.Checked;
                    rol.Guardar();

                }

                rol.BorrarFuncionalidades();
                
                foreach (object listBox in checkedListBox1.CheckedItems)
                {
                    Combo comb = (Combo)listBox;
                   rol.AgregarFuncionalidad(comb.Value);

                }
                if (esAlta)
                MessageBox.Show("El rol ha sido creado correctamente");
                else
                    MessageBox.Show("El rol ha sido modificado correctamente");
                
                this.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
